using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext context;
        public DatingRepository(DataContext context)
        {
            this.context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            //Entity can be any type of Class (in our case User or Photo
            //The add is saved in memory not with database so we dont need async await
            context.Add(entity);
            
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
           
        }

        public async Task<User> getUser(int id)
        {
            //We want to return users with photos too so we use Include, photo is navigatable so we need Inculde
           var user = await context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user; //if there is no user it returns null
        }

        public async Task<IEnumerable<User>> getUsers()
        {
            //The Collection in User model can be accessed with Include method
            //when we return user we want to display a profile pic to so we include photos of that user
            var users = await context.Users.Include(p => p.Photos).ToListAsync();
            return users;
        }

        public async Task<bool> saveAll()
        {
            //if its equal to 0 than nothing is saved to database so we return false (no changes) else return true
            return await context.SaveChangesAsync() > 0;
        }
    }
}
