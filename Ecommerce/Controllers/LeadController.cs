using AutoMapper;
using Ecommerce.Repositories;
using EcommerceCore.ViewModel.Tuyen;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

public class LeadController : Controller
{
    private EcommerceDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly LeaderRepository _leaderRepository;

    public LeadController(EcommerceDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _leaderRepository = new LeaderRepository(_mapper, _dbContext);
        
    }


    [HttpPost]
    public async Task<IActionResult> Create(LeaderCrudViewModel leaderCrudViewModel)
    {
        if (!ModelState.IsValid) return View();

        try
        {
            Console.WriteLine(leaderCrudViewModel.Name);
            _leaderRepository.Add(leaderCrudViewModel);
            await _leaderRepository.CommitAsync();

            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            // return View("Error");
            throw;
        }


        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Index()
    {
        return View();
    }
}