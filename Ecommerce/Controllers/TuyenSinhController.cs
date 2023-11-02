using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

public class TuyenSinhController : Controller
{
    private EcommerceDbContext _dbContext;
    
    public IActionResult Sale()
    {


        return Json("This is sale manager page");
    }
    
    public IActionResult TuyenSinh()
    {


        return View();
    }
    
    public IActionResult Lead()
    {


        return Json("This is lead manager page");
    }
}