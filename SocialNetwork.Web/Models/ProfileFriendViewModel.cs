using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.Web.Models;

public class ProfileFriendViewModel
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;
    
    public string AvatarImage { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public Sex Sex { get; set; }
}