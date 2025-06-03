using JasperFx.Core;
using Mapster;
using Module.User.Domain.Entity;
using UserDto = Shared.DTO.Dtos.User;

namespace Module.User.Presentation.Mappers
{
    public static class UserMappingConfig
    {
        public static void RegisterUserMappings(this TypeAdapterConfig config)
        {
            // Map UserEntity to UserDto
            config.NewConfig<UserEntity, UserDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Phone, src => src.Phone)
                .Map(dest => dest.Password, src => src.Password)
                .Map(dest => dest.RefreshToken, src => src.RefreshToken)
                .Map(dest => dest.RefreshTokenExpiryTime, src => src.RefreshTokenExpiryTime)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Dni, src => src.Dni)
                .Map(dest => dest.RoleId, src => src.Role!.Id)
                .Map(dest => dest.RoleName, src => src.Role!.Name);
        }
    }
}
