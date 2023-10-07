﻿using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scriban.Runtime;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.BL.Settings;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    private readonly IUserService _userService;
    private readonly IMailService _mailService;
    private readonly TokenHelper _tokenHelper;
    private readonly IMapper _mapper;

    public UserController(IUserService userService,IMailService mailService, TokenHelper tokenHelper, ILogger<UserController> logger, IMapper mapper)
    {
        _userService = userService;
        _mailService = mailService;
        _tokenHelper = tokenHelper;
        _logger = logger;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateViewModel user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to create user");

        var userModel = await _userService.CreateUserAsync(_mapper.Map<UserModel>(user), cancellationToken);

        _logger.LogInformation("User was created");

        _logger.LogInformation("Start to create mail");

        //IScriptObject test = new ScriptObject();
        //test.SetValue("name", user.Profile.Name, false);

        TemplatePathesModel pathesModel = new TemplatePathesModel();
        string template = TemplatePathesHelper.GetTemplate(pathesModel.MailActivation);
        await _mailService.SendHTMLEmailAsync(userModel.ToScriptObject,  template);

        _logger.LogInformation("Mail was created and sended");
        //WElcome
        // SendMail(user);

        return Ok();
    }

    
    [AllowAnonymous]
    [HttpPost("activation/{id}")]
    public async Task<IActionResult> ActivateUser (int id)
    {
        _logger.LogInformation("Start to activate user");

        await _userService.ActivateUserAsync(id);

        _logger.LogInformation("User activated");

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UserUpdateViewModel user,
       CancellationToken cancellationToken)
    {
         _logger.LogInformation("Start to update user");

        var userId = User.GetUserId(); //get user id by token
        await _userService.UpdateUserAsync(userId,_mapper.Map<UserModel>(user), cancellationToken); //searching and updating process

        _logger.LogInformation("User was updated");

        //Update

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserAsync(CancellationToken cancellationToken)
    {
         _logger.LogInformation("Get user");

        var userId = User.GetUserId(); //get user id by token
        var user = await _userService.GetByIdAsync(userId, cancellationToken); //find user by id
        var viewModel = _mapper.Map<UserViewModel>(user); // put user in userViewModel

        _logger.LogInformation("Get user");

        return Ok(viewModel);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUserAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start to delete user");

        await _userService.DeleteUserAsync(User.GetUserId(), cancellationToken); //delete user by id

        _logger.LogInformation("User was deleted");

        return Ok("User was deleted");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> AuthorizeUser([FromBody] UserAuthorizeModel model, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByLoginAndPasswordAsync(model.Login, model.Password, cancellationToken);
        var token = _tokenHelper.GetToken(user!.Id);
        var refreshToken = TokenHelper.GenerateRefreshToken(token);
        DateTime? expiredDate = model.IsNeedToRemember ? null : DateTime.Now;
        
        await _userService.AddAuthorizationValueAsync(
            user, 
            refreshToken, 
            LoginType.LocalSystem, 
            expiredDate, 
            cancellationToken);

        _logger.LogInformation("User was logined");

        return Ok(new { accessKey = token, refresh_token = refreshToken, expiredDate = expiredDate });
    }
   
    [AllowAnonymous]
    [HttpPost("new-token")]
    public async Task<IActionResult> UpdateTokenAsync([FromQuery] string refreshToken, CancellationToken cancellationToken)
    {

        refreshToken = refreshToken.Replace(" ", "+"); 
        var user = await _userService.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
        var token = _tokenHelper.GetToken(user.Id);
        return Ok(new { accessKey = token, refresh_token = refreshToken, expiredDate = user.AuthorizationInfo.ExpiredDate });
    }

   
    [HttpPost]
    [Route(("logout"))]
    public async Task<IActionResult> LogOutAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _userService.LogOutAsync(userId, cancellationToken);
        return Ok();

    }
}

