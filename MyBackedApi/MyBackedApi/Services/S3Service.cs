using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Infrastructure.Config.Models;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Options;

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

        public async Task<List<string>> AddMultipleFilesAsync(List<IFormFile> files, string folder = "attachments")
        {
            if (files == null || files.Count == 0)
                throw new WrongInputException("No files provided");

            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                    continue;

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
                var contentType = file.ContentType;

                await using var stream = file.OpenReadStream();
                var url = await UploadFileAsync(stream, fileName, folder, contentType);
                uploadedUrls.Add(url);
            }

            return uploadedUrls;
        }

    }

}
