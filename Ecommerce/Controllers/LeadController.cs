using AutoMapper;
using Ecommerce.Repositories;
using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Tuyen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    public IActionResult Search(string q)
    {
        return Json("");
    }

    public IActionResult Edit(int id)
    {
        var leader = _leaderRepository.FirstOrDefault(x => x.LeadId == id);

        return View(_mapper.Map<LeaderCrudViewModel>(leader));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LeaderCrudViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var errorModel = new ErrorViewModel();
        try
        {
            var leader = _leaderRepository.FirstOrDefault(x => x.LeadId == model.LeadId);
            if (leader != null)
            {
                leader.UpdatedAt = DateTime.Now;
                leader.IsActive = model.IsActive;
                leader.IsDeleted = false;
                leader.Name = model.Name;
                leader.Note = model.Note;
                leader.Source = model.Source;
                leader.Phone = model.Phone;
                leader.Email = model.Email;

                await _leaderRepository.CommitAsync();
                return RedirectToAction("Index");
            }


            errorModel.ErrorMessage = "Lỗi không tìm thấy leader";
            return View("Error", errorModel);
        }
        catch (Exception)
        {
            errorModel.ErrorMessage = "Lỗi khi chỉnh sửa leader";
            return View("Error", errorModel);
        }
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
            leaderCrudViewModel.IsActive = true;
            leaderCrudViewModel.Position = SysEnum.LeadPosition.Lead.ToString();
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

    public IActionResult Index(string keyword)
    {
        var data = _leaderRepository.BuildQuery(
            x => !x.IsDeleted && x.IsActive);

        if (!string.IsNullOrEmpty(keyword))
        {
            data = data.Where(x => EF.Functions.Like(x.Name!, $"%{keyword}%"));
        }

        data = data.OrderByDescending(x => x.CreatedAt);
        var result = _mapper.Map<List<LeaderCrudViewModel>>(data.ToList());

        return View(result);
    }
}