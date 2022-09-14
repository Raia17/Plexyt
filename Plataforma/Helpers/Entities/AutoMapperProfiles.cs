using AutoMapper;
using Plataforma.Dtos.Account;
using Plataforma.Dtos.Media;
using Plataforma.Dtos.Users;
using Plataforma.Models.Identity;
using Plataforma.Models.Media;


namespace Plataforma.Helpers.Entities;

public class AutoMapperProfiles : Profile {

    public AutoMapperProfiles() {
        CreateMap<PersonalSettingsDto, User>();
        CreateMap<User, PersonalSettingsDto>();
        CreateMap<UserDto, User>();
        CreateMap<User, UserDto>();

        CreateMap<File, UploadedFile>().ConstructUsing(i => new UploadedFile() { FileModel = i });
        CreateMap<UploadedFile, File>(MemberList.None);
    }
}