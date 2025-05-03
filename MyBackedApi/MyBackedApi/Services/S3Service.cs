using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Infrastructure.Config.Models;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Options;
using MyBackedApi.DTOs.Forum;
using MyBackedApi.DTOs.Forum.Responses;

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


    }

}
