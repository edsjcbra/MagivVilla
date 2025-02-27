using System.Linq.Expressions;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.DTOs;

namespace MagicVilla_VillaApi.Repository.IRepository;

public interface IVillaRepository : IRepository<Villa>
{
    Task UpdateVillaAsync(Villa villa);
}