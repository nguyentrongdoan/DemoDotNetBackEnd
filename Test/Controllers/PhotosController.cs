﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        // private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repository,
            IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repository = repository;
            _mapper = mapper;
            var cloudinaryConfig1 = cloudinaryConfig;
            
            var account = new Account(
                cloudinaryConfig1.Value.CloudName,
                cloudinaryConfig1.Value.ApiKey,
                cloudinaryConfig1.Value.ApiSecret
                );
            
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repository.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }
        

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _repository.GetUser(userId);

            var file = photoForCreationDto.File;

            // var uploadResult = new ImageUploadResult();
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };
                uploadResult = _cloudinary.Upload(uploadParams);
                // _cloudinary.Upload(uploadParams);
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }
            userFromRepo.Photos.Add(photo);

            // if (await _repository.SaveAll())
            // {
            //     var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
            //     return CreatedAtRoute("GetPhoto", new {userId = userId, id = photo.Id}, photoToReturn);
            // }
            //
            // return BadRequest("Could not add the photo");

            if (!await _repository.SaveAll()) return BadRequest("Could not add the photo");
            var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
            return CreatedAtRoute("GetPhoto", new {userId, id = photo.Id}, photoToReturn);

        }
    }
}