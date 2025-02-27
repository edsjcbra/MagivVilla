using System.Linq.Expressions;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Repository;

public class VillaRepository : Repository<Villa>, IVillaRepository
{
    private readonly AppDbContext _context;

    public VillaRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task UpdateVillaAsync(Villa villa)
    {
        _context.Villas.Update(villa);
        await _context.SaveChangesAsync();
    }
}