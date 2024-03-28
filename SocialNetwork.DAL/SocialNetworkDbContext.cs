﻿using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
#pragma warning disable CS8618

namespace SocialNetwork.DAL;

public class SocialNetworkDbContext : DbContext
{
    public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }

    public DbSet<Profile> Profiles { get; set; }
    
    public DbSet<Friendship> Friends { get; set; }
    
    public DbSet<FriendRequest> FriendRequests { get; set; }
    
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMember> ChatMembers { get; set; }
    public DbSet<Role> Roles { get; set; }
    
    public DbSet<Message> Messages { get; set; }

    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<BaseFileEntity> Files { get; set; }
    public DbSet<FileInMessage> FilesInMassages { get; set; }
    public DbSet<FileInPost> FilesInPost { get; set; }
    
    public DbSet<BasePostEntity> Posts { get; set; }
    public DbSet<UserPost> UserPosts { get; set; }

    public DbSet<BlackList> BlackLists { get; set; }
    public DbSet<FriendRequestNotification> FriendRequestNotifications { get; set; }
    public DbSet<ChatNotification> ChatFriendRequestNotifications { get; set; }
    public DbSet<MessageNotification> MessageNotifications { get; set; }
    public DbSet<ReactionNotification> ReactionNotifications { get; set; }
    public DbSet<BaseNotificationEntity> Notifications { get; set; }
    public DbSet<MessageReadStatus> MessageReadStatuses { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SocialNetworkDbContext).Assembly);
    }
    
}