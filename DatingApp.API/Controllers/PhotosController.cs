

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class PhotosController:ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private Cloudinary cloudinary;

        //Injecting cloudinary configuration, in order to access we use IOptions
        public PhotosController(IDatingRepository repo,IMapper mapper,IOptions<CloudinarySettings> cloudinaryConfig){
            this.repo = repo;
            this.mapper = mapper;
            this.cloudinaryConfig = cloudinaryConfig;
            //Setting up a new Account with values from our AppSettings (we are using our injectedService that we created to tie with the model CloudinarySe.cs)

            Account acc = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );
            //Now we create a new Instance of cloudinary and we will pass the account that we created
            cloudinary = new Cloudinary(acc);   //Now we use cloudinary for accessing methods of cloudinary
        }
     
        [HttpGet("{id}",Name="GetPhoto")]
        //Id of the photo we want to get, we provide name so we can give it to createdAtroute Method in httpPost cuz it needs a RouteName 
        public async Task<IActionResult> getPhoto(int id){
            //this will store the photoObject without UserDetails only photo
            var photoFromRepo = await repo.getPhoto(id); //await becouse it returns a task
            //we dont want to return everything thats inside this photoFromRepo object so we create anoather DTO
            var photo = mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }
        [HttpPost]
        //userId comes from route parameter
        public async Task<IActionResult> addPhotoForUser(int userId,[FromForm]PhotoForCreationDto photoForCreationDto){
            //We need to give .net core a hint [FromForm] we tell where this info is comming from,in this case 
            //we are using a from to give this info

            //we will send different attributes to api not the ones that are in photo.cs so we create a DTO
            //We passsing a file in our api so we need to specify that too
            //1. We match if userId from token is same as Id from route for auth.
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            //now we get the user
            var userFromRepo = await repo.getUser(userId);

            //File comes as parameter/ (as post request) from postman as input
            var file = photoForCreationDto.File; 
            
            //we store the result that we get from cloudinary
            var uploadResult= new ImageUploadResult();

            if(file.Length > 0){//check to see if there's something inside the file
                //we have the file WE WILL READ THE FILE INSIDE MEMORY
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name,stream),
                        //Transformation is for croping big photos if user uploads to only face squeare
                        //Transformation will crop the photo -in square
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    //the photo will be uploaded to clodenary and than will return the result
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }

            //After uploading now we get response from uploadresult
            //we store this url/location of photo to our database
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            //publicId - we use this to access our photo from cloudinary
            photoForCreationDto.PublicId = uploadResult.PublicId;

            //Map our DTO into our Photo.cs 
            var photo = mapper.Map<Photo>(photoForCreationDto);
            //if this is the first photo added we will assing this photo as main photo
            if(!userFromRepo.Photos.Any(u => u.isMain)){
                photo.isMain = true;
            }
            //saving ind dbset in database
            userFromRepo.Photos.Add(photo);

            //var photoToReturn = mapper.Map<PhotoForReturnDto>(photo);
            //this photo wont have the publicId until we save in our database so we move this when its already saved

            //if saving is successfull
            if(await repo.saveAll()){
                //we need to provide a route to get an individual photo
                //Param1.string routeName, the route that will allow us to return a single photo
                //Param2.object routeValue we need to pass the id of the photo that we want to return
                //Param3. value its goonna be a photo object that we creating, the PhotoForReturnDto model
                var photoToReturn = mapper.Map<PhotoForReturnDto>(photo); //here the (photo for sure has the id)
                //After we create our Route with a name (getrequest) we populate this method
                return CreatedAtRoute("GetPhoto",new {id = photo.Id},photoToReturn);
            }

            return BadRequest("Could not add the photo");

        }
        //we pass photo ID and use an emptyBody like setMain
        [HttpPost("{id}/setMain")]
        //the user ID is passed and PhotoId 
        public async Task<IActionResult> setMainPhoto(int userId,int id){
            //Check Authorization
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            //we need to check that user is updating his own photo

            var userFromRepo = await repo.getUser(userId);
            //Check if photo exists in photos of user 
            //nese Id qe po e pass in nuk e match asnje ID te photot athere unauthorized
            if(!userFromRepo.Photos.Any(p=>p.Id==id)){
                return Unauthorized();
            }
            var photoFromRepo = await repo.getPhoto(id);

            if(photoFromRepo.isMain)
            return BadRequest("This Is already a main photo");

            var currentMainPhoto = await repo.getMainPhotoForUser(userId);
            currentMainPhoto.isMain = false;
            //this is the photo that user set as main from clientside
            photoFromRepo.isMain = true;

            if(await repo.saveAll()){
                return NoContent();
            }
            return BadRequest("Could not set the photo to main");
        }
        //takes id of photo
        [HttpDelete("{id}")]
        //userId is from route
        public async Task<IActionResult> deletePhoto(int userId,int id){
            //Check Authorization
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var user = await repo.getUser(userId);
            //check if it has this photo that we want to delete
            if(!user.Photos.Any(p=>p.Id == id)){
                return Unauthorized();
            }
            //now we get that photo
            var photoFromRepo = await repo.getPhoto(id);
            //if it's the main photo we wont let them to delete
            if(photoFromRepo.isMain){
                return BadRequest("You cannot delete your main photo");
            }
            //We have our photo stored in Cloudinary and reference to that 
            //photo saved in database, so we need to delete both of them 
            //we have our publicId stored in DBS
            //we will use .detroy() method and pass the publicId to delete it from cloudinary
            //if succesfull we will back a OK result as string

            //WE NEEED TO CHECK FIRST IF PHOTO HAS PUBLIC ID, CUZ PHOTOS OF USER THAT WE
            //SEEDED WILL NOT HAVE PUBLICID AND THIS METHOD WILL NOT WORK SO WE NEED THAT CHECK
            if(photoFromRepo.publicId != null){
                var deleteParams = new DeletionParams(photoFromRepo.publicId);
                var result = cloudinary.Destroy(deleteParams);
                //we need to check if the resutl is OK
                if(result.Result == "ok"){
                repo.Delete(photoFromRepo);
                }
            }
            //this is for regular photos that we seeded they are not stored in cloudinary
            if(photoFromRepo.publicId == null){
                repo.Delete(photoFromRepo);
            }
            if(await repo.saveAll()){
                return Ok();
            }
            //if fails
            return BadRequest("Failed to delete the photo");
        }
    }
}