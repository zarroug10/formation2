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
    public async Task<MemberDto?> GetMemberAsync(string username)
    {
       return await context.Users
       .Where(u => u.UserName == username)
       .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
       .SingleOrDefaultAsync();
    }

    public async Task<pagedList<MemberDto>> GetMembersAsync(UserParms userParams)
    {
        var query = context.Users.AsQueryable();
        query = query.Where(x => x.UserName != userParams.Currentusername );

        if (userParams.Gender != null)
        {
            query = query.Where( x=>x.Gender == userParams.Gender);
        }
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
        query = userParams.OrderBy switch 
        {
            "created"=> query.OrderByDescending(x=> x.Created),
            _ => query.OrderByDescending(x=> x.LastActive)
        };

        return await pagedList<MemberDto>
        .CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        ,userParams.PageNumber,userParams.PageSize);
    }

    async Task<IEnumerable<AppUser>> IUserRepository.GetAllAsync()
    {
        return await context.Users
        .Include(x=>x.Photos)
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
       .Include(x=>x.Photos)
       .SingleOrDefaultAsync(x => x.UserName == username);
    }

    async Task<bool> IUserRepository.SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0 ;
    }

    void IUserRepository.Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}
