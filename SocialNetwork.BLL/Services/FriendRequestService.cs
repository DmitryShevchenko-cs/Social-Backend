﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BLL.Services;

public class FriendRequestService : IFriendRequestService
{
    private readonly IUserService _userService;
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ILogger<FriendRequestService> _logger;
    private readonly IMapper _mapper;
    private readonly IBlackListService _blackListService;
    
    public FriendRequestService(IFriendRequestRepository friendRequestRepository, ILogger<FriendRequestService> logger, IMapper mapper, IUserService userService, IFriendshipRepository friendshipRepository, IBlackListService blackListService)
    {
        _friendRequestRepository = friendRequestRepository;
        _logger = logger;
        _mapper = mapper;
        _userService = userService;
        _friendshipRepository = friendshipRepository;
        _blackListService = blackListService;
    }
    
    public async Task<FriendRequestModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var friendRequest = await _friendRequestRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(friendRequest, new FriendRequestException($"Friend request by id {id} not found"));
        return _mapper.Map<FriendRequestModel>(friendRequest);
    }
    
    public async Task<FriendRequestModel> GetByUsersId(int senderId, int receiverId, CancellationToken cancellationToken = default)
    {
        var friendRequest = await _friendRequestRepository.GetAll()
            .Where(i => i.ReceiverId == receiverId && i.SenderId == senderId)
            .FirstOrDefaultAsync(cancellationToken);
        
        _logger.LogAndThrowErrorIfNull(friendRequest, new FriendRequestException("Friend request not found"));
        return _mapper.Map<FriendRequestModel>(friendRequest);
    }

    public async Task SendRequest(int userId, int receiverId, CancellationToken cancellationToken = default)
    {
        var senderModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var receiverModel = await _userService.GetByIdAsync(receiverId, cancellationToken);

        _logger.LogAndThrowErrorIfNull(senderModel, new UserNotFoundException($"User with ID {userId} not found."));
        _logger.LogAndThrowErrorIfNull(receiverModel, new UserNotFoundException($"User with ID {receiverId} not found."));

        var friends = await _friendshipRepository
            .GetAllFriendsByUserId(userId)
            .Where(f => f.UserId == userId && f.FriendId == receiverId || f.UserId == receiverId && f.FriendId == userId)
            .Select(f => f.UserId == userId ? f.FriendUser : f.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (friends is not null)
        {
            _logger.LogError("You cant send a friend request to friend");
            throw new FriendRequestException("Friendship is already created");
        }

        var requestExists = await _friendRequestRepository.RequestExists(userId, receiverId, cancellationToken);
        if (requestExists is true)
        {
            _logger.LogError("Friend request already exists");
            throw new FriendRequestException("Friend request already exists");
        }

        if (senderModel!.Id != receiverModel!.Id)
        {
            if (await _blackListService.IsBannedUser(senderModel.Id, receiverModel.Id, cancellationToken))
            {
                _logger.LogError("You can't send a friend request to a banned user");
                throw new FriendRequestException("Friend request can't be sent to a banned user");
            }
            if (await _blackListService.IsBannedUser(receiverModel.Id, senderModel.Id, cancellationToken))
            {
                _logger.LogError("You can't send a friend request to a user who banned you");
                throw new FriendRequestException("Friend request can't be sent to a user who banned you");
            }
            else
            {
                await _friendRequestRepository.CreateFriendRequestAsync(new FriendRequest()
                {
                    SenderId = senderModel.Id,
                    ReceiverId = receiverModel.Id
                }, cancellationToken);
            }
        }
        else
        {
            _logger.LogError("You can't send a friend request to yourself");
            throw new FriendRequestException("Friend request can't be sent to yourself");
        }

    }


    public async Task AcceptRequest(int userId, int requestId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var friendRequest = await _friendRequestRepository.GetByIdAsync(requestId, cancellationToken);
        
        _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException($"User with ID {userId} not found."));
        _logger.LogAndThrowErrorIfNull(friendRequest, new FriendRequestException($"Friend request by id {requestId} not found"));

        var friends = await _friendshipRepository
            .GetAllFriendsByUserId(userId)
            .Where(f => f.UserId == userId && f.FriendId == friendRequest!.ReceiverId || f.UserId == friendRequest!.ReceiverId && f.FriendId == userId)
            .Select(f => f.UserId == userId ? f.FriendUser : f.User)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (friends is not null)
        {
            _logger.LogError("You cant accept a friend request from friend");
            throw new FriendRequestException("You cant accept a friend request from friend");
        }
        
        if (friendRequest!.ReceiverId == userModel!.Id)
        {
            await _friendshipRepository.CreateFriendshipAsync(new Friendship()
            {
                UserId = userModel!.Id,
                FriendId = friendRequest!.Sender.Id,   
            }, cancellationToken);

            await _friendRequestRepository.DeleteFriendRequestAsync(new FriendRequest()
            {
                SenderId = friendRequest!.Sender.Id,
                ReceiverId = userModel.Id
            }, cancellationToken);
        }
        else
        {
            _logger.LogError("User is not receiver");
            throw new FriendRequestException("User is not receiver");
        }
           
    }

    public async Task CancelRequest(int userId, int requestId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var friendRequest = await _friendRequestRepository.GetByIdAsync(requestId, cancellationToken);
        
        _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException($"User with ID {userId} not found."));
        _logger.LogAndThrowErrorIfNull(friendRequest, new FriendRequestException($"Friend request by id {requestId} not found"));
        
        if (friendRequest!.ReceiverId == userModel!.Id)
        {
            await _friendRequestRepository.DeleteFriendRequestAsync(new FriendRequest()
            {
                SenderId = friendRequest!.Sender.Id,
                ReceiverId = userModel.Id
            }, cancellationToken);
        }
        else
        {
            _logger.LogError("User is not receiver");
            throw new FriendRequestException("User is not receiver");
        }
    }

    public async Task<IEnumerable<FriendRequestModel>> GetAllIncomeRequest(int userId, CancellationToken cancellationToken = default)
    {
        var userRequests = await _friendRequestRepository.GetAll()
            .Include(u=>u.Sender.Profile)
            .Include(u=>u.Receiver.Profile)
            .Where(u => u.ReceiverId == userId)
            .ToListAsync(cancellationToken);
        return _mapper.Map<List<FriendRequestModel>>(userRequests);
    }

    public async Task<IEnumerable<FriendRequestModel>> GetAllSentRequest(int userId, CancellationToken cancellationToken)
    {
        var userRequests = await _friendRequestRepository.GetAll()
            .Include(u=>u.Sender.Profile)
            .Include(u=>u.Receiver.Profile)
            .Where(u => u.SenderId == userId)
            .ToListAsync(cancellationToken);
        
        return _mapper.Map<List<FriendRequestModel>>(userRequests);
    }
}