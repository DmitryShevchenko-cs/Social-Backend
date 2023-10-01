﻿using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class ReactionRepository : IReactionRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public ReactionRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }

    public IQueryable<Reaction> GetAll()
    {
        return _socialNetworkDbContext.Reactions.Include(i => i.Author)
            .Include(i => i.Message)
            .AsQueryable();
    }

    public async Task<Reaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Reactions.Include(i => i.Author)
            .Include(i => i.Message)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CreateReactionAsync(Reaction reaction, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Reactions.AddAsync(reaction, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task EditReactionAsync(Reaction reaction, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Reactions.Update(reaction);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteReactionAsync(Reaction reaction, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Reactions.Remove(reaction);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}