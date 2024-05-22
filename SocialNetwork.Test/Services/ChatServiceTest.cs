using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.Test.Helpers;

namespace SocialNetwork.Test.Services;

public class ChatServiceTest : BaseMessageTestService<IChatService, ChatService>
{
    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
        services.AddScoped<IRoleGroupRepository, RoleGroupRepository>();

        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IFriendshipService, FriendshipService>();

        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<IReactionService, ReactionService>();
        services.AddScoped<IMessageReadStatusRepository, MessageReadStatusRepository>();

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped<IGroupBannedListRepository, GroupBannedListRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleGroupRepository, RoleGroupRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupService, GroupService>();

        services.AddScoped<IBlackListService, BlackListService>();
        services.AddScoped<IBlackListRepository, BlackListRepository>();

        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IRequestService, RequestService>();
        base.SetUpAdditionalDependencies(services);
    }

    
    [Test]
    public async Task CreateP2PChat_Ok_ChatCreated()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));

        var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
        await friendService.AddFriendshipAsync(user1!.Id, user2!.Id);

        await Service.CreateDirectChat(user1.Id, user2.Id);
        
        Assert.ThrowsAsync<P2PChatIsExistsException>(async() =>  await Service.CreateDirectChat(user1.Id, user2.Id)) ;

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };

        var chatList = await Service.FindChatByName(user1.Id, paginationModel, "");
        var chat = chatList.Data.First();
        foreach (var chatMember in chat.ChatMembers!)
        {
            Assert.That(chatMember.Role.Count == 1);
        }
    }

    [Test]
    public async Task CreateGroupChat_AddMember_ChatCreatedWith2Members()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        var user4 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        user3 = await userService.GetUserByLogin(user3.Login);
        user4 = await userService.GetUserByLogin(user4.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));
        Assert.That(user4, Is.Not.EqualTo(null));

        await Service.CreateGroupChat(user1!.Id, new ChatModel
        {
            Name = "Chat2",
            Logo = "null",
            IsGroup = true,
        });
        
        var chat = await Service.FindChatByName(user1.Id, new PaginationModel { CurrentPage = 1, PageSize = 10 }, "Chat2");
        Assert.That(chat.Data.Count() == 1);

        await Service.AddUsers(user1.Id, chat.Data.First().Id, new List<int> { user2!.Id, user3!.Id, user4!.Id });
        chat = await Service.FindChatByName(user1.Id, new PaginationModel { CurrentPage = 1, PageSize = 10 }, "Chat2");
        Assert.That(chat.Data.First().ChatMembers!.Count == 4);

        await Service.DelMembers(user1.Id, chat.Data.First().Id, new List<int>() { user2.Id });
        chat = await Service.FindChatByName(user1.Id, new PaginationModel { CurrentPage = 1, PageSize = 10 }, "Chat2");
        Assert.That(chat.Data.First().ChatMembers!.Count == 3);

        Assert.ThrowsAsync<UserNotFoundException>(async () => await Service.AddUsers(user1.Id, chat.Data.First().Id, new List<int> { user2.Id, user3.Id, user4.Id, 50000 }));
    }


    [Test]
    public async Task TryToAddUser_UserNotFound_ThrowError()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        await Service.CreateGroupChat(user1!.Id, new ChatModel
        {
            Name = "Chat2",
            Logo = "null",
            IsGroup = true,
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };
        var chat = (await Service.FindChatByName(user1.Id, paginationModel, "Chat2")).Data;
        Assert.ThrowsAsync<UserNotFoundException>( async () => await Service.AddUsers(user1.Id, chat.First().Id, new List<int>{5000, 6000, 7000}));
        Assert.ThrowsAsync<UserNotFoundException>( async () => await Service.AddUsers(user1.Id, chat.First().Id, new List<int>{user2!.Id, 6000, 7000}));
    }
    
    [Test]
    public async Task CreateGroupChats_GetChats_ChatCreatedWith2Members()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        Assert.That(user1, Is.Not.EqualTo(null));

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };
        await Service.CreateGroupChat(user1!.Id, new ChatModel
        {
            Name = "Chats1",
            Logo = "null",
            IsGroup = true,
        });
        await Service.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chats2",
            Logo = "null",
            IsGroup = true,
        });
        await Service.CreateGroupChat(user1.Id, new ChatModel
        {
            Name = "Chats3",
            Logo = "null",
            IsGroup = true,
        });
       
        var chat = await Service.GetAllChats(user1.Id, paginationModel);
        Assert.That(chat.Data.Count() == 3);
    }
    
    

    [Test]
    public async Task CreateRole_EditRole_RoleEdited()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        var user3 = await UserModelHelper.CreateTestDataAsync(userService);
        var user4 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        user3 = await userService.GetUserByLogin(user3.Login);
        user4 = await userService.GetUserByLogin(user4.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        Assert.That(user3, Is.Not.EqualTo(null));
        Assert.That(user4, Is.Not.EqualTo(null));
        
        await Service.CreateGroupChat(user1!.Id, new ChatModel
        {
            Name = "Chat3",
            Logo = "null",
            IsGroup = true,
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 1
        };

        var chat = (await Service.FindChatByName(user1.Id, paginationModel, "Chat3")).Data.First();
        Assert.That(chat is not null);
        
        await Service.AddUsers(user1.Id, chat!.Id, new List<int>{user2!.Id, user3!.Id});
        
        chat = (await Service.FindChatByName(user1.Id, paginationModel, "Chat3")).Data.First();
        Assert.That(chat.ChatMembers!.Count == 3);
        
        await Service.AddRole(user1.Id, chat.Id,new RoleModel
        {
            RoleName = "Role2",
            RoleColor = "black"
        });
        
        var role = (await Service.GetAllChatRoles(user1.Id, paginationModel, chat.Id)).Data.First();
        Assert.That(role is not null);

        await Service.SetRole(user1.Id, chat.Id, role!.Id, new List<int>(){user2.Id, user3.Id});
        
        role = (await Service.GetAllChatRoles(user1.Id, paginationModel, chat.Id)).Data.First();
        chat = (await Service.FindChatByName(user1.Id, paginationModel, "Chat3")).Data.First();
        Assert.That(chat.ChatMembers!.Any(m => m.Role.Any(r => r.RoleName == role.RoleName) && m.User.Login == user2.Login));
        
        role.RoleAccesses.Clear();
        await Service.EditRole(user1.Id, chat.Id, role.Id, role);
        role = (await Service.GetAllChatRoles(user1.Id,paginationModel, chat.Id)).Data.First();
        Assert.That(!role.RoleAccesses.Contains(ChatAccess.DelMembers) && !role.RoleAccesses.Contains(ChatAccess.DelMembers));
        
        role.RoleName = "Role21";
        role.RoleColor = "black1";
        role.RoleAccesses.Add(ChatAccess.DelMembers);
        role.RoleAccesses.Add(ChatAccess.EditNicknames);
        await Service.EditRole(user1.Id, chat.Id, role.Id, role);
        
        role = (await Service.GetAllChatRoles(user1.Id, paginationModel, chat.Id)).Data.First();
        chat = (await Service.FindChatByName(user1.Id, paginationModel, "Chat3")).Data.First();
        Assert.That(role.RoleName != "Role2" &&
                    chat.ChatMembers!.Any(c => c.Role.Any(r => r.RoleName != "Role2")));
        Assert.That(role.RoleAccesses.Contains(ChatAccess.DelMembers) && role.RoleAccesses.Contains(ChatAccess.DelMembers));
    }
    
    [Test]
    public async Task CreateRole_DelRole_RoleDeleted()
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
        
        await Service.CreateGroupChat(user1!.Id, new ChatModel
        {
            Name = "Chat4",
            Logo = "null",
            IsGroup = true,
        });

        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 10
        };
        var chat = (await Service.FindChatByName(user1.Id,paginationModel, "Chat4")).Data.First();
        
        Assert.That(chat is not null);
        
        await Service.AddUsers(user1.Id, chat!.Id, new List<int>{user2!.Id, user3!.Id});
        chat = (await Service.FindChatByName(user1.Id,paginationModel, "Chat4")).Data.First();
        Assert.That(chat.ChatMembers!.Count == 3);
        
        await Service.AddRole(user1.Id, chat.Id,new RoleModel
        {
            RoleName = "Role2",
            RoleColor = "black",
        });
        
        var role = (await Service.GetAllChatRoles(user1.Id,paginationModel, chat.Id)).Data.FirstOrDefault(r => r.RoleName == "Role2");
        Assert.That(role is not null);

        await Service.SetRole(user1.Id, chat.Id, role!.Id, new List<int>(){user2.Id, user3.Id});
        chat = (await Service.FindChatByName(user1.Id, paginationModel, "Chat4")).Data.First();
        Assert.That(chat.ChatMembers!
            .Any(m => m.Role
                .Any(r => r.RoleName == role.RoleName) && 
                      m.User.Id == user2.Id));
        
        Assert.That(chat.ChatMembers!
            .Any(m => m.Role
                          .Any(r => r.RoleName == role.RoleName) && 
                      m.User.Id == user3.Id));
        
        await Service.UnSetRole(user1.Id, chat.Id, role.Id, new List<int>(){user2.Id, user3.Id});
        chat = (await Service.FindChatByName(user1.Id, paginationModel, "Chat4")).Data.First();
        
        Assert.That(chat.ChatMembers!
            .Any(m => m.Role.Any(r => r.RoleName == role.RoleName&& 
                                                              m.User.Id == user2.Id)) == false);
        
        Assert.That(chat.ChatMembers!
            .Any(m => m.Role.Any(r => r.RoleName == role.RoleName&& 
                                      m.User.Id == user3.Id)) == false);
        
        await Service.DelRole(user1.Id, chat.Id, role.Id);
        Assert.That((await Service.GetAllChatRoles(user1.Id,paginationModel, chat.Id)).Data.Count() == 1);
    }
    [Test]
    public async Task TryToLeave_MakeHost_Leave()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var user1 = await UserModelHelper.CreateTestDataAsync(userService);
        var user2 = await UserModelHelper.CreateTestDataAsync(userService);
        user1 = await userService.GetUserByLogin(user1.Login);
        user2 = await userService.GetUserByLogin(user2.Login);
        Assert.That(user1, Is.Not.EqualTo(null));
        Assert.That(user2, Is.Not.EqualTo(null));
        
        await Service.CreateGroupChat(user1!.Id, new ChatModel
        {
            Name = "ChatLeave",
            Logo = "null",
            IsGroup = true,
        });
        var paginationModel = new PaginationModel
        {
            CurrentPage = 1,
            PageSize = 1
        };
        var chat = (await Service.FindChatByName(user1.Id, paginationModel, "ChatLeave")).Data.First();
        await Service.AddUsers(user1.Id, chat.Id, new List<int>{user2!.Id});
        chat = (await Service.FindChatByName(user1.Id, paginationModel, "ChatLeave")).Data.First();
        
        Assert.ThrowsAsync<CreatorCantLeaveException>( async () => await Service.LeaveChat(user1.Id, chat.Id));
        
        await Service.MakeHost(user1.Id, chat.Id, user2.Id);
        chat = (await Service.FindChatByName(user1.Id, paginationModel,"ChatLeave")).Data.First();
        var chatService = ServiceProvider.GetRequiredService<IChatMemberRepository>();
        var chatMember2 = await chatService.GetByUserIdAndChatId(user2.Id, chat.Id);
        
        Assert.That(chatMember2!.Role.Any(r => r.Rank == 0));
        
    }
    
}