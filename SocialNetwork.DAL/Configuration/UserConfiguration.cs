﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasOne(i => i.Profile)
            .WithOne(i => i.User)
            .HasForeignKey<Profile>(i => i.UserId);

        builder.HasOne(i => i.AuthorizationInfo)
            .WithOne(i => i.User)
            .HasForeignKey<AuthorizationInfo>(i => i.UserId);
    }
}