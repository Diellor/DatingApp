using System;

namespace DatingApp.API.DTO
{
    public class PhotoForReturnDto
    {
        
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        //DateTime the photo was added
        public DateTime DateAdded { get; set; }
        public bool isMain { get; set; }
        //we get this publicid from cloudinary
        public string PublicId {get;set;}
    }
}