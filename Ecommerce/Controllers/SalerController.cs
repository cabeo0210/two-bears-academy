using AutoMapper;
using Ecommerce.Repositories;
using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.Tuyen;
using EcommerceCore.ViewModel.TuyenSinh;
using EcommerceCore.ViewModel.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;

public class SalerController : Controller
{
    private readonly UserRepository _userRepository;
    private readonly EnrollRepository _enrollRepository;
    private readonly LeaderRepository _leaderRepository;
    private readonly IMapper _mapper;
    private EcommerceDbContext _dbContext;

    public SalerController(EcommerceDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userRepository = new UserRepository(_dbContext, _mapper);
        _enrollRepository = new EnrollRepository(_mapper, _dbContext);
        _leaderRepository = new LeaderRepository(_mapper, _dbContext);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEnroll(int LeadId, int SaleId)
    {
        var enroll = _enrollRepository.Add(new Enroll
        {
            UserId = SaleId,
            LeadId = LeadId,
            IsActive = true,
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now
        });


        await _enrollRepository.CommitAsync();

        _enrollRepository.AddHistoryEnroll(new HistoryEnroll
        {
            EnrollId = enroll.Entity.EnrollId,
            StatusEnroll = (int)SysEnum.StatusEnroll.Register,
            IsActive = true,
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now
        });

        await _enrollRepository.CommitAsync();

        return Json("Oke");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var leader = _leaderRepository.FirstOrDefault(x => x.LeadId == id);

        leader.IsDeleted = true;
        await _leaderRepository.CommitAsync();

        return new JsonResult("OK");
    }

    public IActionResult PCLead4Sale(int id, string keyword)
    {
        ViewData["SalerName"] =
            _dbContext.Users.First(x =>
                x.Role == (int)SysEnum.Role.Sale
                && x.IsActive
                && !x.IsDeleted
                && x.UserId == id).Name;

        ViewData["SalerId"] =
            _dbContext.Users.First(x =>
                x.Role == (int)SysEnum.Role.Sale
                && x.IsActive
                && !x.IsDeleted
                && x.UserId == id).UserId;

        var allLead = _dbContext.Leads.Where(
            x =>
                x.ClaimUserId == null
                && x.IsActive
                && !x.IsDeleted).ToList();

        var data = _enrollRepository
            .Where(erroll => erroll.IsActive && !erroll.IsDeleted)
            .Include(erroll => erroll.Lead)
            .Where(erroll => erroll.UserId == id)
            .Select(a => a.Lead)
            .Where(l => !l.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToList();

        var leadsNotEnroll =
            allLead.Where(lead => !data.Contains(lead)).ToList(); // Create a list to store leads for removal

        ViewData["Leads"] = _mapper.Map<List<LeaderCrudViewModel>>(leadsNotEnroll);

        if (!string.IsNullOrEmpty(keyword))
        {
            // data = data.Where(x => EF.Functions.Like(x.Name!, $"%{keyword}%")).ToList();
            data = data.Where(x =>
                    x.Name.Contains($"{keyword}") ||
                    x.Email.Contains($"{keyword}") ||
                    x.Phone.Contains($"{keyword}") ||
                    x.LeadId.ToString().Contains($"{keyword}"))
                .ToList();
        }

        var result = _mapper.Map<List<LeaderCrudViewModel>>(data);

        return View(result);
    }

    public IActionResult Index()
    {
        var data = _dbContext.Users.Where(
            x =>
                x.Role == (int)SysEnum.Role.Sale
                && x.IsActive
                && !x.IsDeleted
        );

        data = data.OrderByDescending(x => x.CreatedAt);
        var result = _mapper.Map<List<UserViewModel>>(data.ToList());

        return View(result);
    }
}