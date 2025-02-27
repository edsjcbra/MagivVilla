using System.Linq.Expressions;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.DTOs;

namespace MagicVilla_VillaApi.Repository.IRepository;

public interface IVillaRepository
{
    Task<List<Villa>> GetAllVillaAsync(Expression<Func<Villa, bool>> filter = null);
    Task<Villa> GetVillaByIdAsync(Expression<Func<Villa, bool>> filter = null, bool tracked = true);
    Task CreateVillaAsync(Villa villa);
    Task UpdateVillaAsync(Villa villa);
    Task DeleteVillaAsync(Villa villa);
    Task SaveAsync();
}