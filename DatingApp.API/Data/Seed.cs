using DatingApp.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private DataContext context;

        public Seed(DataContext context)
        {
            this.context = context;
        }
        
        public void SeedUsers()
        {
            //we want to read all userData 
            //This method reads all file and exits
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json"); 
            //all text is stored in userData, Now we need to serialize to Object and loop over them to save in database

            //Deserializes Json(string) to .Net object
            var users  = JsonConvert.DeserializeObject<List<User>>(userData); //now we have a list of user Objects
            foreach(var user in users)
            {
                //Now we need to hash the password that we get from user
                byte[] passwordHash;
                byte[] passwordSalt;

                //creating the password hash for entered password
                createPasswordHash("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                user.Username = user.Username.ToLower(); //convert usernames to lowercaase
                context.Add(user);
            }
            context.SaveChanges();
            
        }

        private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //computes  hashbased message (random generated key)
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                //anything here will be desposed after we use it
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }
    }
}
