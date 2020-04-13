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

namespace dotnetCoreAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  // [ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
  [ProducesResponseType(StatusCodes.Status400BadRequest)] //since 404 is generic accross all routes, can add this to the top before the class!
  public class NationalParksV2Controller : ControllerBase
  {
    private readonly INationalParkRepository _npRepository;
    private readonly IMapper _mapper;

    public NationalParksV2Controller(INationalParkRepository npRepositoy, IMapper mapper)
    {
        _npRepository = npRepositoy;
        _mapper = mapper;
    }

    /// <summary>
    /// Get list of national parks.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
    public IActionResult GetNationalParks()
    {
      var objList = _npRepository.GetNationalParks();
      var objDto = new List<NationalParkDto>();

      foreach(var obj in objList) {
        objDto.Add(_mapper.Map<NationalParkDto>(obj));
      }

      return Ok(objDto);
    }
  }
}