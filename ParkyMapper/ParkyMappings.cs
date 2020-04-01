using AutoMapper;
using dotnetCoreAPI.Models;
using dotnetCoreAPI.Models.Dtos;

namespace dotnetCoreAPI.ParkyMapper
{
  public class ParkyMappings : Profile
  {
    public ParkyMappings()
    {
        CreateMap<NationalPark, NationalParkDto>().ReverseMap();
    }
  }
}