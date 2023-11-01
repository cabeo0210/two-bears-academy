using AutoMapper;
using DocumentFormat.OpenXml.InkML;
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
    public class CartItemRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CartItemRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(CartItemCrudModel model)
        {
            var cartItem = _mapper.Map<CartItem>(model);
            _context.Set<CartItem>().Add(cartItem);
        }
        public async Task AddAsync(CartItem model)
        {
            await _context.Set<CartItem>().AddAsync(model);
        }
        public void Update(CartItemCrudModel model)
        {
            var cartItem = _mapper.Map<CartItem>(model);
            _context.Set<CartItem>().Update(cartItem);
        }
        public void Delete(CartItemCrudModel model)
        {
            var cartItem = _mapper.Map<CartItem>(model);
            _context.Set<CartItem>().Remove(cartItem);
        }

        public CartViewModel GetById(int id)
        {
            var cartItem = _context.Set<CartItem>().Find(id);
            var data = _mapper.Map<CartViewModel>(cartItem);
            return data;
        }
        public CartItem FirstOrDefault(Expression<Func<CartItem, bool>> predicate)
        {
            IQueryable<CartItem> set = _context.Set<CartItem>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<CartItem> BuildQuery(Expression<Func<CartItem, bool>> predicate)
        {
            IQueryable<CartItem> set = _context.Set<CartItem>();
            return set.Where(predicate);
        }

        public IEnumerable<CartItem> GetAll()
        {
            return _context.Set<CartItem>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void DeleteById(int id)
        {
            var current = _context.Set<CartItem>().Find(id);
            _context.Set<CartItem>().Remove(current);
        }
    }
}
