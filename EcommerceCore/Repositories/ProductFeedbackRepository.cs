using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.ProductFeedback;
using EcommerceCore.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class ProductFeedbackRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ProductFeedbackRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(ProductFeedbackCrudModel model)
        {
            var productFeedback = _mapper.Map<ProductFeedback>(model);
            _context.Set<ProductFeedback>().Add(productFeedback);
        }
        public async Task AddAsync(ProductFeedback model)
        {
            await _context.Set<ProductFeedback>().AddAsync(model);
        }
        public void Update(ProductFeedbackCrudModel model)
        {
            var productFeedback = _mapper.Map<ProductFeedback>(model);
            _context.Set<ProductFeedback>().Update(productFeedback);
        }
        public void Delete(ProductFeedbackCrudModel model)
        {
            var productFeedback = _mapper.Map<ProductFeedback>(model);
            _context.Set<ProductFeedback>().Remove(productFeedback);
        }

        public ProductFeedbackCrudModel GetById(int id)
        {
            var productFeedback = _context.Set<ProductFeedback>().Find(id);
            var data = _mapper.Map<ProductFeedbackCrudModel>(productFeedback);
            return data;
        }
        public ProductFeedback FirstOrDefault(Expression<Func<ProductFeedback, bool>> predicate)
        {
            IQueryable<ProductFeedback> set = _context.Set<ProductFeedback>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<ProductFeedback> BuildQuery(Expression<Func<ProductFeedback, bool>> predicate)
        {
            IQueryable<ProductFeedback> set = _context.Set<ProductFeedback>();
            return set.Where(predicate);
        }

        public IEnumerable<ProductFeedback> GetAll()
        {
            return _context.Set<ProductFeedback>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
