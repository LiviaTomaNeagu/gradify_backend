using Infrastructure.Base;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Files.Responses;
using MyBackedApi.Services;
using MyBackendApi.Services;

namespace MyBackedApi.Controllers
{

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

            using var stream = file.OpenReadStream();
            var fileName = currentUserId.ToString();
            var folder = "images";

            var url = await _s3Service.UploadFileAsync(stream, fileName, folder, file.ContentType);
            await _userService.AddAvatarForUser(currentUserId, url);

            return new UploadAvatarResponse
            { AvatarUrl = url };
        }

        [HttpDelete("delete-avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            var currentUserId = GetUserIdFromToken();
            var folder = "images";

            var avatarExisted = await _userService.RemoveAvatarForUser(currentUserId);
            if(avatarExisted)
                await _s3Service.DeleteFileAsync(currentUserId.ToString(), folder);

            return Ok(new BaseResponseEmpty { Message = "File deleted successfully." });
        }

    }
}
