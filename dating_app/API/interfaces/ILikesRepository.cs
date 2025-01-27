using System;
using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLikeint(int sourceUserId, int torgetUserId );
    Task<pagedList<MemberDto>> GetUserLikes(LikesParams likesParams);

    Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
    void DeleteLike(UserLike like);

    void AddLike(UserLike like);
}
