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
        var leader = _leaderRepository.FirstOrDefault(x => x.LeadId == id);

        leader.IsDeleted = true;
        await _leaderRepository.CommitAsync();

        return new JsonResult("Xóa leader thành công");
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
        var data = _leaderRepository.BuildQuery(
            x => !x.IsDeleted);
        data = data.OrderByDescending(x => x.CreatedAt);
        var result = _mapper.Map<List<LeaderCrudViewModel>>(data.ToList());

        return View(result);
    }
}