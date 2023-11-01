using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Setting;
using EcommerceCore.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class SettingRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public SettingRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(SettingCrudModel model)
        {
            var setting = _mapper.Map<Setting>(model);
            _context.Set<Setting>().Add(setting);
        }
        public async Task AddAsync(Setting model)
        {
            await _context.Set<Setting>().AddAsync(model);
        }
        public void Update(SettingCrudModel model)
        {
            var setting = _mapper.Map<Setting>(model);
            _context.Set<Setting>().Update(setting);
        }
        public void Delete(SettingCrudModel model)
        {
            var setting = _mapper.Map<Setting>(model);
            _context.Set<Setting>().Remove(setting);
        }

        public SettingCrudModel GetById(int id)
        {
            var setting = _context.Set<Setting>().Find(id);
            var data = _mapper.Map<SettingCrudModel>(setting);
            return data;
        }
        public Setting FirstOrDefault(Expression<Func<Setting, bool>> predicate)
        {
            IQueryable<Setting> set = _context.Set<Setting>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<Setting> BuildQuery(Expression<Func<Setting, bool>> predicate)
        {
            IQueryable<Setting> set = _context.Set<Setting>();
            return set.Where(predicate);
        }

        public IEnumerable<Setting> GetAll()
        {
            return _context.Set<Setting>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
