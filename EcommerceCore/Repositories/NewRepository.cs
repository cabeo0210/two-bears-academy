using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.New;
using EcommerceCore.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class NewRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public NewRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(NewCrudModel model)
        {
            var news = _mapper.Map<New>(model);
            _context.Set<New>().Add(news);
        }
        public async Task AddAsync(New model)
        {
            await _context.Set<New>().AddAsync(model);
        }
        public void Update(NewCrudModel model)
        {
            var news = _mapper.Map<New>(model);
            _context.Set<New>().Update(news);
        }
        public void Delete(NewCrudModel model)
        {
            var news = _mapper.Map<New>(model);
            _context.Set<New>().Remove(news);
        }

        public NewCrudModel GetById(int id)
        {
            var news = _context.Set<New>().Find(id);
            var data = _mapper.Map<NewCrudModel>(news);
            return data;
        }
        public New FirstOrDefault(Expression<Func<New, bool>> predicate)
        {
            IQueryable<New> set = _context.Set<New>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<New> BuildQuery(Expression<Func<New, bool>> predicate)
        {
            IQueryable<New> set = _context.Set<New>();
            return set.Where(predicate);
        }

        public IEnumerable<New> GetAll()
        {
            return _context.Set<New>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
