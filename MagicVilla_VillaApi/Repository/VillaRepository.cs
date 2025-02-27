using System.Linq.Expressions;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Repository;

public class VillaRepository : IVillaRepository
{
    private readonly AppDbContext _context;

    public VillaRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Villa>> GetAllVillaAsync(Expression<Func<Villa, bool>> filter = null)
    {
        IQueryable<Villa> query = _context.Villas;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return await query.ToListAsync();
    }

    public async Task<Villa> GetVillaByIdAsync(Expression<Func<Villa, bool>> filter = null, bool tracked = true)
    {
        IQueryable<Villa> query = _context.Villas;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return await query.FirstOrDefaultAsync();
    }

    public async Task CreateVillaAsync(Villa villa)
    {
        await _context.Villas.AddAsync(villa);
        await SaveAsync();
    }

    public async Task UpdateVillaAsync(Villa villa)
    {
        _context.Villas.Update(villa);
        await SaveAsync();
    }

    public async Task DeleteVillaAsync(Villa villa)
    {
        _context.Villas.Remove(villa);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}