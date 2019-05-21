using AutoMapper;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            //CreateMap<User, UserForListDTO>(); //user is mapped to userForListDTO

            //We need to provide configuration to userForListDto about age and PhotoUrl properties
            CreateMap<User, UserForListDTO>()
                //destination is photourl and it is maped from photoUrl of Photos inside user
                .ForMember(destination => destination.PhotoUrl, opt =>
                 {
                     opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.isMain).Url);
                 })
                 .ForMember(dest => dest.Age, opt =>
                  {
                      //we dont have age property to map, we need to costum calculate it
                      opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                  });
            CreateMap<User, UserForDetailedDTO>()
                 //destination
                .ForMember(destination => destination.PhotoUrl, opt =>
                {
                    //source
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.isMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    //we dont have age property to map, we need to costum calculate it
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });

           CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDTO, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
        }
    }
}
