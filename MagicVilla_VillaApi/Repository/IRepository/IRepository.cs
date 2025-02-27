﻿using System.Linq.Expressions;

namespace MagicVilla_VillaApi.Repository.IRepository;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task<T> GetByIdAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);
    Task CreateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveAsync();
}