using System.Linq.Expressions;
using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Tuyen;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories;

public class LeaderRepository
{
    private readonly EcommerceDbContext _context;
    private readonly IMapper _mapper;

    public LeaderRepository(IMapper mapper, EcommerceDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public void Add(LeaderCrudViewModel model)
    {
        var leader = _mapper.Map<Lead>(model);
        _context.Set<Lead>().Add(leader);
    }

    public void Update(LeaderCrudViewModel model)
    {
        var leader = _mapper.Map<Lead>(model);
        _context.Set<Lead>().Update(leader);
    }

    public void Delete(LeaderCrudViewModel model)
    {
        var leader = _mapper.Map<Lead>(model);
        _context.Set<Lead>().Remove(leader);
    }

    public LeaderCrudViewModel GetById(int id)
    {
        var leader = _context.Set<Lead>().Find(id);
        var data = _mapper.Map<LeaderCrudViewModel>(leader);

        return data;
    }

    public Lead FirstOrDefault(Expression<Func<Lead, bool>> predicate)
    {
        IQueryable<Lead> set = _context.Set<Lead>();
        return set.FirstOrDefault(predicate);
    }

    public IQueryable<Lead> BuildQuery(Expression<Func<Lead, bool>> predicate)
    {
        IQueryable<Lead> set = _context.Set<Lead>();
        return set.Where(predicate);
    }

    public IEnumerable<Lead> GetAll()
    {
        return _context.Set<Lead>().ToList();
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}