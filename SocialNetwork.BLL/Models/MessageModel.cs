﻿namespace SocialNetwork.BLL.Models;

public class MessageModel : BaseModel
{
    public string Text { get; set; } = null!;
    public ICollection<FileModel>? FileModels { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    
    
    public int AuthorId { get; set; }
    public int ChatId { get; set; }
    public int? ToReplyMessageId { get; set; }
    
    public ChatMemberModel? Author { get; set; }
    public ChatModel ChatModel { get; set; } = null!;
    public MessageModel? ToReplyMessage { get; set; }
    
    public ICollection<ReactionModel> Reactions { get; set; } = null!;
}