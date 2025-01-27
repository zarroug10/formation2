using System;
using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;
using CloudinaryDotNet;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {//inside this constructor we create our mapping profile
        CreateMap<AppUser,MemberDto>()
            .ForMember(d => d.Age , o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
            .ForMember(d => d.PhotoUrl, // the member that need to map
            o => o.MapFrom(//source to map from
                s => s.Photos.FirstOrDefault(//lamda expression to see if the url is empty
                    x => x.IsMain)!.Url));                                     
        CreateMap<Photo,PhotoDTO>();            //createMap<from,to>
        CreateMap<MemberUpadteDTO,AppUser>();
        CreateMap<AcountDto,AppUser>();
        CreateMap<string,DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        CreateMap<Message,MessageDTO>()
            .ForMember(d=> d.SenderPhotoUrl,o =>o
                .MapFrom(s=> s.Sender.Photos
                    .FirstOrDefault(x=> x.IsMain)!.Url))
            .ForMember(d=> d.RecipientPhotoUrl,o =>o
                .MapFrom(s=> s.Recipient.Photos
                    .FirstOrDefault(x=> x.IsMain)!.Url));
        CreateMap<DateTime,DateTime>().ConvertUsing(d=> DateTime.SpecifyKind(d,DateTimeKind.Utc));
        CreateMap<DateTime?,DateTime?>().ConvertUsing(d=> d.HasValue 
            ? DateTime.SpecifyKind(d.Value,DateTimeKind.Utc): null);
    }
}
