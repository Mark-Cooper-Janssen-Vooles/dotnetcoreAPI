using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using dotnetCoreAPI.Data;
using dotnetCoreAPI.Models;
using dotnetCoreAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace dotnetCoreAPI.Repository
{
  public class UserRepository : IUserRepository
  {
    private readonly ApplicationDbContext _db;
    private readonly AppSettings _appSettings;
    public UserRepository(ApplicationDbContext db, IOptions<AppSettings> appSettings)
    {
      _db = db;
      _appSettings = appSettings.Value;
    }
    public User Authenticate(string username, string password)
    {
      //will need JWT token, if user is authenticated we want to pass it back to API call.
      //in order to do this we need appSettings secret key
      var user = _db.Users.SingleOrDefault(x => x.Username == username && x.Password == password);
      // if user not found
      if(user == null) 
      {
        return null;
      }

      //if user was found generate JWT token
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]{
          new Claim(ClaimTypes.Name, user.Id.ToString()),
          new Claim(ClaimTypes.Role, user.Role)
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);
      user.Token = tokenHandler.WriteToken(token);
      user.Password = "********"; //should hide the password or not return it at all!

      return user;
    }

    public bool IsUniqueUser(string username)
    {
      var user = _db.Users.SingleOrDefault(x => x.Username == username);

      //return null if user not found 
      if (user == null)
        return true;

      return false;
    }

    public User Register(string username, string password)
    {
      User userObj = new User()
      {
        Username = username,
        Password = password,
        Role = "Admin"
      };

      _db.Users.Add(userObj);
      _db.SaveChanges();
      userObj.Password = "********";
      return userObj;
    }
  }
}