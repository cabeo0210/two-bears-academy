using EcommerceCore.Models;

namespace EcommerceCore.ViewModel.TuyenSinh;

public class EnrollViewModel
{
    public int EnrollId { get; set; }
    public Models.User User { get; set; }
    public Lead Lead { get; set; }
    public List<HistoryEnroll> HistoryEnrolls { get; set; }
}