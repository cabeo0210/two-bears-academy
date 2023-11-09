﻿using System.Linq.Dynamic.Core;
using AutoMapper;
using Ecommerce.Helper;
using Ecommerce.Repositories;
using EcommerceCore.Const;
using EcommerceCore.Models;
using EcommerceCore.ViewModel.TuyenSinh;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;

public class TuyenSinhController : Controller
{
    private EcommerceDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly EnrollRepository _enrollRepository;

    public TuyenSinhController(EcommerceDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _enrollRepository = new EnrollRepository(_mapper, _dbContext);
        
    }
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult GetEnroll(string? keySearch)
    {
        var user = HttpContext.Session.GetCurrentAuthentication();
        var enroll = _enrollRepository
            .Where(erroll => erroll.IsActive && !erroll.IsDeleted)
            .Include(erroll => erroll.User)
            .Include(erroll => erroll.Lead)
            .Include(erroll => erroll.HistoryEnrolls)
            .Where(erroll => erroll.UserId == user.UserId).ToList();
        if (keySearch != null)
        {
            enroll = enroll.Where(e =>
                (
                    (e.Lead.Name != null && e.Lead.Name.ToLower().Contains(keySearch.ToLower(), StringComparison.OrdinalIgnoreCase)) ||
                    (e.Lead.Email != null && e.Lead.Email.Contains(keySearch, StringComparison.OrdinalIgnoreCase)) ||
                    (e.Lead.Phone != null && e.Lead.Phone.Contains(keySearch, StringComparison.OrdinalIgnoreCase))
                )
            ).ToList();
        }

        var result = _mapper.Map<List<EnrollViewModel>>(enroll);
        return PartialView(result);


    }

    public async Task<IActionResult> UpdateEnrollHistory(EnrollUpdateModel model)
    {
        var enroll = _enrollRepository.Where(e=>e.EnrollId==model.EnrollId)
            .Include(e=>e.HistoryEnrolls)
            .FirstOrDefault();
        
        HistoryEnroll currentHis;
        foreach (var history in enroll!.HistoryEnrolls.Where(history => history.IsActive))
        {
            currentHis = _enrollRepository.GetHisEnrollById(history.HistoryEnrollId);
            currentHis.IsActive = false;
            currentHis.UpdatedAt = DateTime.Now;
            _enrollRepository.UpdateHistoryEnroll(currentHis);
            await _enrollRepository.CommitAsync();
        }

        currentHis = new HistoryEnroll()
        {
            EnrollId = enroll.EnrollId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsDeleted = false,
            IsActive = true,
            StatusEnroll = model.StatusCode
        };
        _enrollRepository.AddHistoryEnroll(currentHis);
        await _enrollRepository.CommitAsync();
        return new JsonResult("Oke");
    }
}