using System.Linq;
using AutoMapper;
using DateApp.Core.DataModels;
using DateApp.Core.EntityModels;

namespace DateApp.DAL.Mappers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserListDm>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => { opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url); })
                .ForMember(dest=> dest.Age, opt => { opt.MapFrom(x => x.DateOfBirth.CalculateAge()); });
            CreateMap<User, UserDetailsDm>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => { opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url); })
                .ForMember(dest => dest.Age, opt => { opt.MapFrom(x => x.DateOfBirth.CalculateAge()); });
            CreateMap<Photo, PhotosDm>();
            CreateMap<UserUpdateDm, User>();
            CreateMap<UserPhotoDm, Photo>();
            CreateMap<Photo, UserPhotoDm>();
            CreateMap<UserDm, User>();
            CreateMap<User, UserDm>();
            CreateMap<UserDetailsDm, User>();
            CreateMap<MessageForCreationDm, Message>();
            CreateMap<Message, MessageForCreationDm>()
                .ForMember(dest => dest.SenderKnownAs, opt => { opt.MapFrom(x => x.Sender.KnownAs); })
                .ForMember(dest => dest.SenderPhotoUrl,
                    opt => { opt.MapFrom(x => x.Sender.Photos.FirstOrDefault(y => y.IsMain).Url); });

            CreateMap<Message, MessageToReturnDm>()
                .ForMember(dest => dest.SenderKnownAs, opt => { opt.MapFrom(x => x.Sender.KnownAs); })
                .ForMember(dest => dest.SenderPhotoUrl,
                    opt => { opt.MapFrom(x => x.Sender.Photos.FirstOrDefault(y => y.IsMain).Url); })
                .ForMember(dest => dest.RecipientKnownAs, opt => { opt.MapFrom(x => x.Recipient.KnownAs); })
                .ForMember(dest => dest.RecipientPhotoUrl,
                    opt => { opt.MapFrom(x => x.Recipient.Photos.FirstOrDefault(y => y.IsMain).Url); });
        }
    }
}
