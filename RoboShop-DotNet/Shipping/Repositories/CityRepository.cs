using Microsoft.EntityFrameworkCore;
using RoboShop.Shipping.Infrastructure;
using RoboShop.Shipping.Models;

namespace RoboShop.Shipping.Repositories;

/// <summary>
/// City data access — converted from Java CityRepository (Spring Data CrudRepository).
/// JPQL @Query is replaced with LINQ in EF Core.
/// </summary>
public class CityRepository
{
    private readonly ShippingDbContext _db;

    public CityRepository(ShippingDbContext db)
    {
        _db = db;
    }

    public async Task<long> CountAsync() =>
        await _db.Cities.LongCountAsync();

    public async Task<List<City>> FindByCodeAsync(string code) =>
        await _db.Cities
            .Where(c => c.Code == code)
            .ToListAsync();

    /// <summary>
    /// Replaces the JPQL: "select c from City c where c.code = ?1 and c.city like ?2%"
    /// </summary>
    public async Task<List<City>> MatchAsync(string code, string text) =>
        await _db.Cities
            .Where(c => c.Code == code && EF.Functions.Like(c.CityName, $"{text}%"))
            .ToListAsync();

    public async Task<City?> FindByIdAsync(long id) =>
        await _db.Cities.FindAsync(id);
}
