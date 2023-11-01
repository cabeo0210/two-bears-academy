using AutoMapper;
using EcommerceCore.Models;
using EcommerceCore.Models ;
using EcommerceCore.ViewModel;
using EcommerceCore.ViewModel.Image;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class ImageRepository
    {
        protected readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ImageRepository(EcommerceDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Add(ImageCrudModel model)
        {
            var user = _mapper.Map<Image>(model);
            _context.Set<Image>().Add(user);
        }
        public void Update(ImageCrudModel model)
        {
            var user = _mapper.Map<Image>(model);
            _context.Set<Image>().Update(user);
        }
        public void Delete(ImageCrudModel model)
        {
            var user = _mapper.Map<Image>(model);
            _context.Set<Image>().Remove(user);
        }

        public ImageCrudModel GetById(int id)
        {
            var user = _context.Set<Image>().Find(id);
            var data = _mapper.Map<ImageCrudModel>(user);
            return data;
        }
        public Image FirstOrDefault(Expression<Func<Image, bool>> predicate)
        {
            IQueryable<Image> set = _context.Set<Image>();
            return set.FirstOrDefault(predicate);
        }

        public IQueryable<Image> BuildQuery(Expression<Func<Image, bool>> predicate)
        {
            IQueryable<Image> set = _context.Set<Image>();
            return set.Where(predicate);
        }

        public IEnumerable<Image> GetAll()
        {
            return _context.Set<Image>().ToList();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
