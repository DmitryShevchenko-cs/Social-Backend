﻿namespace SocialNetwork.BLL.Models;

public class FileModel : BaseModel
{
    public string FilePath { get; set; }
    public int MessageId { get; set; }
    public MessageModel Message { get; set; }
}