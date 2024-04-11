namespace SocialNetwork.DAL.Entity;

public class Chat : BaseEntity
{
    
    public string Name { get; set; } = null!;
    
    public string Logo { get; set; } = null!;
    
    public bool IsGroup { get; set; }
    
    public ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();
    public ICollection<Role> Roles { get; set; } = null!;
    
    public ICollection<Message> Messages { get; set; } = null!;
}