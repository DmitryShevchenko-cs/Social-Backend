﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class ChatMembersConfiguration : IEntityTypeConfiguration<ChatMember>
{
    public void Configure(EntityTypeBuilder<ChatMember> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasOne(i => i.User)
            .WithMany(i => i.ChatMembers);
        
        builder.HasMany(i => i.Role)
            .WithMany(r => r.ChatMembers);
        
        builder.HasMany(i => i.MessagesSent)
            .WithOne(r => r.Sender)
            .OnDelete(DeleteBehavior.Cascade);
    }
}