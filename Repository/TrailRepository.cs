using System.Collections.Generic;
using System.Linq;
using dotnetCoreAPI.Data;
using dotnetCoreAPI.Models;
using dotnetCoreAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace dotnetCoreAPI.Repository
{
  public class TrailRepository : ITrailRepository
  {
    private readonly ApplicationDbContext _db;

    public TrailRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public bool CreateTrail(Trail trail)
    {
      _db.Trails.Add(trail); //.Add is a default you get on the entity framework, we have NationalParks because it was added in ApplicationDbContext.cs
      return Save();
    }

    public bool DeleteTrail(Trail trail)
    {
      _db.Trails.Remove(trail);
      return Save();
    }

    public Trail GetTrail(int trailId)
    {
      return _db.Trails.Include(c=>c.NationalPark).FirstOrDefault(a=>a.Id == trailId);
    }

    public ICollection<Trail> GetTrails()
    {
      return _db.Trails.Include(c=>c.NationalPark).OrderBy(a=>a.Name).ToList();
    }

    public bool TrailExists(string name)
    {
      bool value = _db.Trails.Any(a=>a.Name.ToLower().Trim() == name.ToLower().Trim());
      return value;
    }

    public bool TrailExists(int id)
    {
      return _db.Trails.Any(a=>a.Id == id);
    }

    public bool Save()
    {
      return _db.SaveChanges() >= 0 ? true : false;
    }

    public bool UpdateTrail(Trail trail)
    {
      _db.Trails.Update(trail);
      return Save();
    }

    public ICollection<Trail> GetTrailsInNationalPark(int nationalParkId)
    {
      return _db.Trails.Include(c=>c.NationalPark).Where(c=>c.NationalParkId == nationalParkId).ToList();
    }
  }
}