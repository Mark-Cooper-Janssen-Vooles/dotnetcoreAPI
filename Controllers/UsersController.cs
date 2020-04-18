using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using dotnetCoreAPI.Models;
using dotnetCoreAPI.Models.Dtos;
using dotnetCoreAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace dotnetCoreAPI.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/v{version:apiVersion}/users")]
  [ApiVersion("1.0")]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UsersController : ControllerBase
  {
      private readonly IUserRepository _userRepo;

      public UsersController(IUserRepository userRepo)
      {
          _userRepo = userRepo;
      }

      [AllowAnonymous] //without this gives error "bearer authentication required", because the whole controller is behind [Authorize] (up above) - we want to be able to call this without being authorised yet!
      [HttpPost("authenticate")]
      public IActionResult Authenticate([FromBody] AuthenticaionModel model)
      {
          var user = _userRepo.Authenticate(model.Username, model.Password);
          if (user == null)
          {
            return BadRequest(new {message = "Username or password is invalid"});
          }
          return Ok(user);
      }

      [AllowAnonymous]
      [HttpPost("register")]
      public IActionResult Register([FromBody] AuthenticaionModel model)
      {
          bool ifUserNameUnique = _userRepo.IsUniqueUser(model.Username);
          if(!ifUserNameUnique)
          {
            return BadRequest(new {message = "Username already exists"});
          }
          var user = _userRepo.Register(model.Username, model.Password);

          if(user == null)
          {
            return BadRequest(new {message = "Error while registering"});
          }

          return Ok();
      }
  }
}
