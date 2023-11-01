using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class UserRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public UserRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(UserCrudModel model)
        {
            var user = _mapper.Map<User>(model);
            _context.Set<User>().Add(user);
        }
        public void Update(UserCrudModel model)
        {
            var user = _mapper.Map<User>(model);
            _context.Set<User>().Update(user);
        }
        public void Delete(UserCrudModel model)
        {
            var user = _mapper.Map<User>(model);
            _context.Set<User>().Remove(user);
        }

        public UserCrudModel GetById(int id)
        {
            var user = _context.Set<User>().Find(id);
            var data = _mapper.Map<UserCrudModel>(user);
            return data;
        }
        public User FirstOrDefault(Expression<Func<User, bool>> predicate)
        {
            IQueryable<User> set = _context.Set<User>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<User> Where(Expression<Func<User, bool>> predicate)
        {
            IQueryable<User> set = _context.Set<User>();
            return set.Where(predicate);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Set<User>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
