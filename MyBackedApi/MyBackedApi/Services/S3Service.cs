﻿using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Infrastructure.Config.Models;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Options;
using MyBackedApi.DTOs.Forum;
using MyBackedApi.DTOs.Forum.Responses;
using Newtonsoft.Json;

namespace MyBackedApi.Services
{


    public class S3Service
    {
        private readonly AwsS3Settings _settings;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IOptions<AwsS3Settings> settings)
        {
            _settings = settings.Value;

            _s3Client = new AmazonS3Client(
                _settings.AccessKey,
                _settings.SecretKey,
                Amazon.RegionEndpoint.GetBySystemName(_settings.Region)
            );
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder, string contentType)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);

            string key = $"{folder}/{fileName}";

            var request = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                BucketName = _settings.BucketName,
                Key = key,
                ContentType = contentType
            };

            await fileTransferUtility.UploadAsync(request);

            return $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/{key}";
        }

        public async Task DeleteFileAsync(string fileName, string folder)
        {
            string key = $"{folder}/{fileName}";

            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }

        public async Task<List<string>> AddMultipleFilesAsync(List<IFormFile> files, string folder)
        {
            if (files == null || files.Count == 0)
                throw new WrongInputException("No files provided");

            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                    continue;

                var fileName = file.FileName.ToString();
                var contentType = file.ContentType;

                await using var stream = file.OpenReadStream();
                var url = await UploadFileAsync(stream, fileName, folder, contentType);
                uploadedUrls.Add(url);
            }

            return uploadedUrls;
        }

        public async Task AddFilesToQuestionDetails(GetQuestionDetailsResponse? question)
        {
            if (question == null || string.IsNullOrEmpty(question.Id.ToString()))
                return;

            var bucketName = _settings.BucketName;
            var prefix = $"attachments/{question.Id}/";

            var listRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefix
            };

            var response = await _s3Client.ListObjectsV2Async(listRequest);

            if (response.S3Objects == null || response.S3Objects.Count == 0)
            {
                question.Attachments = new List<AttachmentsDto>();
                return;
            }

            question.Attachments = new List<AttachmentsDto>();

            foreach (var s3Object in response.S3Objects)
            {
                var fileName = s3Object.Key.Substring(prefix.Length);

                var presignedUrl = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = s3Object.Key,
                    Expires = DateTime.UtcNow.AddMinutes(10), // link valabil 10 min
                    ResponseHeaderOverrides = new ResponseHeaderOverrides
                    {
                        ContentDisposition = $"attachment; filename=\"{fileName}\""
                    }
                });

                question.Attachments.Add(new AttachmentsDto
                {
                    Name = fileName,
                    Url = presignedUrl
                });
            }
        }

        public async Task<List<(string FileName, Stream FileStream, string ContentType)>> GetQuestionAttachmentsAsync(Guid questionId)
        {
            var folderPrefix = $"attachments/{questionId}/";
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _settings.BucketName,
                Prefix = folderPrefix
            };

            var response = await _s3Client.ListObjectsV2Async(listRequest);

            var files = new List<(string FileName, Stream FileStream, string ContentType)>();

            if (response.S3Objects == null || response.S3Objects.Count == 0)
                return files; // returnează listă goală

            foreach (var s3Object in response.S3Objects)
            {
                if (string.IsNullOrEmpty(s3Object.Key) || s3Object.Size == 0)
                    continue;

                var getResponse = await _s3Client.GetObjectAsync(_settings.BucketName, s3Object.Key);

                using var originalStream = getResponse.ResponseStream;
                var memStream = new MemoryStream();
                await originalStream.CopyToAsync(memStream);
                memStream.Position = 0;

                files.Add((s3Object.Key, memStream, getResponse.Headers.ContentType));

            }

            return files;
        }

        public async Task SaveMetadataJsonAsync(Guid questionId, List<FileTextExtractor.ExtractedFileText> extractedFiles)
        {
            var metadata = new Dictionary<string, object>();

            foreach (var file in extractedFiles)
            {
                if (file.Pages.Any())
                {
                    var fileName = Path.GetFileName(file.FileName); // Extrage doar numele
                    metadata[fileName] = file.Pages.ToDictionary(p => p.PageNumber.ToString(), p => p.Text);


                }
            }

            var json = JsonConvert.SerializeObject(metadata, Formatting.Indented);
            var metadataKey = $"attachments/{questionId}/metadata.json";

            var putRequest = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = metadataKey,
                ContentBody = json
            };

            await _s3Client.PutObjectAsync(putRequest);
        }

        public async Task<Dictionary<string, Dictionary<string, string>>?> GetMetadataForQuestionAsync(Guid questionId)
        {
            try
            {
                var response = await _s3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = $"attachments/{questionId}/metadata.json"
                });

                using var reader = new StreamReader(response.ResponseStream);
                var json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; // fallback pentru documentele vechi
            }
        }


    }
}
