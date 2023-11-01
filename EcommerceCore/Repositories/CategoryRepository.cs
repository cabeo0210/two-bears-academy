using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Category;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class CategoryRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CategoryRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(CategoryCrudModel model)
        {
            var user = _mapper.Map<Category>(model);
            _context.Set<Category>().Add(user);
        }
        public void Update(CategoryCrudModel model)
        {
            var user = _mapper.Map<Category>(model);
            _context.Set<Category>().Update(user);
        }
        public void Delete(CategoryCrudModel model)
        {
            var user = _mapper.Map<Category>(model);
            _context.Set<Category>().Remove(user);
        }

        public CategoryCrudModel GetById(int id)
        {
            var user = _context.Set<Category>().Find(id);
            var data = _mapper.Map<CategoryCrudModel>(user);
            return data;
        }
        public Category FirstOrDefault(Expression<Func<Category, bool>> predicate)
        {
            IQueryable<Category> set = _context.Set<Category>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<Category> BuildQuery(Expression<Func<Category, bool>> predicate)
        {
            IQueryable<Category> set = _context.Set<Category>();
            return set.Where(predicate);
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Set<Category>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
