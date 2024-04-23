﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RequestController : ControllerBase
{

    private readonly ILogger<RequestController> _logger;
    private readonly IMapper _mapper;
    private readonly IRequestService _requestService;

    public RequestController(ILogger<RequestController> logger, IMapper mapper, IRequestService requestService)
    {
        _logger = logger;
        _mapper = mapper;
        _requestService = requestService;
    }

    [HttpPost("friend/{receiverId:int}")]
    public async Task<IActionResult> SendFriendRequest(int receiverId, CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        await _requestService.SendFriendRequestAsync(new FriendRequestModel() { SenderId = userId, ToUserId = receiverId }, cancellationToken);
        return Ok();
    }

    [HttpPost("group/{receiverId:int}")]
    public async Task<IActionResult> SendGroupRequest(int receiverId, CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        await _requestService.SendGroupRequestAsync(new GroupRequestModel() { SenderId = userId, ToGroupId = receiverId }, cancellationToken);
        return Ok();
    }

    [HttpPost("friend/accept/{requestId:int}")]
    public async Task<IActionResult> AcceptFriendRequest(int requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _requestService.AcceptFriendRequest(userId, requestId, cancellationToken);
        return Ok();
    }

    [HttpPost("group/accept/{requestId:int}")]
    public async Task<IActionResult> AcceptGroupRequest(int requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _requestService.AcceptGroupRequest(userId, requestId, cancellationToken);
        return Ok();
    }

    [HttpPost("friend/cancel/{requestId:int}")]
    public async Task<IActionResult> CancelFriendRequest(int requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _requestService.CancelFriendRequest(userId, requestId, cancellationToken);
        return Ok();
    }

    [HttpPost("group/cancel/{requestId:int}")]
    public async Task<IActionResult> CancelGroupRequest(int requestId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _requestService.CancelGroupRequest(userId, requestId, cancellationToken);
        return Ok();
    }

    [HttpGet("friend/sent")]
    public async Task<IActionResult> GetAllSentFriendRequest([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var requests = await _requestService.GetAllSentFriendRequest(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<BaseRequestViewModel>>(requests));
    }

    [HttpGet("group/sent")]
    public async Task<IActionResult> GetAllSentGroupRequest([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var requests = await _requestService.GetAllSentGroupRequest(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<BaseRequestViewModel>>(requests));
    }

    [HttpGet("friend/received")]
    public async Task<IActionResult> GetAllIncomeFriendRequest([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var requests = await _requestService.GetAllIncomeFriendRequest(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<BaseRequestViewModel>>(requests));
    }

    [HttpGet("group/received")]
    public async Task<IActionResult> GetAllIncomeGroupRequest([FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var requests = await _requestService.GetAllIncomeGroupRequest(userId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<BaseRequestViewModel>>(requests));
    }
}