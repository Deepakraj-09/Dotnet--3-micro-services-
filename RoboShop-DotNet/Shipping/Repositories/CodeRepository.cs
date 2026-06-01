using Microsoft.EntityFrameworkCore;
using RoboShop.Shipping.Infrastructure;
using RoboShop.Shipping.Models;

namespace RoboShop.Shipping.Repositories;

/// <summary>
/// Code data access — converted from Java CodeRepository (PagingAndSortingRepository).
/// Sorted findAll() is replaced with OrderBy in LINQ.
/// </summary>
public class CodeRepository
{
    private readonly ShippingDbContext _db;

    public CodeRepository(ShippingDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Replaces coderepo.findAll(Sort.by(Sort.Direction.ASC, "name"))
    /// </summary>
    public async Task<List<Code>> FindAllOrderedByNameAsync() =>
        await _db.Codes
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<Code?> FindByIdAsync(long id) =>
        await _db.Codes.FindAsync(id);
}
