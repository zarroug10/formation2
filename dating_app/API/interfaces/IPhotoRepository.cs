using System;
using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.interfaces;

public interface IPhotoRepository
{
    public Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
    public Task<Photo?> GetPhotoById(int Photoid);
    void RemovePhoto(Photo photo);
}
