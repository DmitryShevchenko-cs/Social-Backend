﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly TokenHelper _tokenHelper;

    public UserController(IUserService userService, TokenHelper tokenHelper)
    {
        _userService = userService;
        _tokenHelper = tokenHelper;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateViewModel user,
        CancellationToken cancellationToken)
    {
        await _userService.CreateUserAsync(UserVieModelMapper.ConvertToBlModel(user), cancellationToken);

        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("login")]
    public async Task<ActionResult> AuthorizeUser([FromBody] UserAuthorizeModel model)
    {
        var user = await _userService.GetUserByLoginAndPasswordAsync(model.Login, model.Password);

        var token = _tokenHelper.GetToken(user.Id);

        return Ok(token);
    }
    
}