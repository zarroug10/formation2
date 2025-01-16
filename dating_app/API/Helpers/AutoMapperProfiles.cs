using System;
using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;

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
                x => x.IsMain)!.Url));                                     //createMap<from,to>
        CreateMap<Photo,PhotoDTO>();            //createMap<from,to>
        CreateMap<MemberUpadteDTO,AppUser>();
        CreateMap<AcountDto,AppUser>();
        CreateMap<string,DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
    }
}
