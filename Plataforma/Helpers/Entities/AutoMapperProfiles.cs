using AutoMapper;
using Plataforma.Dtos.Account;
using Plataforma.Dtos.Media;
using Plataforma.Dtos.Users;
using Plataforma.Dtos.Work;
using Plataforma.Models.Identity;
using Plataforma.Models.Media;
using Plataforma.Models.Work;

namespace Plataforma.Helpers.Entities;

public class AutoMapperProfiles : Profile {

    public AutoMapperProfiles() {
        CreateMap<PersonalSettingsDto, User>();
        CreateMap<User, PersonalSettingsDto>();
        CreateMap<UserDto, User>();
        CreateMap<User, UserDto>();

        #region Work
        CreateMap<Client, ClientDto>();
        CreateMap<ClientDto, Client>();

        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeDto, Employee>();

        CreateMap<Vehicle, VehicleDto>();
        CreateMap<VehicleDto, Vehicle>();

        CreateMap<Material, MaterialDto>();
        CreateMap<MaterialDto, Material>();

        CreateMap<WorkSheet, WorkSheetDto>();
        CreateMap<WorkSheetDto, WorkSheet>();
        #endregion

        CreateMap<File, UploadedFile>().ConstructUsing(i => new UploadedFile() { FileModel = i });
        CreateMap<UploadedFile, File>(MemberList.None);
    }
}