using WebTest.Dtos.Friend;
using WebTest.Dtos.User;
using WebTest.Interfaces;
using WebTest.Interfaces.Repository;
using WebTest.Interfaces.Service;
using WebTest.Mappers;
using WebTest.Models;

namespace WebTest.Services;

public class FriendService(IFriendRepository friendRepository, IUserRepository userRepository) : IFriendService
{
    // Взять лист друзей
    public async Task<ServiceResponse<List<FriendResponseDto>>> GetFriendsList(string userId)   
    {
        var response = new ServiceResponse<List<FriendResponseDto>>();
            
        try
        {
            var friendships = await friendRepository.GetFriendsListAsync(userId);
            
            List<FriendResponseDto> responseDtos = new List<FriendResponseDto>();
            
            foreach (var friendship in friendships)
            {
                User user;
                
                if (friendship.FriendId == userId)
                {
                    
                    user = (await userRepository.GetByIdAsync(friendship.UserId))!;
                }
                else
                {
                    user = (await userRepository.GetByIdAsync(friendship.FriendId))!;
                }
                
                responseDtos.Add(FriendshipMapper.ToFullFriendResponseDto(friendship, user));
            }
            
            response.Data = responseDtos;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    // Поиск людей по имени
    public async Task<ServiceResponse<List<FriendResponseDto>>> SearchFriends(string query, string userId)
    {
        var response = new ServiceResponse<List<FriendResponseDto>>();
            
        try
        {
            var allUsers = await userRepository.SearchUsersAsync(query);
            var users = allUsers.Where(x => x.Id != userId);
            
            List<FriendResponseDto> responseDtos = new List<FriendResponseDto>();
            
            foreach (var user in users)
            {
                if (await friendRepository.IsFriendsAsync(user.Id, userId) 
                    || await friendRepository.IsFriendRequestExistsAsync(user.Id, userId))
                {
                    var friendship = await friendRepository.GetFriendAsync(userId,user.Id);
                    responseDtos.Add(FriendshipMapper.ToFullFriendResponseDto(friendship, user));
                }
                else
                {
                    responseDtos.Add(FriendshipMapper.ToFullNoFriendResponseDto(user));
                }
                
            }
            
            response.Data = responseDtos;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    // Отправить приглашение
    public async Task<ServiceResponse<bool>> SendFriendRequest(string fromUserId, string toUserId)
    {
        var response = new ServiceResponse<bool>();
            
        try
        {
            bool IsSend = await friendRepository.SendFriendRequestAsync(fromUserId, toUserId);
            
            response.Data = IsSend;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    // Согласится с приглашением
    public async Task<ServiceResponse<bool>> AcceptFriendRequest(string fromUserId, string toUserId)
    {
        var response = new ServiceResponse<bool>();
            
        try
        {
            bool IsSend = await friendRepository.AcceptFriendRequestAsync(fromUserId, toUserId);
            
            response.Data = IsSend;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    // Удалить друга
    public async Task<ServiceResponse<bool>> DeleteFriend(string userId, string friendId)
    {
        var response = new ServiceResponse<bool>();
            
        try
        {
            bool IsSend = await friendRepository.DeleteFriendAsync(userId, friendId);
            
            response.Data = IsSend;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }
}