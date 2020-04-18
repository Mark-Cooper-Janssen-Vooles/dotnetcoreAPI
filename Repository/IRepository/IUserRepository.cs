using System;
using System.Collections.Generic;
using dotnetCoreAPI.Models;

namespace dotnetCoreAPI.Repository.IRepository 
{
  public interface IUserRepository
  {
    bool IsUniqueUser(string username);
    User Authenticate(string username, string password);
    User Register(string username, string password);
  }
}