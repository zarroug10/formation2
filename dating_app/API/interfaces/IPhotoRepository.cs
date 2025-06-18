using System;

using Microsoft.AspNetCore.Mvc;

using API.DTO;
using API.Entities;

namespace API.interfaces;

public interface IPhotoRepository
{
    public Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
    public Task<Photo?> GetPhotoById(int Photoid);
    void RemovePhoto(Photo photo);
}
