using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class ProductRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(ProductCrudModel model)
        {
            var user = _mapper.Map<Product>(model);
            _context.Set<Product>().Add(user);
        }
        public void Update(ProductCrudModel model)
        {
            var user = _mapper.Map<Product>(model);
            _context.Set<Product>().Update(user);
        }
        public void Delete(ProductCrudModel model)
        {
            var user = _mapper.Map<Product>(model);
            _context.Set<Product>().Remove(user);
        }

        public ProductCrudModel GetById(int id)
        {
            var user = _context.Set<Product>().Find(id);
            var data = _mapper.Map<ProductCrudModel>(user);
            return data;
        }
        public Product FirstOrDefault(Expression<Func<Product, bool>> predicate)
        {
            IQueryable<Product> set = _context.Set<Product>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<Product> BuildQuery(Expression<Func<Product, bool>> predicate)
        {
            IQueryable<Product> set = _context.Set<Product>();
            return set.Where(predicate);
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Set<Product>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
