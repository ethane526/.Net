using Sabio.Models;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Friends;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface IFriendService
    {
        int AddV3(FriendAddRequestV3 friendModel, int currentUserId);
        void DeleteV3(int Id);
        List<FriendV3> GetAllV3();
        FriendV3 GetV3(int id);
        Paged<FriendV3> PaginationV3(int PageIndex, int PageSize);
        Paged<FriendV3> SearchV3(int PageIndex, int PageSize, string Query);
        void UpdateV3(FriendUpdateRequestV3 friendModel, int currentUserId);
    }
}