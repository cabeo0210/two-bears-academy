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
    public class ProductImageRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ProductImageRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(ProductImageCrudModel model)
        {
            var productImage = _mapper.Map<ProductImage>(model);
            _context.Set<ProductImage>().Add(productImage);
        }
        public void Update(ProductImageCrudModel model)
        {
            var productImage = _mapper.Map<ProductImage>(model);
            _context.Set<ProductImage>().Update(productImage);
        }
        public void Delete(ProductImageCrudModel model)
        {
            var productImage = _mapper.Map<ProductImage>(model);
            _context.Set<ProductImage>().Remove(productImage);
        }

        public ProductImageCrudModel GetById(int id)
        {
            var productImage = _context.Set<ProductImage>().Find(id);
            var data = _mapper.Map<ProductImageCrudModel>(productImage);
            return data;
        }
        public ProductImage FirstOrDefault(Expression<Func<ProductImage, bool>> predicate)
        {
            IQueryable<ProductImage> set = _context.Set<ProductImage>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<ProductImage> BuildQuery(Expression<Func<ProductImage, bool>> predicate)
        {
            IQueryable<ProductImage> set = _context.Set<ProductImage>();
            return set.Where(predicate);
        }

        public IEnumerable<ProductImage> GetAll()
        {
            return _context.Set<ProductImage>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
