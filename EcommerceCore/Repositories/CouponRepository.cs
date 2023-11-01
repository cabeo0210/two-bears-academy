using AutoMapper;
using EcommerceCore.Models;
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
    public class CouponHistoryRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CouponHistoryRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(CouponHistoryCrudModel model)
        {
            var coupon = _mapper.Map<CouponHistory>(model);
            _context.Set<CouponHistory>().Add(coupon);
        }
        public async Task AddAsync(CouponHistory model)
        {
            await _context.Set<CouponHistory>().AddAsync(model);
        }
        public void Update(CouponHistoryCrudModel model)
        {
            var coupon = _mapper.Map<CouponHistory>(model);
            _context.Set<CouponHistory>().Update(coupon);
        }
        public void Delete(CouponHistoryCrudModel model)
        {
            var coupon = _mapper.Map<CouponHistory>(model);
            _context.Set<CouponHistory>().Remove(coupon);
        }

        public CouponHistoryCrudModel GetById(int id)
        {
            var coupon = _context.Set<CouponHistory>().Find(id);
            var data = _mapper.Map<CouponHistoryCrudModel>(coupon);
            return data;
        }
        public CouponHistory FirstOrDefault(Expression<Func<CouponHistory, bool>> predicate)
        {
            IQueryable<CouponHistory> set = _context.Set<CouponHistory>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<CouponHistory> BuildQuery(Expression<Func<CouponHistory, bool>> predicate)
        {
            IQueryable<CouponHistory> set = _context.Set<CouponHistory>();
            return set.Where(predicate);
        }

        public IEnumerable<CouponHistory> GetAll()
        {
            return _context.Set<CouponHistory>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void DeleteById(int id)
        {
            var current = _context.Set<CouponHistory>().Find(id);
            _context.Set<CouponHistory>().Remove(current);
        }
    }
}
