﻿using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IFriendshipRepository : IBasicRepository<Friendship>
{
    Task<bool> DeleteFriendsAsync(Friendship friendship, CancellationToken cancellationToken = default);
    Task<bool> DeleteAllFriendsAsync(int userId, CancellationToken cancellationToken = default);

    Task CreateFriendshipAsync(Friendship friendship, CancellationToken cancellationToken = default);
    
    IQueryable<Friendship> GetAllFriendsByUserId(int id);
}