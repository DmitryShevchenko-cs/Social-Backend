using SocialNetwork.BLL.Models;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IFriendshipService : IBaseService<FriendshipModel>
{
    Task AddFriendshipAsync(int userId, int firendId, CancellationToken cancellationToken = default);
    Task DeleteFriendshipAsync(int userId, int firendId, CancellationToken cancellationToken = default);

    Task<PaginationResultModel<UserModel>> GetAllFriends(int userId, PaginationModel pagination, string? sortType, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<UserModel>> GetAllFriends(int userId, PaginationModel pagination, CancellationToken cancellationToken = default);
    
    // we can pass NameSurname like "Name Surname", "Surname Name", "Name", "Surname" and find all who has something similar
    Task<PaginationResultModel<UserModel>> FindFriendByNameSurname(int userId, PaginationModel pagination, string nameSurname, string? sortType, CancellationToken cancellationToken = default);
    Task<PaginationResultModel<UserModel>> FindFriendByNameSurname(int userId, PaginationModel pagination, string nameSurname, CancellationToken cancellationToken = default);
    
    Task<UserModel> FindFriendByEmail(int userId,string friendEmail, CancellationToken cancellationToken = default);
    Task<bool> IsFriends(int userId, int user2Id, CancellationToken cancellationToken = default);
}