﻿using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Hubs;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IMapper _mapper;
    private readonly IChatService _chatService;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    public ChatController(ILogger<ChatController> logger, IMapper mapper, IChatService chatService, IHubContext<NotificationHub> notificationHubContext)
    {
        _logger = logger;
        _mapper = mapper;
        _chatService = chatService;
        _notificationHubContext = notificationHubContext;
    }


    [HttpPost]
    public async Task<IActionResult> CreateGroupChat([FromBody]ChatCreateViewModel chatCreateViewModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to create group chat");
        var userId = User.GetUserId();
        var chat = await _chatService.CreateGroupChat(userId, _mapper.Map<ChatModel>(chatCreateViewModel), cancellationToken);
        _logger.LogInformation("Group chat was created");
        return Ok(_mapper.Map<ChatViewModel>(chat));
    }

    [HttpPost("add-chat-member")]
    public async Task<IActionResult> AddChatMember([FromBody] AddUserInChatModel addUserInChatModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to add user in chat");
        var userId = User.GetUserId();
        var notifications = await _chatService.AddUsers(userId, addUserInChatModel.ChatId, addUserInChatModel.NewMeberIds, cancellationToken);
        _logger.LogInformation("User was added in chat");
        foreach (var notification in notifications)
        {
            await _notificationHubContext.Clients.Group(notification!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notification)), cancellationToken: cancellationToken);
        }
        return Ok();
    }

    [HttpDelete("chat-member")]
    public async Task<IActionResult> DelChatMember([FromBody] DelChatMembersModel delChatMembersModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete user in chat");
        var userId = User.GetUserId();
        var notifications = await _chatService.DelMembers(userId, delChatMembersModel.ChatId, delChatMembersModel.MemberIds , cancellationToken);
        _logger.LogInformation("User was deleted in chat");
        foreach (var notification in notifications)
        {
            await _notificationHubContext.Clients.Group(notification!.ToUserId.ToString())
                .SendAsync("ReceivedNotification", JsonSerializer.Serialize(_mapper.Map<BaseNotificationViewModel>(notification)), cancellationToken);
        }
        return Ok();
    }
    
    [HttpPost("edit-chat")]
    public async Task<IActionResult> EditChat([FromBody] ChatEditModel chatEditModel ,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var chat = await _chatService.EditChat(userId, chatEditModel.ChatId, _mapper.Map<ChatModel>(chatEditModel), cancellationToken);
        return Ok(_mapper.Map<ChatViewModel>(chat));
    }
    
    [HttpDelete("{chatId:int}")]
    public async Task<IActionResult> DelChat(int chatId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete chat");
        var userId = User.GetUserId();
        await _chatService.DeleteChat(userId, chatId, cancellationToken);
        _logger.LogInformation("Chat was deleted");
        return Ok();
    }

    [HttpGet("chats-by-name")]
    public async Task<IActionResult> FindChatByName([FromQuery] PaginationModel pagination,[FromQuery] string chatName, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var chat = await _chatService.FindChatByName(userId, pagination,chatName, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<ChatViewModel>>(chat));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllChats([FromQuery] PaginationModel? pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var chat = await _chatService.GetAllChats(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<ChatViewModel>>(chat));
    }
    

    [HttpGet("chat-members")]
    public async Task<IActionResult> GetChatMembers([FromQuery] PaginationModel pagination, [FromQuery] int chatId, [FromQuery] int roleId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if(roleId != 0)
            return Ok(_mapper.Map<PaginationResultViewModel<ChatMemberViewModel>>
                (await _chatService.GetChatMembers(userId, pagination, chatId, roleId, cancellationToken)));
        
        return Ok(_mapper.Map<PaginationResultViewModel<ChatMemberViewModel>>
            (await _chatService.GetChatMembers(userId, pagination, chatId, cancellationToken)));
    }

    [HttpPost("leave")]
    public async Task<IActionResult> EditRolesRank([FromQuery] int chatId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _chatService.LeaveChat(userId, chatId, cancellationToken);
        return Ok();
    }
    

    [HttpPost("make-host")]
    public async Task<IActionResult> MakeHost([FromQuery] int chatId, [FromQuery] int chatMemberId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _chatService.MakeHost(userId, chatId, chatMemberId, cancellationToken);
        return Ok();
    }
    

}