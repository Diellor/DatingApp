using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.DTO
{
    public class PhotoForCreationDto
    {
        public string Url {get; set;}
        //this represents a file sent on hhtp request, We dont have IFormFile in our Photo.cs
        public IFormFile File {get; set;} //this is the photo that we are uploading
        public DateTime dateAdded {get;set;}

        //this is what we get back from Cloudinary
        public string PublicId {get; set;}
        public PhotoForCreationDto(){
            dateAdded = DateTime.Now;

        }
        

    }
}