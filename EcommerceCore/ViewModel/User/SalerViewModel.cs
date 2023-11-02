using EcommerceCore.Models;

namespace EcommerceCore.ViewModel.User;

public class SalerViewModel : BaseUser
{
    public List<LeaderViewModel> LeaderViewModels { get; set; }
}