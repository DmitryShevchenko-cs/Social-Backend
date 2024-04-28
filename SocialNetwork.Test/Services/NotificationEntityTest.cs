﻿using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class NotificationEntityTest : BaseMessageTestService<INotificationService, NotificationService>
{

    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendRequestService, FriendRequestService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();

        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();
        
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();

        base.SetUpAdditionalDependencies(services);
    }
    
    [Test]
    public async Task CreateNewFriendRequest_CheckFriendRequestNotificationEntity_OK()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        var fiendRequestService = ServiceProvider.GetRequiredService<IFriendRequestService>();
        await fiendRequestService.SendRequest(createdUser1!.Id, createdUser2!.Id);
        var notification = await Service.GetByUserId(createdUser2!.Id);
        var notificationModels = notification.ToList();
        Assert.That(notificationModels.First().ToUserId == createdUser2!.Id);
        Assert.That(notificationModels.First().IsRead is false);
    }

    [Test]
    public async Task CreateFriendRequestNotification_ChangeReadProp_DeleteNotification()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        var fiendRequestService = ServiceProvider.GetRequiredService<IFriendRequestService>();
        await fiendRequestService.SendRequest(createdUser1!.Id, createdUser2!.Id);
        
        var fRequest = await fiendRequestService.GetByUsersId(createdUser1.Id, createdUser2.Id);
        var notifications = await Service.GetByUserId(createdUser2.Id);
        var notification = notifications.First();
        await Service.ReadNotification(createdUser2.Id, notification!.Id);
        
        notification = await Service.GetByIdAsync(notification.Id);
        Assert.That(notification!.IsRead);
    
        await Service.RemoveNotification(createdUser2.Id, notification.Id);
        Assert.ThrowsAsync<NotificationNotFoundException>(async () => await Service.GetByIdAsync(notification.Id));
    }

    [Test]
    public async Task AddToChat_DeleteFromChat_GetNotifications()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var createdUser1 = await userService.GetUserByLogin(user1.Login);
        var createdUser2 = await userService.GetUserByLogin(user2.Login);
        
        var chatService = ServiceProvider.GetRequiredService<IChatService>();

        var chat = await chatService.CreateGroupChat(createdUser1!.Id, new ChatModel
        {
            Name = "Chat",
            Logo = "null",
            IsGroup = false,
        });
        await chatService.AddUsers(createdUser1!.Id, chat.Id, new List<int>{ createdUser2!.Id });
        var notifications = (await Service.GetByUserId(createdUser2.Id)).ToList();
        Assert.That(notifications.Count() == 1
            && notifications.First().GetType() == typeof(ChatNotificationModel));
        
        await chatService.DelMembers(createdUser1.Id, chat.Id, new List<int> { createdUser2.Id });
        notifications = (await Service.GetByUserId(createdUser2.Id)).ToList();
        Assert.That(notifications.Count() == 2 
                    && notifications.Skip(1).FirstOrDefault()?.GetType() == typeof(ChatNotificationModel));
    }
}