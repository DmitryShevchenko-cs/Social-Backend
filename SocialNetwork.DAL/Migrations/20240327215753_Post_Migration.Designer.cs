﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialNetwork.DAL;

#nullable disable

namespace SocialNetwork.DAL.Migrations
{
    [DbContext(typeof(SocialNetworkDbContext))]
    [Migration("20240327215753_Post_Migration")]
    partial class Post_Migration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChatMemberRole", b =>
                {
                    b.Property<int>("ChatMembersId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("ChatMembersId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("ChatMemberRole");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.AuthorizationInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("LoginType")
                        .HasColumnType("int");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("AuthorizationInfo");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.BaseNotificationEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InitiatorId")
                        .HasColumnType("int");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("NotificationMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ToUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InitiatorId");

                    b.ToTable("Notifications");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseNotificationEntity");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.BasePostEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Posts");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BasePostEntity");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.BlackList", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("BannedUserId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("UserId", "BannedUserId");

                    b.HasIndex("BannedUserId");

                    b.ToTable("BlackLists");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsGroup")
                        .HasColumnType("bit");

                    b.Property<string>("Logo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.ChatMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ChatId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId");

                    b.ToTable("ChatMembers");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.FileEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Files");

                    b.HasDiscriminator<string>("Discriminator").HasValue("FileEntity");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.FriendRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<int>("ReceiverId")
                        .HasColumnType("int");

                    b.HasKey("Id", "SenderId", "ReceiverId");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("FriendRequests");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Friendship", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("FriendId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("UserId", "FriendId");

                    b.HasIndex("FriendId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int>("ChatId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEdited")
                        .HasColumnType("bit");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ToReplyMessageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ChatId");

                    b.HasIndex("ToReplyMessageId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.MessageReadStatus", b =>
                {
                    b.Property<int>("ChatMemberId")
                        .HasColumnType("int");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ReadAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ChatMemberId", "MessageId");

                    b.HasIndex("MessageId");

                    b.ToTable("MessageReadStatuses");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AvatarImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Reaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("MessageId");

                    b.ToTable("Reactions");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ChatId")
                        .HasColumnType("int");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("RoleColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.RoleChatAccess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ChatAccess")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleChatAccess");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorizationInfoId")
                        .HasColumnType("int");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OnlineStatus")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProfileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.ChatNotification", b =>
                {
                    b.HasBaseType("SocialNetwork.DAL.Entity.BaseNotificationEntity");

                    b.Property<int>("ChatId")
                        .HasColumnType("int");

                    b.HasIndex("ChatId");

                    b.HasDiscriminator().HasValue("ChatNotification");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.FriendRequestNotification", b =>
                {
                    b.HasBaseType("SocialNetwork.DAL.Entity.BaseNotificationEntity");

                    b.Property<int>("FriendRequestId")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("FriendRequestNotification");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.UserPost", b =>
                {
                    b.HasBaseType("SocialNetwork.DAL.Entity.BasePostEntity");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasIndex("UserId");

                    b.HasDiscriminator().HasValue("UserPost");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.FileInMessage", b =>
                {
                    b.HasBaseType("SocialNetwork.DAL.Entity.FileEntity");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.HasIndex("MessageId");

                    b.HasDiscriminator().HasValue("FileInMessage");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.FileInPost", b =>
                {
                    b.HasBaseType("SocialNetwork.DAL.Entity.FileEntity");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.HasIndex("PostId");

                    b.HasDiscriminator().HasValue("FileInPost");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.MessageNotification", b =>
                {
                    b.HasBaseType("SocialNetwork.DAL.Entity.ChatNotification");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.HasIndex("MessageId");

                    b.HasDiscriminator().HasValue("MessageNotification");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.ReactionNotification", b =>
                {
                    b.HasBaseType("SocialNetwork.DAL.Entity.ChatNotification");

                    b.Property<int>("ReactionId")
                        .HasColumnType("int");

                    b.HasIndex("ReactionId")
                        .IsUnique()
                        .HasFilter("[ReactionId] IS NOT NULL");

                    b.HasDiscriminator().HasValue("ReactionNotification");
                });

            modelBuilder.Entity("ChatMemberRole", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.ChatMember", null)
                        .WithMany()
                        .HasForeignKey("ChatMembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.AuthorizationInfo", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.User", "User")
                        .WithOne("AuthorizationInfo")
                        .HasForeignKey("SocialNetwork.DAL.Entity.AuthorizationInfo", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.BaseNotificationEntity", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.User", "Initiator")
                        .WithMany("Notifications")
                        .HasForeignKey("InitiatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Initiator");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.BlackList", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.User", "BannedUser")
                        .WithMany()
                        .HasForeignKey("BannedUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.User", "User")
                        .WithMany("BlackLists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("BannedUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.ChatMember", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.Chat", "Chat")
                        .WithMany("ChatMembers")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.User", "User")
                        .WithMany("ChatMembers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.FriendRequest", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.User", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.User", "Sender")
                        .WithMany("Requests")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Friendship", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.User", "FriendUser")
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.User", "User")
                        .WithMany("Friends")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("FriendUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Message", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.ChatMember", "Author")
                        .WithMany("MessagesSent")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.Message", "ToReplyMessage")
                        .WithMany()
                        .HasForeignKey("ToReplyMessageId");

                    b.Navigation("Author");

                    b.Navigation("Chat");

                    b.Navigation("ToReplyMessage");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.MessageReadStatus", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.ChatMember", "ChatMember")
                        .WithMany()
                        .HasForeignKey("ChatMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.Message", "Message")
                        .WithMany("MessageReadStatuses")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChatMember");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Profile", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.User", "User")
                        .WithOne("Profile")
                        .HasForeignKey("SocialNetwork.DAL.Entity.Profile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Reaction", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.ChatMember", "Author")
                        .WithMany("Reactions")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SocialNetwork.DAL.Entity.Message", "Message")
                        .WithMany("Reactions")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Role", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.Chat", "Chat")
                        .WithMany("Roles")
                        .HasForeignKey("ChatId");

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.RoleChatAccess", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.Role", "Role")
                        .WithMany("RoleAccesses")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.ChatNotification", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.Chat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.UserPost", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.FileInMessage", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.Message", "Message")
                        .WithMany("Files")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.FileInPost", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.BasePostEntity", "Post")
                        .WithMany("Files")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.MessageNotification", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.Message", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.ReactionNotification", b =>
                {
                    b.HasOne("SocialNetwork.DAL.Entity.Reaction", "Reaction")
                        .WithOne("Notification")
                        .HasForeignKey("SocialNetwork.DAL.Entity.ReactionNotification", "ReactionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Reaction");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.BasePostEntity", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Chat", b =>
                {
                    b.Navigation("ChatMembers");

                    b.Navigation("Messages");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.ChatMember", b =>
                {
                    b.Navigation("MessagesSent");

                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Message", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("MessageReadStatuses");

                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Reaction", b =>
                {
                    b.Navigation("Notification");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.Role", b =>
                {
                    b.Navigation("RoleAccesses");
                });

            modelBuilder.Entity("SocialNetwork.DAL.Entity.User", b =>
                {
                    b.Navigation("AuthorizationInfo");

                    b.Navigation("BlackLists");

                    b.Navigation("ChatMembers");

                    b.Navigation("Friends");

                    b.Navigation("Notifications");

                    b.Navigation("Profile")
                        .IsRequired();

                    b.Navigation("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}
