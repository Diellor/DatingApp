using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        //DateTime the photo was added
        public DateTime DateAdded { get; set; }
        public bool isMain { get; set; }

        //Now we have cascade delete,
        //If User will be deleted also the photos will be deleted
        public int UserId { get; set; }
        public User User { get; set; }

    }
}