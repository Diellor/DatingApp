using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository //used to query our database with ef 
    {
        DataContext context;
        public AuthRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<User> logIn(string username, string password)
        {
          
            //we use username to identify User in our Database
            //we need to compute the hash of this pass and compare it with the one in database

            var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
                return null; //if null user will not be authroitized

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)){
                return null;
            } //if match password return true

            return user;
           
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //we pas passwordSalt(as key in method) so i'll identify than the hash and if generated hash matches with password than its true
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                //it will compute a hash based on password but itll use the key also
                //computedHash is array so we compare with this bytearr with every element in passwordHashArray
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false; //passwords do not match
                }

            }
            return true; //paswords match
        }
        public async Task<User> register(User user, string password) //psw = diellor (si input)
        {
            byte[] passwordHash;
            byte[] passwordSalt;
            //the method creates/populates these bytearr
            createPasswordHash(password, out passwordHash, out passwordSalt); //we want to pass by reference so we use out

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await  context.AddAsync(user);
            await  context.SaveChangesAsync();

            return user;
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
        public async Task<bool> userExists(string username)
        {
            //compare this username with any username in database
            if(await context.Users.AnyAsync(x=>x.Username.Equals(username)))
                return true;
            return false;
        }
    }
}
