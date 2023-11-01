using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Cart;
using EcommerceCore.ViewModel.Coupon;
using EcommerceCore.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class CartRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CartRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(CartCrudModel model)
        {
            var cart = _mapper.Map<Cart>(model);
            _context.Set<Cart>().Add(cart);
        }
        public async Task AddAsync(Cart model)
        {
            await _context.Set<Cart>().AddAsync(model);
        }
        public void Update(CartCrudModel model)
        {
            var cart = _mapper.Map<Cart>(model);
            _context.Set<Cart>().Update(cart);
        }
        public void Delete(CartCrudModel model)
        {
            var cart = _mapper.Map<Cart>(model);
            _context.Set<Cart>().Remove(cart);
        }

        public CartViewModel GetById(int id)
        {
            var cart = _context.Set<Cart>().Find(id);
            var data = _mapper.Map<CartViewModel>(cart);
            return data;
        }
        public Cart FirstOrDefault(Expression<Func<Cart, bool>> predicate)
        {
            IQueryable<Cart> set = _context.Set<Cart>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<Cart> BuildQuery(Expression<Func<Cart, bool>> predicate)
        {
            IQueryable<Cart> set = _context.Set<Cart>();
            return set.Where(predicate);
        }

        public IEnumerable<Cart> GetAll()
        {
            return _context.Set<Cart>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void DeleteById(int id)
        {
            var current = _context.Set<Cart>().Find(id);
            _context.Set<Cart>().Remove(current);
        }
    }
}
