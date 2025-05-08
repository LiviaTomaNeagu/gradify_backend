using Infrastructure.Base;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Files.Responses;
using MyBackedApi.Services;
using MyBackendApi.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace MyBackedApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/files")]
    public class FilesController : BaseApiController
    {
        private readonly S3Service _s3Service;
        private readonly UserService _userService;
        public FilesController(S3Service s3Service,
            UserService userService)
        {
            _s3Service = s3Service;
            _userService = userService;
        }


        [HttpPost("upload-avatar")]
        public async Task<UploadAvatarResponse> UploadAvatar(IFormFile file)
        {
            var currentUserId = GetUserIdFromToken();
            if (file == null || file.Length == 0)
                throw new WrongInputException("Empty files");

            var folder = "images";
            var fileName = currentUserId.ToString();
            var extension = Path.GetExtension(file.FileName).ToLower(); // păstrezi extensia
            var contentType = file.ContentType;

            using var inputStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(256, 256),
                Mode = ResizeMode.Max
            }));

            var width = image.Width;
            var height = image.Height;
            if (width != height)
            {
                var side = Math.Min(width, height);
                var cropX = (width - side) / 2;
                var cropY = (height - side) / 2;

                image.Mutate(x => x.Crop(new Rectangle(cropX, cropY, side, side)));
            }


            await using var outputStream = new MemoryStream();
            IImageEncoder encoder = extension switch
            {
                ".png" => new SixLabors.ImageSharp.Formats.Png.PngEncoder(),
                ".bmp" => new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder(),
                _ => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() // default JPEG
            };

            await image.SaveAsync(outputStream, encoder);
            outputStream.Position = 0;

            var url = await _s3Service.UploadFileAsync(outputStream, fileName, folder, contentType);
            await _userService.AddAvatarForUser(currentUserId, url);

            return new UploadAvatarResponse { AvatarUrl = url };
        }


        [HttpDelete("delete-avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            var currentUserId = GetUserIdFromToken();
            var folder = "images";

            var avatarExisted = await _userService.RemoveAvatarForUser(currentUserId);
            if (avatarExisted)
                await _s3Service.DeleteFileAsync(currentUserId.ToString(), folder);

            return Ok(new BaseResponseEmpty { Message = "File deleted successfully." });
        }

    }
}
