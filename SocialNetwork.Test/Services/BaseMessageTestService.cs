﻿using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class BaseMessageTestService<T, V> : DefaultServiceTest<T, V>
    where T : class
    where V : class, T
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
        
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<IReactionService, ReactionService>();
        
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IMessageReadStatusRepository, MessageReadStatusRepository>();

        base.SetUpAdditionalDependencies(services);
    }
    
    public override void SetUp()
    {
        base.SetUp();
        var roleRepo = ServiceProvider.GetRequiredService<IRoleRepository>();
        foreach (var role in RoleHelper.CreateRole())
        {
            roleRepo.CreateRole(role).Wait();
        }
    }
}