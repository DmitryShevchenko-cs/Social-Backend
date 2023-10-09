﻿using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class ReactionServiceTest : DefaultServiceTest<IReactionService, ReactionService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IReactionService, ReactionService>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
        
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        base.SetUpAdditionalDependencies(services);
    }
    
    private async Task CreateRole()
    {
        var _roleRepository =  ServiceProvider.GetRequiredService<IRoleRepository>();
        await _roleRepository.CreateRole(new Role
        {
            RoleName = "@everyone",
            RoleColor = "null",
            RoleAccesses = new List<ChatAccess>()
            {
                ChatAccess.SendMessages,
                ChatAccess.SendAudioMess,
                ChatAccess.SendFiles,
                ChatAccess.DelMessages
            }
        });
        
        await _roleRepository.CreateRole(new Role
        {
            RoleName = "Admin",
            RoleColor = "null",
            RoleAccesses = new List<ChatAccess>()
            {
                ChatAccess.SendMessages,
                ChatAccess.SendAudioMess,
                ChatAccess.SendFiles,
                ChatAccess.EditRoles,
                ChatAccess.AddMembers,
                ChatAccess.DelMembers,
                ChatAccess.MuteMembers,
                ChatAccess.DelMessages,
                ChatAccess.EditNicknames,
                ChatAccess.EditChat
            }
        });
        await _roleRepository.CreateRole(new Role
        {
            RoleName = "P2PAdmin",
            RoleColor = "null",
            RoleAccesses = new List<ChatAccess>()
            {
                ChatAccess.SendMessages,
                ChatAccess.SendAudioMess,
                ChatAccess.SendFiles,
                ChatAccess.EditRoles,
                ChatAccess.AddMembers,
                ChatAccess.DelMembers,
                ChatAccess.MuteMembers,
                ChatAccess.DelMessages,
                ChatAccess.EditNicknames,
                ChatAccess.EditChat
            }
        });
    }
    
     [Test]
    public async Task CreateMessages_AddReactions_EditReaction()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        user3 = await userService.GetUserByLogin(user3.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));

        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await friendService.AddFriendshipAsync(user1!.Id, user2!.Id);
        await friendService.AddFriendshipAsync(user1!.Id, user3!.Id);

        await CreateRole();

        var chatService = ServiceProvider.GetRequiredService<IChatService>();
        await chatService.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chat2",
            Logo = "null",
            isGroup = true,
        });

        var chatList = await chatService.FindChatByName(user1.Id, "Chat2");
        var chat = chatList.First();

        await chatService.AddUsers(user1.Id, chat.Id, new List<int>
        {
            user2.Id,
            user3.Id
        });

        var messageService = ServiceProvider.GetRequiredService<IMessageService>();
        await messageService.CreateMessage(user1.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 1",
            Files = "test1.png"
        });
        var message2 = await messageService.CreateMessage(user2.Id, chat.Id, new MessageModel()
        {
            Text = "Test message 2",
            Files = "test2.png"
        });
        var message3 = await messageService.ReplyMessage(user3.Id, chat.Id, message2.Id, new MessageModel()
        {
            Text = "Test message 3",
            Files = "test3.png"
        });
        
        await Service.AddReaction(user1.Id, message2.Id, new ReactionModel
        {
            Type = "smile"
        });
        await Service.AddReaction(user1.Id, message2.Id, new ReactionModel
        {
            Type = "like"
        });
        await Service.AddReaction(user1.Id, message3.Id, new ReactionModel
        {
            Type = "smile"
        });
        
        var messages = await messageService.GetMessages(user2.Id, chat.Id);

        Assert.That(messages.Count() == 3);
        Assert.That(messages.Any(c => c.Text == "Test message 2" &&  c.Reactions.Any(c => c.Type == "like")));
        Assert.That(messages.Any(c => c.Text == "Test message 2" &&  c.Reactions.Any(c => c.Type == "smile")) is false);
        Assert.That(messages.Any(c => c.Text == "Test message 3" &&  c.Reactions.Any(c => c.Type == "smile")));

    }
    
}