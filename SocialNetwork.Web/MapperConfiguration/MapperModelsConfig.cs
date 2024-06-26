﻿using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using SocialNetwork.BLL.Models;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.Web.Models;
using Profile = AutoMapper.Profile;

namespace SocialNetwork.Web.MapperConfiguration
{
    public class MapperModelsConfig : Profile
    {
        public MapperModelsConfig()
        {
            CreateMap<UserCreateViewModel, UserModel>();

            CreateMap<ProfileCreateViewModel, ProfileModel>();
            CreateMap<FriendshipModel, Friendship>().ReverseMap();
            CreateMap<FriendRequestModel, FriendRequest>().ReverseMap();
            CreateMap<BlackListModel, BlackList>().ReverseMap();

            CreateMap<BaseRequestViewModel, BaseRequestModel>()
                .ForMember(dest => dest.SenderId, opt => opt.Ignore())
                .ReverseMap();

            CreatePaginationResultMapping<BannedUserViewModel, UserModel>();
            CreatePaginationResultMapping<FriendViewModel, UserModel>();
            CreatePaginationResultMapping<BaseRequestViewModel, BaseRequestModel>();

            CreatePaginationResultMapping<ChatViewModel, ChatModel>();
            CreatePaginationResultMapping<RoleViewModel, RoleModel>();
            CreatePaginationResultMapping<ChatMemberViewModel, ChatMemberModel>();
            CreatePaginationResultMapping<MessageViewModel, MessageModel>();
            CreatePaginationResultMapping<BasePostViewModel, BasePostModel>();
            CreatePaginationResultMapping<UserPostViewModel, UserPostModel>();
            CreatePaginationResultMapping<LikePostViewModel, LikePostModel>();
            CreatePaginationResultMapping<CommentPostViewModel, CommentPostModel>();
            
            CreatePaginationResultMapping<BannedUsersInGroupViewModel, BannedUserInGroupModel>();
            CreatePaginationResultMapping<BannedUserList, BannedUserInGroupModel>();
            

            CreatePaginationResultMapping<GroupViewModel, GroupModel>();
            CreatePaginationResultMapping<GroupMemberViewModel, GroupMemberModel>();
            CreatePaginationResultMapping<RoleGroupViewModel, RoleGroupModel>();
            void CreatePaginationResultMapping<TViewModel, TModel>()
            {
                CreateMap<PaginationResultViewModel<TViewModel>, PaginationResultModel<TModel>>()
                    .ForMember(dest => dest.PageSize, opt => opt.Ignore())
                    .ReverseMap();
            }
            
            CreateMap<ProfileFriendViewModel, ProfileModel>()
                .ForMember(dest =>dest.Birthday, opt=> opt.Ignore())
                .ForMember(dest =>dest.Description, opt=> opt.Ignore())
                .ReverseMap();
            CreateMap<FriendViewModel, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest =>dest.Login, opt=> opt.Ignore())
                .ForMember(dest =>dest.Password, opt=> opt.Ignore())
                .ForMember(dest =>dest.IsEnabled, opt=> opt.Ignore())
                .ForMember(dest =>dest.AuthorizationInfo, opt=> opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<ProfileBannedUserModel, ProfileModel>()
                 .ForMember(dest => dest.Birthday, opt => opt.Ignore())
                 .ForMember(dest => dest.Description, opt => opt.Ignore())
                 .ReverseMap();
            CreateMap<BannedUserViewModel, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Login, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.IsEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationInfo, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<ProfileCreateViewModel, ProfileModel>();  

            CreateMap<ProfileUpdateViewModel, ProfileModel>(); //created map model based on ProfileUpdateViewModel
            CreateMap<UserUpdateViewModel, UserModel>(); //created map model based on UserUpdateViewModel

            CreateMap<ProfileModel, ProfileViewModel>(); //created map model based on ProfileGetViewModel
            CreateMap<UserModel, UserViewModel>(); //created map model based on UserGetViewModel


            CreateMap<UserModel, User>()
                .ForMember(dest => dest.ProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationInfoId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<AuthorizationInfoModel, AuthorizationInfo>().ReverseMap();
            CreateMap<ProfileModel, SocialNetwork.DAL.Entity.Profile>().ReverseMap();
            CreateMap<ChatModel, Chat>().ReverseMap();

            
            CreateMap<ChatMemberModel, ChatMember>().ReverseMap();
            
            CreateMap<Role, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.UsersIds, opt => opt.MapFrom(src => src.ChatMembers.Select(cm => cm.User.Id)))
                .ForMember(dest => dest.RoleAccesses, from => from.MapFrom(f => f.RoleAccesses.Select(i => i.ChatAccess)))
                .ReverseMap();

            CreateMap<RoleGroup, RoleGroupModel>()
                .ForMember(dest => dest.UsersIds, opt => opt.MapFrom(src => src.GroupMembers.Select(cm => cm.User.Id)))
                .ForMember(dest => dest.RoleAccesses, from => from.MapFrom(f => f.RoleAccesses.Select(i => i.GroupAccess)))
                .ReverseMap();

            CreateMap<ChatAccess, RoleChatAccess>()
                .ForMember(dest => dest.ChatAccess, from => from.MapFrom(f => f));

            CreateMap<GroupAccess, RoleGroupAccess>()
                            .ForMember(dest => dest.GroupAccess, from => from.MapFrom(f => f));

            CreateMap<ChatCreateViewModel, ChatModel>()
                .ForMember(dest => dest.ChatMembers, opt=>opt.Ignore())
                .ForMember(dest => dest.Roles, opt=>opt.Ignore())
                .ReverseMap();

            CreateMap<CreateRoleModel, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.UsersIds, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CreateRoleGroupModel, RoleGroupModel>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.UsersIds, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RoleEditModel, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<RoleViewModel, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RoleGroupViewModel, RoleGroupModel>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<GroupMemberRoleGroupViewModel, RoleGroupModel>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.UsersIds, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ChatMemberViewModel, ChatMemberModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<ChatViewModel, ChatModel>()
                .ForMember(dest => dest.ChatMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ChatEditModel, ChatModel>()
                .ForMember(dest => dest.ChatMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<ChatModel, ChatEditModel>()
                .ForMember(dest => dest.ChatId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<GroupEditModel, GroupModel>()
                .ForMember(dest => dest.GroupMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<GroupModel, GroupEditModel>()
                .ForMember(dest => dest.GroupId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RoleRankModel, RoleModel>()
                .ForMember(dest => dest.RoleName, opt => opt.Ignore())
                .ForMember(dest => dest.RoleColor, opt => opt.Ignore())
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<Message, MessageModel>()
                .ForMember(dest => dest.Files, opt => opt.MapFrom(d => d.Files))
                .ReverseMap();
            
            CreateMap<Reaction, ReactionModel>()
                .ReverseMap();
            
            CreateMap<BaseFileModel, BaseFileEntity>()
                .ReverseMap();
            
            CreateMap<FileInPostModel, FileInPost>()
                .IncludeBase<BaseFileModel, BaseFileEntity>()
                .ReverseMap();
            
            CreateMap<FileInMessageModel, FileInMessage>()
                .IncludeBase<BaseFileModel, BaseFileEntity>()
                .ReverseMap();
            
            CreateMap<FileViewModel, FileInMessageModel>()
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ReverseMap();
            
                        
            CreateMap<FileViewModel, FileInPostModel>()
                .ForMember(dest => dest.PostId, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<FileSend, FileInMessageModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<MessageViewModel, MessageModel>()
                .ForMember(dest => dest.Files, opt => opt.MapFrom(d => d.FileModels))
                .ForMember(dest => dest.ToReplyMessage, opt => opt.Ignore())
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<ReactionViewModel, ReactionModel>()
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<AddReactionModel, ReactionModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UserViewModel, ChatMemberModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<NotificationEntity, BaseNotificationModel>().ReverseMap();
           

            CreateMap<UserViewModel, GroupMemberModel>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.RoleGroup, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<NotificationEntity, BaseNotificationModel>().ReverseMap();
            CreateMap<BaseRequestEntity, BaseRequestModel>().ReverseMap();

            CreateMap<FriendRequestNotification, FriendRequestNotificationModel>()
                .IncludeBase<NotificationEntity, BaseNotificationModel>();

            CreateMap<MessageReadStatusModel, MessageReadStatus>()
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.ChatMember, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<MessageReadStatusModel, MessageReadStatusViewModel>()
                .ReverseMap();
            
            CreateMap<ChatNotification, ChatNotificationModel>()
                .IncludeBase<NotificationEntity, BaseNotificationModel>()
                .ReverseMap();
            
            CreateMap<FriendRequestNotification, FriendRequestNotificationModel>()
                .IncludeBase<NotificationEntity, BaseNotificationModel>();

            CreateMap<BaseNotificationViewModel, BaseNotificationModel>()
                .ReverseMap();
            
            CreateMap<MessageNotification, MessageNotificationModel>()
                .IncludeBase<ChatNotification, ChatNotificationModel>()
                .IncludeBase<NotificationEntity, BaseNotificationModel>()
                .ReverseMap();
            
            CreateMap<ReactionNotification, ReactionNotificationModel>()
                .IncludeBase<ChatNotification, ChatNotificationModel>()
                .IncludeBase<NotificationEntity, BaseNotificationModel>()
                .ReverseMap();
            
            CreateMap<ChatNotificationViewModel, ChatNotificationModel>()
                .IncludeBase<BaseNotificationViewModel, BaseNotificationModel>()
                .ReverseMap();
            
            CreateMap<FriendRequestNotificationViewModel, FriendRequestNotificationModel>()
                .IncludeBase<BaseNotificationViewModel, BaseNotificationModel>()
                .ReverseMap();
            
            CreateMap<MessageNotificationViewModel, MessageNotificationModel>()
                .IncludeBase<ChatNotificationViewModel, ChatNotificationModel>()
                .IncludeBase<BaseNotificationViewModel, BaseNotificationModel>()
                .ReverseMap();
            
            CreateMap<ReactionNotificationViewModel, ReactionNotificationModel>()
                .IncludeBase<ChatNotificationViewModel, ChatNotificationModel>()
                .IncludeBase<BaseNotificationViewModel, BaseNotificationModel>()
                .ReverseMap();

            CreateMap<BasePostEntity, BasePostModel>().ReverseMap();
                
            CreateMap<UserPost, UserPostModel>()
                .IncludeBase<BasePostEntity, BasePostModel>()
                .ReverseMap();

            CreateMap<CreateUserPostModel, UserPostModel>()
                .ForMember(dest => dest.Files, opt => opt.MapFrom(d => d.FilePaths))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<FileSend, FileInPostModel>()
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(d => d.FilePath))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore())
                .ForMember(dest => dest.PostId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UserPostViewModel, UserPostModel>()
                .IncludeBase<BasePostViewModel, BasePostModel>()
                .ReverseMap();
            
            CreateMap<BasePostViewModel, BasePostModel>()
                .ReverseMap();
            

            CreateMap<GroupMember, User>().ReverseMap();

            CreateMap<GroupModel, Group>().ReverseMap(); 

            CreateMap<GroupMemberModel, GroupMember>().ReverseMap();

            CreateMap<GroupCreateViewModel, GroupModel>()
                .ForMember(dest => dest.GroupMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<GroupMemberViewModel, GroupMemberModel>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.RoleGroup, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<GroupViewModel, GroupModel>()
                .ForMember(dest => dest.GroupMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<BannedUserList, BannedUserInGroupModel>().ReverseMap();
            CreateMap<BannedUsersInGroupViewModel, BannedUserInGroupModel>().ReverseMap();

            CreateMap<LikePostModel, LikePost>()
                .ReverseMap();
            
            CreateMap<CommentPostModel, CommentPost>()
                .ReverseMap();
            
            CreateMap<CommentPostViewModel, CommentPostModel>()
                .ForMember(r => r.Post, opt => opt.Ignore())
                .ForMember(r => r.ToReplyComment, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<LikePostViewModel, LikePostModel>()
                .ForMember(r => r.Post, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
