using System.ComponentModel.DataAnnotations;

namespace dotnetCoreAPI.Models
{
  public class AuthenticaionModel
  {
    [Required]
    public string Username {get; set;}
    [Required]
    public string Password {get; set;}
  }
}