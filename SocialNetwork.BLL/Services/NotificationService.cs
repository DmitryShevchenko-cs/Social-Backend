﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BLL.Services;

public class NotificationService : INotificationService
{
    private readonly IUserService _userService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<ChatService> _logger;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository notificationRepository, ILogger<ChatService> logger, IMapper mapper, IUserService userService)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<NotificationModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var friendRequestNotification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(friendRequestNotification,
            new NotificationNotFoundException($"Notification with this Id {id} not found"));
        return _mapper.Map<FriendRequestNotificationModel>(friendRequestNotification);
    }
    public async Task<NotificationModel?> GetByIdAsync(int id, NotificationType notificationType, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, notificationType, cancellationToken);
        _logger.LogAndThrowErrorIfNull(notification,
            new NotificationNotFoundException($"Notification with this Id {id} not found"));
        
        switch (notificationType)
        {
            case NotificationType.FriendRequest:
                    return _mapper.Map<FriendRequestNotificationModel>(notification);
            default:
                throw new ArgumentOutOfRangeException(nameof(notificationType), notificationType, null);
        }
        
    }

   public async Task<NotificationModel> CreateNotification(NotificationModel notificationModel,
        CancellationToken cancellationToken = default)
    {
        var friendRequestNotificationId = await _notificationRepository.CreateNotification(
            _mapper.Map<DAL.Entity.FriendRequestNotification>(notificationModel), cancellationToken);
        return _mapper.Map<FriendRequestNotificationModel>(await _notificationRepository.GetByIdAsync(friendRequestNotificationId, cancellationToken));
    }
   
   public async Task RemoveNotification(int userId, int notificationId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userService.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(notification,
            new NotificationNotFoundException($"Notification with this Id {notificationId} not found"));
        await _notificationRepository.RemoveNotification(notification!, cancellationToken);
    }

    public async Task ReadNotification(int userId, int notificationId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userService.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(notification,
            new NotificationNotFoundException($"Notification with this Id {notificationId} not found"));
        if (notification!.UserId == userId)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateNotification(notification, cancellationToken);
        }
        else
        {
            _logger.LogAndThrowErrorIfNull(notification,
                new NotificationNotFoundException($"Notification with this user id {notificationId} not found"));
        }
         
    }

    public async Task<IEnumerable<NotificationModel>> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userService.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var notification = await _notificationRepository.GetAll().Where(r => r.UserId == userId).ToListAsync(cancellationToken);
        
        _logger.LogAndThrowErrorIfNull(notification,
            new NotificationNotFoundException($"Notifications with this user id {userId} not found"));
        return _mapper.Map<List<NotificationModel>>(notification);
    }
}