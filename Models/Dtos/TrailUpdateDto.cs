using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static dotnetCoreAPI.Models.Trail;

//DTO used for creating and updating => i.e. theres no "public NationalParkDto NationalPark {get; set;}"
namespace dotnetCoreAPI.Models.Dtos
{
  public class TrailUpdateDto
  {
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public double Distance { get; set; }
    public DifficultyType Difficulty { get; set; }
    [Required]
    public int NationalParkId { get; set; }
  }
}