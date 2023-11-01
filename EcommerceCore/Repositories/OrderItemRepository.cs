using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Order;
using EcommerceCore.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class OrderItemRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public OrderItemRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(OrderItemCrudModel model)
        {
            var orderItem = _mapper.Map<OrderItem>(model);
            _context.Set<OrderItem>().Add(orderItem);
        }
        public async Task AddAsync(OrderItem model)
        {
            await _context.Set<OrderItem>().AddAsync(model);
        }
        public void Update(OrderItemCrudModel model)
        {
            var orderItem = _mapper.Map<OrderItem>(model);
            _context.Set<OrderItem>().Update(orderItem);
        }
        public void Delete(OrderItemCrudModel model)
        {
            var orderItem = _mapper.Map<OrderItem>(model);
            _context.Set<OrderItem>().Remove(orderItem);
        }

        public OrderItemCrudModel GetById(int id)
        {
            var orderItem = _context.Set<OrderItem>().Find(id);
            var data = _mapper.Map<OrderItemCrudModel>(orderItem);
            return data;
        }
        public OrderItem FirstOrDefault(Expression<Func<OrderItem, bool>> predicate)
        {
            IQueryable<OrderItem> set = _context.Set<OrderItem>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<OrderItem> BuildQuery(Expression<Func<OrderItem, bool>> predicate)
        {
            IQueryable<OrderItem> set = _context.Set<OrderItem>();
            return set.Where(predicate);
        }

        public IEnumerable<OrderItem> GetAll()
        {
            return _context.Set<OrderItem>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
