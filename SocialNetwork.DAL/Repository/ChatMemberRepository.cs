﻿using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class ChatMemberRepository : IChatMemberRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public ChatMemberRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }

    public IQueryable<ChatMember> GetAll()
    {
        return _socialNetworkDbContext.ChatMembers
            .Include(c => c.Chat)
            .Include(c => c.Role.OrderBy(r => r.Rank))
            .Include(c => c.User)
            .ThenInclude(u => u.Profile)
            .AsQueryable();
    }

    public async Task<ChatMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.ChatMembers
            .Include(c => c.Chat)
            .Include(c => c.Role)
            .Include(c => c.User)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task SetRole(List<ChatMember> chatMembers, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.ChatMembers.UpdateRange(chatMembers);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ChatMember?> GetByUserIdAndChatId(int userId, int chatId,
        CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.ChatMembers
            .Include(c => c.Chat)
            .Include(c => c.Role)
            .ThenInclude(r => r.RoleAccesses)
            .Include(c => c.User).ThenInclude(i => i.Profile)
            .FirstOrDefaultAsync(i => i.User.Id == userId && i.Chat.Id == chatId, cancellationToken);
    }
}