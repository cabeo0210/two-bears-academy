using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

public class TuyenSinhController : Controller
{
    private EcommerceDbContext _dbContext;
    
    public IActionResult Sale()
    {
        return View();
    }
    
    public IActionResult TuyenSinh()
    {
        return View();
    }
    
    public IActionResult Lead()
    {
        return View();
    }
}