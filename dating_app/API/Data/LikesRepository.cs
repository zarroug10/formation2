using System;

using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using API.DTO;
using API.Entities;
using API.Helpers;
using API.interfaces;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
        .Where(x => x.SourceUserId == currentUserId)
        .Select(x => x.TargetUserId)
        .ToListAsync();
    }

    public async Task<UserLike?> GetUserLikeint(int sourceUserId, int torgetUserId)
    {
        return await context.Likes
        .FindAsync(sourceUserId, torgetUserId);
    }

    public async Task<pagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;

        switch (likesParams.Predecate)
        {
            case "liked":
               query = likes
                .Where(x => x.SourceUserId == likesParams.UserId)
                .Select(x => x.TorgetUser)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            case "Likedby":
                  query = likes
                .Where(x => x.TargetUserId == likesParams.UserId)
                .Select(x => x.SourceUser)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            default:

            var  likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

           query = likes
            .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
            .Select(x => x.SourceUser)
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
            break;
         }
         return await pagedList<MemberDto>.CreateAsync(query,likesParams.PageNumber,likesParams.PageSize);
    }
}
