using SocialNetwork.BLL.Models;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IChatService : IBaseService<ChatModel>
{
    Task<ChatModel> CreateDirectChat(int userId, int user2Id, CancellationToken cancellationToken = default);
    Task<ChatModel> CreateGroupChat(int userId, ChatModel chatModel, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatNotificationModel>> AddUsers(int userId, int chatId, List<int> userIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatNotificationModel>> DelMembers(int userId, int chatId, List<int> userIds, CancellationToken cancellationToken = default);
    
    Task<ChatModel> EditChat(int userId, int chatId, ChatModel chatModel, CancellationToken cancellationToken = default);
    Task DeleteChat(int userId, int chatId, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<ChatModel>> FindChatByName(int userId, PaginationModel pagination, string chatName, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<ChatModel>> GetAllChats(int userId, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<ChatModel>> GetAllChats(int userId, PaginationModel? pagination, CancellationToken cancellationToken = default);

    Task AddRole(int userId, int chatId, RoleModel roleModel, CancellationToken cancellationToken = default);
    Task<RoleModel> GetRoleById(int userId, int chatId, int roleId, CancellationToken cancellationToken = default);
    Task DelRole(int userId, int chatId, int roleId, CancellationToken cancellationToken = default);
    Task<RoleModel> EditRole(int userId, int chatId, int roleId, RoleModel roleModel, CancellationToken cancellationToken = default);

    Task SetRole(int userId, int chatId, int roleId, List<int> userIds, CancellationToken cancellationToken = default);
    Task UnSetRole(int userId, int chatId, int roleId, List<int> userIds, CancellationToken cancellationToken = default);

    Task<PaginationResultModel<RoleModel>> GetAllChatRoles(int userId,PaginationModel pagination, int chatId, CancellationToken cancellationToken = default);

    Task<PaginationResultModel<ChatMemberModel>> GetChatMembers(int userId, PaginationModel pagination, int chatId, int roleId, CancellationToken cancellationToken = default);
    
    Task<PaginationResultModel<ChatMemberModel>> GetChatMembers(int userId, PaginationModel pagination, int chatId, CancellationToken cancellationToken = default);
    Task<List<RoleModel>> EditRolesRank(int userId, int chatId, List<RoleModel> roleModels, CancellationToken cancellationToken = default);

    Task LeaveChat(int userId, int chatId, CancellationToken cancellationToken = default);
    Task MakeHost(int userId, int chatId, int user2Id, CancellationToken cancellationToken = default);
    Task<bool> UserInChatCheck(int userId, int chatId, CancellationToken cancellationToken = default);
}