﻿using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;

namespace SocialNetwork.Test.Services;

public class FakeMailService : IMailService
{
    public Task SendHtmlEmailAsync(MailModel mailModel)
    {
        return Task.CompletedTask;
    }
}