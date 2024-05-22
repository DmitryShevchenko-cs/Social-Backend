using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FriendshipController : ControllerBase
{
    private readonly ILogger<FriendshipController> _logger;
    private readonly IMapper _mapper;
    private readonly IFriendshipService _friendshipService;

    public FriendshipController(IFriendshipService friendshipService, ILogger<FriendshipController> logger, IMapper mapper)
    {
        _friendshipService = friendshipService;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpDelete]
    public async Task<IActionResult> DelFriendship([FromQuery] int FriendID, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _friendshipService.DeleteFriendshipAsync(userId, FriendID, cancellationToken);
        return Ok();
    }
    
    /// <param name="request">mail or name_surname</param>
    [HttpGet] 
    public async Task<IActionResult> GetFriendship([FromQuery] string? request, [FromQuery] PaginationModel pagination, [FromQuery] string sortType, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if(string.IsNullOrEmpty(request))
        {
            var userModelFriends = await _friendshipService.GetAllFriends(userId, pagination, sortType, cancellationToken);
            return Ok(_mapper.Map<PaginationResultViewModel<FriendViewModel>>(userModelFriends));
        }
        if (request.Contains('@'))
        {
            var userModelFriends = await _friendshipService.FindFriendByEmail(userId, request, cancellationToken);
            var friend = _mapper.Map<FriendViewModel>(userModelFriends);
            return Ok(friend);
        }
        else
        {
            var userModelFriends = await _friendshipService.FindFriendByNameSurname(userId, pagination, request, cancellationToken);
            var friend = _mapper.Map<PaginationResultViewModel<FriendViewModel>>(userModelFriends);
            return Ok(friend);
        }
    }
}

