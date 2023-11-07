using AutoMapper;
using Ecommerce.Repositories;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Tuyen;
using Microsoft.AspNetCore.Mvc;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

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
    public async Task<IActionResult> Delete(int id)
    {
        var leader = _leaderRepository.GetById(id);
        if (leader != null)
        {
            _leaderRepository.Delete(_mapper.Map<LeaderCrudViewModel>(leader));
            await _leaderRepository.CommitAsync();
        }

        Response.StatusCode = 200;
        return new JsonResult("Xóa thành công");
    }


    [HttpPost]
    public async Task<IActionResult> Create(LeaderCrudViewModel leaderCrudViewModel)
    {
        if (!ModelState.IsValid) return View();

        try
        {
            _leaderRepository.Add(leaderCrudViewModel);
            await _leaderRepository.CommitAsync();

            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Index()
    {
        var leads = _leaderRepository.GetAll();

        return View(_mapper.Map<List<LeaderCrudViewModel>>(leads));
    }
}