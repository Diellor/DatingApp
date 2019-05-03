using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
        Task<User> register(User user, string password); //Returns Task<UseR>
        Task<User> logIn(string username, string password);

        Task<bool> userExists(string username);
    }
}
