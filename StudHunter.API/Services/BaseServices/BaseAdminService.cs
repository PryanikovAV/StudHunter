using StudHunter.API.ModelsDto.AdminDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.BaseServices;

public abstract class BaseAdminService(StudHunterDbContext context) : BaseService(context)
{
    protected static AdminDto MapToAdminDto(Administrator adminDto)
    {
        return new AdminDto
        {
            Id = adminDto.Id,
            Email = adminDto.Email,
            ContactEmail = adminDto.ContactEmail,
            ContactPhone = adminDto.ContactPhone,
            CreatedAt = adminDto.CreatedAt,
            IsDeleted = adminDto.IsDeleted,
            FirstName = adminDto.FirstName,
            LastName = adminDto.LastName,
            Patronymic = adminDto.Patronymic
        };
    }
}
