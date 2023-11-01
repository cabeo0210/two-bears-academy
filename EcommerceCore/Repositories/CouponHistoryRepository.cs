using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Coupon;
using EcommerceCore.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class CouponRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CouponRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(CouponCrudModel model)
        {
            var order = _mapper.Map<Coupon>(model);
            _context.Set<Coupon>().Add(order);
        }
        public async Task AddAsync(Coupon model)
        {
            await _context.Set<Coupon>().AddAsync(model);
        }
        public void Update(CouponCrudModel model)
        {
            var order = _mapper.Map<Coupon>(model);
            _context.Set<Coupon>().Update(order);
        }
        public void Delete(CouponCrudModel model)
        {
            var order = _mapper.Map<Coupon>(model);
            _context.Set<Coupon>().Remove(order);
        }

        public CouponCrudModel GetById(int id)
        {
            var order = _context.Set<Coupon>().Find(id);
            var data = _mapper.Map<CouponCrudModel>(order);
            return data;
        }
        public Coupon FirstOrDefault(Expression<Func<Coupon, bool>> predicate)
        {
            IQueryable<Coupon> set = _context.Set<Coupon>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<Coupon> BuildQuery(Expression<Func<Coupon, bool>> predicate)
        {
            IQueryable<Coupon> set = _context.Set<Coupon>();
            return set.Where(predicate);
        }

        public IEnumerable<Coupon> GetAll()
        {
            return _context.Set<Coupon>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
