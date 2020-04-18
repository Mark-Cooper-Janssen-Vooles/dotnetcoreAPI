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
  [ApiController]
  // [Route("api/trails")]
  [Route("api/v{version:apiVersion}/trails")]
  [ApiVersion("1.0")]
  // [ApiExplorerSettings(GroupName = "ParkyOpenAPISpecT")]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
  {
    private readonly ITrailRepository _trailRepository;
    private readonly IMapper _mapper;

    public TrailsController(ITrailRepository trailRepositoy, IMapper mapper)
    {
        _trailRepository = trailRepositoy;
        _mapper = mapper;
    }

    /// <summary>
    /// Get list of trails.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
    public IActionResult GetTrails()
    {
      var objList = _trailRepository.GetTrails();
      var objDto = new List<TrailDto>();

      foreach(var obj in objList) {
        objDto.Add(_mapper.Map<TrailDto>(obj));
      }

      return Ok(objDto);
    }

    /// <summary>
    /// Get individual trail
    /// </summary>
    /// <param name="trailId"> The Id of the trail </param>
    /// <returns></returns>
    [HttpGet("{id:int}", Name = "GetTrail")]
    [ProducesResponseType(200, Type = typeof(TrailDto))]
    [ProducesResponseType(404)]
    [ProducesDefaultResponseType]
    [Authorize(Roles = "Admin")]
    public IActionResult GetTrail(int trailId)
    {
      var obj = _trailRepository.GetTrail(trailId);
      if (obj == null)
      {
        return NotFound();
      }

      var objDto = _mapper.Map<TrailDto>(obj);
      return Ok(objDto);
    }

    [HttpGet("[action]/{NationalParkId:int}", Name = "GetTrailsInNationalPark")]
    [ProducesResponseType(200, Type = typeof(TrailDto))]
    [ProducesResponseType(404)]
    [ProducesDefaultResponseType]
    public IActionResult GetTraiInNationalPark(int nationalParkId)
    {
      var objList = _trailRepository.GetTrailsInNationalPark(nationalParkId);
      if (objList == null)
      {
        return NotFound();
      }
      var objDto = new List<TrailDto>();
      foreach(var obj in objList)
      {
        objDto.Add(_mapper.Map<TrailDto>(obj));
      }

      return Ok(objDto);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(TrailDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(500)]
    public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
    {
      if (trailDto == null)
      {
        return BadRequest(ModelState);
      } 

      if(_trailRepository.TrailExists(trailDto.Name))
      {
        ModelState.AddModelError("", "Trail Exists!");
        return StatusCode(404, ModelState);
      }

      var trailObj = _mapper.Map<Trail>(trailDto);

      if(!_trailRepository.CreateTrail(trailObj))
      {
        ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");
        return StatusCode(500, ModelState);
      }

      //createdAtRoute takes route name, route value, final values. Returns a 201 Created
      return CreatedAtRoute("GetTrail", new { id = trailObj.Id}, trailObj);
    }

    [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailDto)
    {
      if (trailDto == null || trailId != trailDto.Id)
      {
        return BadRequest(ModelState);
      } 

      var trailObj = _mapper.Map<Trail>(trailDto);

      if(!_trailRepository.UpdateTrail(trailObj))
      {
        ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
        return StatusCode(500, ModelState);
      }
      
      return NoContent(); //patch request usually returns no content
    }

    [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteTrail(int trailId)
    {
      if (!_trailRepository.TrailExists(trailId))
      {
        return NotFound();
      } 

      var trailObj = _trailRepository.GetTrail(trailId);

      if(!_trailRepository.DeleteTrail(trailObj))
      {
        ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
        return StatusCode(500, ModelState);
      }
      
      return NoContent(); //delete request usually returns no content
    }
  }
}
