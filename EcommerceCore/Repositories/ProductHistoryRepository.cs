using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.ProductHistory;
using EcommerceCore.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class ProductHistoryRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ProductHistoryRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(ProductHistoryCrudModel model)
        {
            var producthistory = _mapper.Map<ProductHistory>(model);
            _context.Set<ProductHistory>().Add(producthistory);
        }
        public async Task AddAsync(ProductHistory model)
        {
            await _context.Set<ProductHistory>().AddAsync(model);
        }
        public void Update(ProductHistoryCrudModel model)
        {
            var producthistory = _mapper.Map<ProductHistory>(model);
            _context.Set<ProductHistory>().Update(producthistory);
        }
        public void Delete(ProductHistoryCrudModel model)
        {
            var producthistory = _mapper.Map<ProductHistory>(model);
            _context.Set<ProductHistory>().Remove(producthistory);
        }

        public ProductHistoryCrudModel GetById(int id)
        {
            var producthistory = _context.Set<ProductHistory>().Find(id);
            var data = _mapper.Map<ProductHistoryCrudModel>(producthistory);
            return data;
        }
        public ProductHistory FirstOrDefault(Expression<Func<ProductHistory, bool>> predicate)
        {
            IQueryable<ProductHistory> set = _context.Set<ProductHistory>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<ProductHistory> BuildQuery(Expression<Func<ProductHistory, bool>> predicate)
        {
            IQueryable<ProductHistory> set = _context.Set<ProductHistory>();
            return set.Where(predicate);
        }

        public IEnumerable<ProductHistory> GetAll()
        {
            return _context.Set<ProductHistory>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
