using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Test.Data;
using Test.Dtos;
using Test.Helpers;
using Test.Model;

namespace Test.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController: ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudingConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repository,
            IMapper mapper,
            IOptions<CloudinarySettings> cloudingConfig)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudingConfig = cloudingConfig;
            
            Account account = new Account(
                _cloudingConfig.Value.CloudName,
                _cloudingConfig.Value.ApiKey,
                _cloudingConfig.Value.ApiSecret
                );
            
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _repository.GetUser(userId);

            var file = photoForCreationDto.File;

            var imageUploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };
                _cloudinary.Upload(uploadParams);
            }

            photoForCreationDto.Url = imageUploadResult.Uri.ToString();
            photoForCreationDto.PublicId = imageUploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }
            userFromRepo.Photos.Add(photo);

            if (await _repository.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Could not add the photo");
        }
    }
}