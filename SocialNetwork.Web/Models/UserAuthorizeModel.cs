﻿namespace SocialNetwork.Web.Models;

public class UserAuthorizeModel
{
    public string Login { get; set; }
    
    public string Password { get; set; }

    public bool isNeedToRemember { get; set; }
}