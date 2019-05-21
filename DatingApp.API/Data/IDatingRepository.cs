using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        //Instead of creating two methods one for Adding User one for Photo
        //we Create a Generic method(one method) specify T and save that resource in DBS 
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        //Saving changes to dbs returns true if savedAll
        Task<bool> saveAll();
        //Return users
        Task<IEnumerable<User>> getUsers();
        //getting individualUser from dbs
        Task<User> getUser(int id);
        Task<Photo> getPhoto(int id);
        Task<Photo> getMainPhotoForUser(int userId);

    }
}
