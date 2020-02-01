using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DateApp.API.Helpers;
using DateApp.Core.Contracts.DAL;
using DateApp.Core.DataModels;
using DateApp.Core.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IPhotoRepository _photoRepo;
        private readonly IMapper _mapper;
        private readonly CloudinarySettings _cloudinaryConfig;
        private CloudinaryDotNet.Cloudinary _cloudinary;

        public PhotosController(IUserRepository repository, IPhotoRepository photoRepo, IMapper mapper, CloudinarySettings cloudinaryConfig)
        {
            _repository = repository;
            _photoRepo = photoRepo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account
            {
                ApiKey = _cloudinaryConfig.ApiKey,
                ApiSecret = _cloudinaryConfig.ApiSecret,
                Cloud = _cloudinaryConfig.CloudName
            };

            _cloudinary = new CloudinaryDotNet.Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _photoRepo.GetPhoto(id);

            var photoToReturn = _mapper.Map<UserPhotoDm>(photoFromRepo);

            return Ok(photoToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]UserPhotoDm photos)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repository.GetUser(userId);
            var file = photos.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photos.url = uploadResult.Uri.ToString();
            photos.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photos);

            if (!userFromRepo.Photos.Any(x => x.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if (await _photoRepo.SaveAll())
            {
                var photoToReturn = _mapper.Map<UserPhotoDm>(photo);
                return Ok(photoToReturn);
            }

            return BadRequest("Could not add the photo.");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repository.GetUser(userId);
            if (user.Photos.FirstOrDefault(x => x.Id == id) == null)
            {
                return Unauthorized();
            }

            var photoFromRepo = await _photoRepo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("This photo is already Main Photo!");

            var currentMainPhoto = await _photoRepo.GetMainPhoto(userId);
            if (currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
                photoFromRepo.IsMain = true;
            }

            if (await _photoRepo.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could not set photo to main.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repository.GetUser(userId);
            if (user.Photos.FirstOrDefault(x => x.Id == id) == null)
            {
                return Unauthorized();
            }

            var photoFromRepo = await _photoRepo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("Main photo cannot be deleted!");
            if (photoFromRepo.PublicId != null)
            {
                var result = _cloudinary.Destroy(new DeletionParams(photoFromRepo.PublicId));
                if (result.Result.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
                {
                    _photoRepo.Delete(photoFromRepo);
                }

            }
            else
            {
                _photoRepo.Delete(photoFromRepo);
            }

            if (await _photoRepo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}