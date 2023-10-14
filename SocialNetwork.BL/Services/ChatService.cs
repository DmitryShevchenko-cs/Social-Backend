﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Helpers;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Options;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BL.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipService _friendshipService;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<ChatService> _logger;
    private readonly IChatMemberRepository _chatMemberRepository;
    private readonly IMapper _mapper;
    private readonly RoleOption _roleOptions;

    public ChatService(
        IChatRepository chatRepository,
        ILogger<ChatService> logger,
        IMapper mapper,
        IUserRepository userRepository,
        IFriendshipService friendshipService,
        IRoleRepository roleRepository,
        IChatMemberRepository chatMemberRepository,
        IOptions<RoleOption> roleOptions)
    {
        _chatRepository = chatRepository;
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _friendshipService = friendshipService;
        _roleRepository = roleRepository;
        _chatMemberRepository = chatMemberRepository;
        _roleOptions = roleOptions.Value;
    }

    private async Task<ChatMember?> GetUserInChatAsync(int userId, int chatId, ChatAccess access,
        CancellationToken cancellationToken)
    {
        return await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User.Id == userId)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.RoleAccesses.Any(i => i.ChatAccess == access)), cancellationToken);
    }

    public async Task<ChatModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var chat = await _chatRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chat, new ChatNotFoundException($"Chat with this Id {id} not found"));
        return _mapper.Map<ChatModel>(chat);
    }

    public async Task CreateP2PChat(int userId, int user2Id, ChatModel chatModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var user2Db = await _userRepository.GetByIdAsync(user2Id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        _logger.LogAndThrowErrorIfNull(user2Db, new UserNotFoundException($"User with this Id {userId} not found"));
        if (await _friendshipService.IsFriends(userDb!.Id, user2Db!.Id, cancellationToken) is false)
            return;

        chatModel.isGroup = false;
        var chatId = await _chatRepository.CreateChat(_mapper.Map<Chat>(chatModel), cancellationToken);
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        //p2pAdmin role
        var roleList = new List<Role>()
            { (await _roleRepository.GetByIdAsync(_roleOptions.RoleP2PAdminId, cancellationToken))! };

        await _chatRepository.AddChatMemberAsync(new ChatMember
        {
            Chat = chatDb!,
            User = userDb,
            Role = new List<Role>(roleList)
        }, chatDb!, cancellationToken);
        await _chatRepository.AddChatMemberAsync(new ChatMember
        {
            Chat = chatDb!,
            User = user2Db,
            Role = new List<Role>(roleList)
        }, chatDb!, cancellationToken);
    }

    public async Task<ChatModel> CreateGroupChat(int userId, ChatModel chatModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        chatModel.isGroup = true;
        var chatId = await _chatRepository.CreateChat(_mapper.Map<Chat>(chatModel), cancellationToken);
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var roleDb = await _roleRepository.CreateRole(new Role
        {
            RoleName = "everyone",
            RoleColor = "#FFFFFF",
            RoleAccesses = new List<RoleChatAccess>()
            {
                new ()
                {
                    ChatAccess =  ChatAccess.SendMessages
                },
                new ()
                {
                    ChatAccess =  ChatAccess.SendAudioMess
                },
                new ()
                {
                    ChatAccess =  ChatAccess.SendFiles
                },
                new ()
                {
                    ChatAccess =  ChatAccess.DelMessages
                },
            },
            Chat = chatDb,
            Rank = 100000
        }, cancellationToken);
        
        //admin role
        var roleList = new List<Role>
        {
            (await _roleRepository.GetByIdAsync(_roleOptions.RoleAdminId, cancellationToken))!,
            roleDb
        };

        var member = new ChatMember
        {
            Chat = chatDb!,
            User = userDb!,
            Role = new List<Role>(roleList)
        };
        await _chatRepository.AddChatMemberAsync(member, chatDb!, cancellationToken);
        return _mapper.Map<ChatModel>(await _chatRepository.GetByIdAsync(chatId, cancellationToken));
    }

    public async Task AddUsers(int userId, int chatId, List<int> userIds, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));


        var usersDb = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
        var notFoundUsers = userIds.Where(u => !usersDb.Select(i => i.Id).Contains(u)).ToList();
        if (notFoundUsers.Any())
        {
            throw new UserNotFoundException($"Users with ids {string.Join(", ", notFoundUsers)} not found");
        }
        
        if (chatDb!.IsGroup is false)
        {
            _logger.LogAndThrowErrorIfNull(chatDb, new NoRightException($"Chat is not group"));
        }

        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.AddMembers, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var alreadyIn = await _chatMemberRepository.GetAll()
            .Where(c => userIds.Contains(c.User.Id) && c.Chat.Id == chatId).Select(u => u.User.Id)
            .ToListAsync(cancellationToken);
        var idsToAdd = userIds.Except(alreadyIn);

        var roleList = new List<Role>()
        {
            (await _roleRepository.GetAll().Where(r => r.Chat == chatDb)
                .FirstOrDefaultAsync(r => r.RoleName == "everyone", cancellationToken))!
        };
        var usersToAdd = await _userRepository.GetAll().Where(i => idsToAdd.Contains(i.Id))
            .ToListAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(usersToAdd, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        List<ChatMember> chatMembers = new List<ChatMember>();
        foreach (var userToAdd in usersToAdd)
        {
            chatMembers.Add(new ChatMember
            {
                Chat = chatDb,
                User = userToAdd!,
                Role = new List<Role>(roleList)
            });
        }

        await _chatRepository.AddChatMemberAsync(chatMembers, chatDb, cancellationToken);
    }

    public async Task DelMember(int userId, int chatId, List<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        
        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.DelMembers, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var membersToDel = await _chatMemberRepository.GetAll().Where(i => userIds.Contains(i.Id))
            .ToListAsync(cancellationToken);
        if(membersToDel.Count == 0)
            throw new Exception($"Chat members not found");
        
        var AdminRole = await _roleRepository.GetByIdAsync(_roleOptions.RoleAdminId, cancellationToken);

        if (membersToDel.SingleOrDefault(i => i.Role.Contains(_mapper.Map<Role>(AdminRole))) is not null ||
            membersToDel.Contains(userInChat!))
            throw new Exception($"User is creator of chat");
            
        await _chatRepository.DelChatMemberAsync(membersToDel, chatDb!, cancellationToken);
    }

    public async Task DeleteChat(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat.Id == chatId)
            .Where(c => c.User == userDb)
            .SingleOrDefaultAsync(c => c.Role.Any(r => r.Id == 1), cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        await _chatRepository.DeleteChatAsync(chatDb!, cancellationToken);
    }

    public async Task<ChatModel> EditChat(int userId, int chatId, ChatModel chatModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));


        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditChat, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        foreach (var propertyMap in ReflectionHelper.WidgetUtil<ChatModel, Chat>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(chatModel);
            var roleTargetValue = roleDbProperty.GetValue(chatDb);

            if (roleSourceValue != null && roleSourceValue != "" && !roleSourceValue.Equals(roleTargetValue))
            {
                roleDbProperty.SetValue(chatDb, roleSourceValue);
            }
        }

        await _chatRepository.EditChat(chatDb!, cancellationToken);

        return _mapper.Map<ChatModel>(chatDb);
    }

    public async Task<List<ChatModel>> FindChatByName(int userId, string chatName,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        chatName = chatName.ToLower();
        var chatList = await _chatRepository.GetAll()
            .Where(i => i.ChatMembers!.Any(u => u.User.Id == userId) && i.Name.ToLower().StartsWith(chatName))
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatModel>>(chatList);
    }

    public async Task<List<ChatModel>> GetAllChats(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));

        var chatList = await _chatRepository.GetAll()
            .Where(chat => chat.ChatMembers!.Any(member => member.User.Id == userId))
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatModel>>(chatList);
    }

    public async Task AddRole(int userId, int chatId, RoleModel roleModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var isAlreadyNamed = await _roleRepository.GetAll()
            .Where(r => r.Chat!.Id == chatId && r.RoleName == roleModel.RoleName)
            .FirstOrDefaultAsync(cancellationToken);
        if (isAlreadyNamed is not null)
        {
            _logger.LogInformation("Role with this name is already created");
            throw new Exception("Role with this name is already created");
        }

        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var role = _mapper.Map<Role>(roleModel);
        role.Chat = chatDb;
        _logger.LogInformation("Role was added in chat");
        await _roleRepository.CreateRole(role, cancellationToken);
    }

    public async Task<RoleModel> GetRoleById(int userId, int chatId, int roleId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

         var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
         _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var role = await _roleRepository.GetAll()
            .Where(r => r.Id == roleId && r.Chat == chatDb).SingleOrDefaultAsync(cancellationToken);
        _logger.LogAndThrowErrorIfNull(role, new RoleNotFoundException($"Role not found"));
        return _mapper.Map<RoleModel>(role);
    }

    public async Task DelRole(int userId, int chatId, int roleId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

         var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
         _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(role, new RoleNotFoundException($"Role not found"));
        await _roleRepository.DeleteRole(role!, cancellationToken);
    }


    public async Task<RoleModel> EditRole(int userId, int chatId, int roleId, RoleModel roleModel,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));

        if (chatDb!.Roles!.Contains(roleDb!) == false)
            throw new Exception("This role is not in this chat");

        var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        foreach (var propertyMap in ReflectionHelper.WidgetUtil<RoleModel, Role>.PropertyMap)
        {
            var roleProperty = propertyMap.Item1;
            var roleDbProperty = propertyMap.Item2;

            var roleSourceValue = roleProperty.GetValue(roleModel);
            var roleTargetValue = roleDbProperty.GetValue(roleDb);

            if (roleSourceValue != null && roleSourceValue != "" && !roleSourceValue.Equals(roleTargetValue) && roleSourceValue.GetType() == roleTargetValue!.GetType())
            {
                roleDbProperty.SetValue(roleDb, roleSourceValue);
            }

            else if (roleSourceValue is List<ChatAccess> chatAccesses && chatAccesses.Any())
            {
                roleDbProperty.SetValue(roleDb, chatAccesses.Select(i => new RoleChatAccess()
                {
                    ChatAccess = i,
                    RoleId = roleDb!.Id
                }).ToList());
            }
        }

        var existingIds = new HashSet<int>(roleDb!.ChatMembers.Select(r => r.User.Id));
        var newIds = new HashSet<int>(roleModel.UsersIds);

        var idsToAdd = newIds.Except(existingIds);
        var idsToRemove = existingIds.Except(newIds);

        await SetRole(userId, chatId, roleDb.Id, idsToAdd.ToList(), cancellationToken);
        await UnSetRole(userId, chatId, roleDb.Id, idsToRemove.ToList(), cancellationToken);

        roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));

        await _roleRepository.EditRole(roleDb!, cancellationToken);
        roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {userId} not found"));
        return _mapper.Map<RoleModel>(roleDb);
    }


    public async Task SetRole(int userId, int chatId, int roleId, List<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

         var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
         _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));

        var chatMembersDb = new List<ChatMember>() { };
        foreach (var uId in userIds)
        {
            var chatMemberDb = await _chatMemberRepository.GetAll().Where(m => m.Chat == chatDb && m.User.Id == uId)
                .SingleOrDefaultAsync(cancellationToken);
            _logger.LogAndThrowErrorIfNull(chatMemberDb,
                new UserNotFoundException($"User with this Id {uId} not found"));
            chatMemberDb!.Role.Add(roleDb!);
            chatMembersDb.Add(chatMemberDb!);
        }

        await _chatMemberRepository.SetRole(chatMembersDb!, cancellationToken);
    }

    public async Task UnSetRole(int userId, int chatId, int roleId, List<int> userIds,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

         var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
         _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));

        var chatMembers = new List<ChatMember>() { };
        foreach (var uId in userIds)
        {
            var chatMemberDb = await _chatMemberRepository.GetAll()
                .Where(m => m.Chat == chatDb && m.User.Id == uId)
                .SingleOrDefaultAsync(cancellationToken);
            _logger.LogAndThrowErrorIfNull(chatMemberDb,
                new UserNotFoundException($"User with this Id {uId} not found"));
            chatMemberDb!.Role.Remove(roleDb!);
            chatMembers.Add(chatMemberDb!);
        }

        await _chatMemberRepository.SetRole(chatMembers!, cancellationToken);
    }

    public async Task<List<RoleModel>> GetAllChatRoles(int userId, int chatId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var roles = await _roleRepository.GetAll().Where(r => r.Chat == chatDb).ToListAsync(cancellationToken);
        return _mapper.Map<List<RoleModel>>(roles);
    }

    public async Task<List<ChatMemberModel>> GetChatMembers(int userId, int chatId, int roleId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));
        var roleDb = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(roleDb, new RoleNotFoundException($"Role with this Id {roleId} not found"));

        var chatMembers = await _chatMemberRepository.GetAll()
            .Where(c => c.Role.Any(r => r == roleDb) && c.Chat == chatDb)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatMemberModel>>(chatMembers);
    }

    public async Task<List<ChatMemberModel>> GetChatMembers(int userId, int chatId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var chatMembers = await _chatMemberRepository.GetAll()
            .Where(c => c.Chat == chatDb)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<ChatMemberModel>>(chatMembers);
    }

    public async Task<List<RoleModel>> EditRolesRank(int userId, int chatId, List<RoleModel> roleModels,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

         var userInChat = await GetUserInChatAsync(userId, chatId, ChatAccess.EditRoles, cancellationToken);
         _logger.LogAndThrowErrorIfNull(userInChat, new NoRightException($"You have no rights for it"));

        var roleIds = roleModels.Select(rm => rm.Id).ToList();
        var rolesDb = await _roleRepository.GetAll().Where(r => r.Chat.Id == chatDb.Id && roleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        for (int i = 0; i < roleModels.Count; i++)
        {
            rolesDb[i].Rank = roleModels[i].Rank;
        }

        await _roleRepository.EditRole(rolesDb!, cancellationToken);
        return _mapper.Map<List<RoleModel>>(rolesDb);
    }

    public async Task LeaveChat(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var userInChat = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new UserNotFoundException($"ChatMember with this Id {userId} not found"));
        
        var AdminRole = await _roleRepository.GetByIdAsync(_roleOptions.RoleAdminId, cancellationToken);
        if (userInChat!.Role.Contains(_mapper.Map<Role>(AdminRole)))
        {
            throw new Exception("You are creator, you can`t do this");
        }
        
        await _chatRepository.DelChatMemberAsync(new List<ChatMember>{userInChat!}, chatDb!, cancellationToken);
    }

    public async Task MakeHost(int userId, int chatId, int chatMemberId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with this Id {chatId} not found"));

        var chatMember = await _chatMemberRepository.GetByIdAsync(chatMemberId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMember, new UserNotFoundException($"ChatMember with this Id {chatMemberId} not found"));
        
        var userInChat = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userInChat, new UserNotFoundException($"ChatMember with this Id {userId} not found"));
        
        var AdminRole = await _roleRepository.GetByIdAsync(_roleOptions.RoleAdminId, cancellationToken);
        if (!userInChat!.Role.Contains(_mapper.Map<Role>(AdminRole)))
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        AdminRole!.ChatMembers.Remove(userInChat);
        AdminRole.ChatMembers.Add(chatMember!);
        await _roleRepository.EditRole(_mapper.Map<Role>(AdminRole), cancellationToken);
    }
}