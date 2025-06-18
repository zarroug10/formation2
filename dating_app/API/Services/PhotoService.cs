using System;

using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using var stream = file.OpenReadStream() ;

using API.Helpers;
using API.interfaces;

namespace API.Services;

public class PhotoService : IPhotoService
{

    private readonly Cloudinary _cloudinary ;
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResults = new ImageUploadResult() ;
        if (file.Length > 0)
        {
var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName,stream),
                Transformation = new Transformation() 
                .Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder ="da-net8"
            };

            uploadResults = await _cloudinary.UploadAsync(uploadParams) ;
        }
        return uploadResults;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return  await _cloudinary.DestroyAsync(deleteParams);
    }
}
