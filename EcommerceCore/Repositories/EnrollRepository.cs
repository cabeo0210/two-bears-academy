using System.Linq.Expressions;
using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Tuyen;
using EcommerceCore.ViewModel.TuyenSinh;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories;

public class EnrollRepository
{
    private readonly EcommerceDbContext _context;
    private readonly IMapper _mapper;

    public EnrollRepository(IMapper mapper, EcommerceDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }
    
    public void Add(EnrollViewModel model)
    {
        var enroll = _mapper.Map<Enroll>(model);
        _context.Set<Enroll>().Add(enroll);
    }
    public void AddHistoryEnroll(HistoryEnroll model)
    {
        _context.Set<HistoryEnroll>().Add(model);
    }
    public void UpdateHistoryEnroll(HistoryEnroll model)
    {
        _context.Set<HistoryEnroll>().Update(model);
    }
    public HistoryEnroll GetHisEnrollById(int id)
    {
        var enroll = _context.Set<HistoryEnroll>().Find(id);
        return enroll;
    }
    public void Update(EnrollViewModel model)
    {
        var enroll = _mapper.Map<Enroll>(model);
        _context.Set<Enroll>().Update(enroll);
    }
    public void Delete(EnrollViewModel model)
    {
        var enroll = _mapper.Map<Enroll>(model);
        _context.Set<Enroll>().Remove(enroll);
    }

    public EnrollViewModel GetById(int id)
    {
        var enroll = _context.Set<Enroll>().Find(id);
        var data = _mapper.Map<EnrollViewModel>(enroll);
        
        return data;
    }
    public Enroll FirstOrDefault(Expression<Func<Enroll, bool>> predicate)
    {
        IQueryable<Enroll> set = _context.Set<Enroll>();
        return set.FirstOrDefault(predicate);
    }

    public IQueryable<Enroll> Where(Expression<Func<Enroll, bool>> predicate)
    {
        IQueryable<Enroll> set = _context.Set<Enroll>();
        return set.Where(predicate);
    }

    public IEnumerable<Enroll> GetAll()
    {
        return _context.Set<Enroll>().Include(p=>p.HistoryEnrolls).ToList();
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public IQueryable<Enroll> BuildQuery(Expression<Func<Enroll, bool>> predicate)
    {
        IQueryable<Enroll> set = _context.Set<Enroll>();
        return set.Include(p=>p.HistoryEnrolls).Where(predicate);
    }
}