using System;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using API.DTO;
using API.Entities;
using API.interfaces;

namespace API.Data;

public class PhotoRepository(DataContext context) : IPhotoRepository
{
    public async Task<Photo?> GetPhotoById(int id)
    {
        return await context.Photos
        .IgnoreQueryFilters()
        .SingleOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        return await context.Photos
        .IgnoreQueryFilters()
        .Where(p => p.IsApproved == false)
        .Select(u => new PhotoForApprovalDto
        {
            Id = u.Id,
            Username = u.AppUser.UserName,
            Url = u.Url,
            IsApproved = u.IsApproved
        }).ToListAsync();
    }
    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
    
}
