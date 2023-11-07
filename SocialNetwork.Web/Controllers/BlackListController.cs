﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;
using System.Collections.Generic;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BlackListController : ControllerBase
    {
        private readonly ILogger<BlackListController> _logger;
        private readonly TokenHelper _tokenHelper;
        private readonly IMapper _mapper;
        private readonly IBlackListService _blackListService;
       

        public BlackListController(IBlackListService blackListService, TokenHelper tokenHelper,
            ILogger<BlackListController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _tokenHelper = tokenHelper;
            _blackListService = blackListService;
          
        }
        [HttpPost]
        public async Task<IActionResult> AddUserToBlackList([FromQuery] int wantToBanId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _blackListService.AddUserToBlackListAsync(userId, wantToBanId, cancellationToken);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DelUserBlackList([FromQuery] int bannedID, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _blackListService.DeleteUserFromBlackListAsync(userId, bannedID, cancellationToken);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBannedUsers(CancellationToken cancellationToken)
        {
            var loggedInUserId = User.GetUserId();
            var userModels = await _blackListService.GetAllBannedUser(loggedInUserId, cancellationToken);
            return Ok(_mapper.Map<List<BannedUserViewModel>>(userModels));
        }
    }
}