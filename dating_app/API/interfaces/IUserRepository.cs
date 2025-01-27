using System;
using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.interfaces;

public interface IUserRepository
{
    void Update (AppUser user);
    Task <IEnumerable<AppUser>> GetAllAsync ();
    Task <AppUser?> GetUserByIdAsync (int id);
    Task <AppUser?> GetUserByUsernameAsync (string username);
    Task <pagedList<MemberDto>> GetMembersAsync (UserParms userParms);
    Task <MemberDto?> GetMemberAsync (string username, bool isCurrentUser);
    Task<AppUser?> GetUserByPhotoId(int photoId);
    Task<pagedList<MemberDto>> FilterMembers(UserParms userParms);
}
