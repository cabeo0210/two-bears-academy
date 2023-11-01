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
    public class OrderRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public OrderRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(OrderCrudModel model)
        {
            var order = _mapper.Map<Order>(model);
            _context.Set<Order>().Add(order);
        }
        public async Task AddAsync(Order model)
        {
            await _context.Set<Order>().AddAsync(model);
        }
        public void Update(OrderCrudModel model)
        {
            var order = _mapper.Map<Order>(model);
            _context.Set<Order>().Update(order);
        }
        public void Delete(OrderCrudModel model)
        {
            var order = _mapper.Map<Order>(model);
            _context.Set<Order>().Remove(order);
        }

        public OrderCrudModel GetById(int id)
        {
            var order = _context.Set<Order>().Find(id);
            var data = _mapper.Map<OrderCrudModel>(order);
            return data;
        }
        public Order FirstOrDefault(Expression<Func<Order, bool>> predicate)
        {
            IQueryable<Order> set = _context.Set<Order>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<Order> BuildQuery(Expression<Func<Order, bool>> predicate)
        {
            IQueryable<Order> set = _context.Set<Order>();
            return set.Where(predicate);
        }

        public IEnumerable<Order> GetAll()
        {
            return _context.Set<Order>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
