using System;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{
    public async Task<pagedList<MemberDto>> FilterMembers(UserParms userParms)
    {
        var query = context.Users.Include(x=> x.Photos).AsQueryable();

        if(userParms.Gender is not null)
        {
            query = query.Where(x=> x.Gender == userParms.Gender);
        }
        query = userParms.OrderBy switch
        {
            "created" => query.OrderByDescending(x=> x.Created),
            _ => query.OrderByDescending(x=> x.LastActive)
        };
        return await pagedList<MemberDto>
        .CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        , userParms.PageNumber, userParms.PageSize);
    }

    public async Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser)
    {
        var query = context.Users
                          .Where(x=> x.UserName == username)
                          .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                          .AsQueryable();
         if(isCurrentUser) query = query.IgnoreQueryFilters();
         return await query.SingleOrDefaultAsync();
    }

    public async Task<pagedList<MemberDto>> GetMembersAsync(UserParms userParams)
    {
        var query = context.Users.Include(x=> x.Photos).Where(x=> x.Photos.Any(x=> x.IsApproved)).AsQueryable();
        query = query.Where(x=> x.UserName != userParams.Currentusername);

        if (userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };

        return await pagedList<MemberDto>
        .CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        , userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUser?> GetUserByPhotoId(int photoId)
    {
        return await context.Users
        .Include(p => p.Photos)
        .IgnoreQueryFilters()
        .Where(p => p.Photos.Any(p => p.Id == photoId))
        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AppUser>> IUserRepository.GetAllAsync()
    {
        return await context.Users
        .Include(x => x.Photos)
        .ToListAsync();
    }

    async Task<AppUser?> IUserRepository.GetUserByIdAsync(int id)
    {
        return await context.Users
        .FindAsync(id);
    }

    async Task<AppUser?> IUserRepository.GetUserByUsernameAsync(string username)
    {
        return await context.Users
       .Include(x => x.Photos)
       .SingleOrDefaultAsync(x => x.UserName == username);
    }

    void IUserRepository.Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}
