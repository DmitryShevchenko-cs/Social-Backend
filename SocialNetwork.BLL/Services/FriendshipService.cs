using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Models.Enums;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BLL.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ILogger<FriendshipService> _logger;
    private readonly IMapper _mapper;

    public FriendshipService(IUserService userService, IUserRepository userRepository,
        IFriendshipRepository friendshipRepository, ILogger<FriendshipService> logger, IMapper mapper)
    {
        _userService = userService;
        _userRepository = userRepository;
        _friendshipRepository = friendshipRepository;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<FriendshipModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var friendDb = await _friendshipRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(friendDb, new FriendNotFoundException("Friend not found"));
        var friendModel = _mapper.Map<FriendshipModel>(friendDb);
        return friendModel;
    }

    public async Task AddFriendshipAsync(int userId, int friendId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var user2Model = await _userService.GetByIdAsync(friendId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException("User not found"));
        _logger.LogAndThrowErrorIfNull(user2Model, new UserNotFoundException("User not found"));
        
        if (userModel!.Id != user2Model!.Id)
        {
            var friendship = new Friendship()
            {
                UserId = userModel!.Id,
                FriendId = user2Model!.Id,
            };
            await _friendshipRepository.CreateFriendshipAsync(friendship, cancellationToken);
        }
        else
        {
            _logger.LogError("You cant add to friend yourself");
            throw new FriendNotFoundException($"Friends not found");
        }
    }

    public async Task DeleteFriendshipAsync(int userId, int friendId, CancellationToken cancellationToken = default)
    {
        var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
        var user2Model = await _userService.GetByIdAsync(friendId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException("User not found"));
        _logger.LogAndThrowErrorIfNull(user2Model, new UserNotFoundException("User not found"));
        var friendship = new Friendship()
        {
            UserId = userModel!.Id,
            FriendId = user2Model!.Id,
        };
        await _friendshipRepository.DeleteFriendsAsync(friendship, cancellationToken);
    }

    public async Task<PaginationResultModel<UserModel>> GetAllFriends(int userId, PaginationModel pagination, string? sortType, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException("User not found"));

        SortType? parsedSortType = null;
        if (sortType is not null)
        {
            parsedSortType = SortTypeConverter.ConvertFromString(sortType);
        }
        
        var friendsQuery = _friendshipRepository
            .GetAllFriendsByUserId(userDb!.Id);
        
        var paginationModel = new PaginationResultModel<UserModel>
        {
            TotalDbItems = friendsQuery.Select(f => f.UserId == userDb.Id ? f.FriendUser : f.User).Count()
        };
        
        
        if (parsedSortType.HasValue)
        {
            switch (parsedSortType)
            {
                case SortType.RecentlyAdded:
                    friendsQuery = friendsQuery.OrderByDescending(r => r.Id);
                    break;
                case SortType.Online:
                    friendsQuery = friendsQuery.OrderByDescending(u => u.FriendUser.OnlineStatus).ThenByDescending(u => u.User.OnlineStatus);
                    break;
                case SortType.FirstName:
                    friendsQuery = friendsQuery.OrderBy(u => u.FriendUser.Profile.Name).ThenBy(u => u.User.Profile.Name);
                    break;
                case SortType.LastName:
                    friendsQuery = friendsQuery.OrderBy(u => u.FriendUser.Profile.Surname).ThenBy(u => u.User.Profile.Surname);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parsedSortType), parsedSortType, "Unhandled sortType");
            }
        }
    
        var users = await friendsQuery
            .Select(f => f.UserId == userDb.Id ? f.FriendUser : f.User)
            .Pagination(pagination.CurrentPage, pagination.PageSize)
            .ToListAsync(cancellationToken);

        var userModels = _mapper.Map<IEnumerable<UserModel>>(users);

        paginationModel.Data = userModels;
        paginationModel.CurrentPage = pagination.CurrentPage;
        paginationModel.PageSize = pagination.PageSize;
        paginationModel.TotalItems = users.Count;
        
        return paginationModel;
    }

    public async Task<PaginationResultModel<UserModel>> GetAllFriends(int userId, PaginationModel pagination, CancellationToken cancellationToken = default)
    {
        return await GetAllFriends(userId, pagination, null, cancellationToken);
    }

    //like
    public async Task<PaginationResultModel<UserModel>> FindFriendByNameSurname(int userId,PaginationModel pagination,string nameSurname, string? sortType,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException("User not found"));
        
        string[] parts = nameSurname.Split();

        IEnumerable<User>? matchingUsers = null;
        if (parts.Length == 1)
        {
            string name = parts[0].ToLower();
            matchingUsers = await _friendshipRepository.GetAllFriendsByUserId(userDb!.Id)
                .Where(f => f.User.Profile.Name.ToLower().StartsWith(name)
                            || f.User.Profile.Surname.ToLower().StartsWith(name)
                            || f.FriendUser.Profile.Name.ToLower().StartsWith(name)
                            || f.FriendUser.Profile.Surname.ToLower().StartsWith(name))
                .Select(f => f.UserId == userDb.Id ? f.FriendUser : f.User)
                .Pagination(pagination.CurrentPage, pagination.PageSize)
                .ToListAsync(cancellationToken);
        }
        else if (parts.Length == 2)
        {
            string firstName = parts[0].ToLower();
            string lastName = parts[1].ToLower();

            matchingUsers = await _friendshipRepository.GetAllFriendsByUserId(userDb!.Id)
                .Where(f => ((f.User.Profile.Name.ToLower().StartsWith(firstName)
                              && f.User.Profile.Surname.ToLower().StartsWith(lastName))
                             || f.User.Profile.Name.ToLower().StartsWith(lastName)
                             && f.User.Profile.Surname.ToLower().StartsWith(firstName))
                            ||
                            (f.FriendUser.Profile.Name.ToLower().StartsWith(firstName)
                             && f.FriendUser.Profile.Surname.ToLower().StartsWith(lastName))
                            || f.FriendUser.Profile.Name.ToLower().StartsWith(lastName)
                            && f.FriendUser.Profile.Surname.ToLower().StartsWith(firstName))
                .Select(f => f.UserId == userDb.Id ? f.FriendUser : f.User)
                .Pagination(pagination.CurrentPage, pagination.PageSize)
                .ToListAsync(cancellationToken);
        }

        var friends = _mapper.Map<IEnumerable<UserModel>>(matchingUsers);

        var paginationModel = new PaginationResultModel<UserModel>
        {
            Data = friends,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = friends.Count(),
            TotalDbItems = _friendshipRepository.GetAllFriendsByUserId(userDb!.Id).Select(f => f.UserId == userDb.Id ? f.FriendUser : f.User).Count()
        };

        return paginationModel;
    }

    public async Task<PaginationResultModel<UserModel>> FindFriendByNameSurname(int userId, PaginationModel pagination, string nameSurname,
        CancellationToken cancellationToken = default)
    {
        return await FindFriendByNameSurname(userId, pagination, nameSurname, null, cancellationToken);
    }

    public async Task<UserModel> FindFriendByEmail(int userId, string friendEmail,
        CancellationToken cancellationToken = default)
    {
        var user2Model = await _userService.GetUserByEmail(friendEmail, cancellationToken);
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        
        _logger.LogAndThrowErrorIfNull(user2Model, new UserNotFoundException("User not found"));
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException("User not found"));
        var friend = await _friendshipRepository.GetAll()
            .Where(f => f.UserId == userDb!.Id && f.FriendUser.Profile.Email == friendEmail)
            .Select(f => f.FriendUser)
            .Union(_friendshipRepository.GetAll()
                .Where(f => f.FriendId == userDb!.Id && f.User.Profile.Email == friendEmail)
                .Select(f => f.User))
            .SingleOrDefaultAsync(cancellationToken);

        
        _logger.LogAndThrowErrorIfNull(friend, new FriendNotFoundException("Friend not found"));

        var userModel = _mapper.Map<UserModel>(friend);
        return userModel;
    }

    public async Task<bool> IsFriends(int userId, int user2Id, CancellationToken cancellationToken = default)
    {
        var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var user2Db = await _userRepository.GetByIdAsync(user2Id, cancellationToken);
        
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException("User not found"));
        _logger.LogAndThrowErrorIfNull(user2Db, new UserNotFoundException("User not found"));

        var friendship = await _friendshipRepository
            .GetAllFriendsByUserId(userId)
            .Where(u => u.UserId == user2Id || u.FriendId == user2Id)
            .SingleOrDefaultAsync(cancellationToken);
        if(friendship is null)
            return false;
        return true;
    }
}