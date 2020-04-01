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
  public class NationalParksController : ControllerBase
  {
    private readonly INationalParkRepository _npRepository;
    private readonly IMapper _mapper;

    public NationalParksController(INationalParkRepository npRepositoy, IMapper mapper)
    {
        _npRepository = npRepositoy;
        _mapper = mapper;
    }

    /// <summary>
    /// Get list of national parks.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetNationalParks()
    {
      var objList = _npRepository.GetNationalParks();
      var objDto = new List<NationalParkDto>();

      foreach(var obj in objList) {
        objDto.Add(_mapper.Map<NationalParkDto>(obj));
      }

      return Ok(objDto);
    }

    /// <summary>
    /// Get individual national park
    /// </summary>
    /// <param name="nationalParkId"> The Id of the national Park </param>
    /// <returns></returns>
    [HttpGet("{id:int}", Name = "GetNationalPark")]
    public IActionResult GetNationalPark(int nationalParkId)
    {
      var obj = _npRepository.GetNationalPark(nationalParkId);
      if (obj == null)
      {
        return NotFound();
      }

      var objDto = _mapper.Map<NationalParkDto>(obj);
      return Ok(objDto);
    }

    [HttpPost]
    public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
    {
      if (nationalParkDto == null)
      {
        return BadRequest(ModelState);
      } 

      if(_npRepository.NationalParkExists(nationalParkDto.Name))
      {
        ModelState.AddModelError("", "National Park Exists!");
        return StatusCode(404, ModelState);
      }

      var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

      if(!_npRepository.CreateNationalPark(nationalParkObj))
      {
        ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
        return StatusCode(500, ModelState);
      }

      //createdAtRoute takes route name, route value, final values. Returns a 201 Created
      return CreatedAtRoute("GetNationalPark", new { id = nationalParkObj.Id}, nationalParkObj);
    }

    [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
    public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
    {
      if (nationalParkDto == null || nationalParkId != nationalParkDto.Id)
      {
        return BadRequest(ModelState);
      } 

      var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

      if(!_npRepository.UpdateNationalPark(nationalParkObj))
      {
        ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkObj.Name}");
        return StatusCode(500, ModelState);
      }
      
      return NoContent(); //patch request usually returns no content
    }

    [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
    public IActionResult DeleteNationalPark(int nationalParkId)
    {
      if (!_npRepository.NationalParkExists(nationalParkId))
      {
        return NotFound();
      } 

      var nationalParkObj = _npRepository.GetNationalPark(nationalParkId);

      if(!_npRepository.DeleteNationalPark(nationalParkObj))
      {
        ModelState.AddModelError("", $"Something went wrong when deleting the record {nationalParkObj.Name}");
        return StatusCode(500, ModelState);
      }
      
      return NoContent(); //delete request usually returns no content
    }
  }
}