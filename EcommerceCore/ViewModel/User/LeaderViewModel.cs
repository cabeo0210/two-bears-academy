using EcommerceCore.Models;

namespace EcommerceCore.ViewModel.User;

public class LeaderViewModel : BaseUser
{
    public bool LeaderStatus { get; set; }
    public string LeaderNotes { get; set; }
    public string IsInvitedFrom { get; set; }
    
    public List<SalerViewModel> SalerViewModels { get; set; }
}