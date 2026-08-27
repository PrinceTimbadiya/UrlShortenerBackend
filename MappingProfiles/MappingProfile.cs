using AutoMapper;
using UrlShortenerBackend.Models.Dtos;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<UserMaster, UserMaster>();
            CreateMap<UserCreateDto, UserMaster>();
            CreateMap<UserUpdateDto, UserMaster>();

            // URL
            CreateMap<UrlMaster, UrlResponseDto>();

            CreateMap<UrlCreateDto, UrlMaster>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ShortCode, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.UserMaster, opt => opt.Ignore());

            CreateMap<UrlUpdateDto, UrlMaster>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ShortCode, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.UserMaster, opt => opt.Ignore());

            // Credential
            CreateMap<CredentialMaster, CredentialResponseDto>()
                .ForMember(
                    dest => dest.SecretKey,
                    opt => opt.Ignore());
        }
    }
}