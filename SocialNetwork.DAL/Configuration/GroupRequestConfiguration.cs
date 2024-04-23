﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration
{
    public class GroupRequestConfiguration : IEntityTypeConfiguration<GroupRequest>
    {
        public void Configure(EntityTypeBuilder<GroupRequest> builder)
        {
            //builder.HasKey(f => new { f.Id, f.SenderId, f.ToGroupId });

            //builder.Property(f => f.Id)
            //    .ValueGeneratedOnAdd();

            builder.HasOne(i => i.ToGroup)
                .WithMany()
                .HasForeignKey(i => i.ToGroupId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}