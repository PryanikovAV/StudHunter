using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;
// TODO: добавить пагинацию
public interface IAdminDictionariesService : IDictionariesService
{
    Task<Result<LookupDto>> CreateSkillAsync(CreateSkillDto dto);
    Task<Result<LookupDto>> UpdateSkillAsync(Guid id, UpdateSkillDto dto);
    Task<Result<bool>> DeleteSkillAsync(Guid id);
}

public class AdminDictionariesService(StudHunterDbContext context, IRegistrationManager registrationManager)
    : DictionariesService(context, registrationManager), IAdminDictionariesService
{
    public async Task<Result<LookupDto>> CreateSkillAsync(CreateSkillDto dto)
    {
        var normalizedName = dto.Name.Trim();

        if (await _context.AdditionalSkills.AnyAsync(s => s.Name.ToLower() == normalizedName.ToLower()))
            return Result<LookupDto>.Failure(ErrorMessages.AlreadyExists(nameof(AdditionalSkill)), StatusCodes.Status400BadRequest);

        var skill = new AdditionalSkill { Name = normalizedName };
        _context.AdditionalSkills.Add(skill);

        var result = await SaveChangesAsync<AdditionalSkill>();
        return result.IsSuccess
            ? Result<LookupDto>.Success(new LookupDto(skill.Id, skill.Name))
            : Result<LookupDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<LookupDto>> UpdateSkillAsync(Guid id, UpdateSkillDto dto)
    {
        var skill = await _context.AdditionalSkills.FindAsync(id);
        if (skill == null)
            return Result<LookupDto>.Failure(
                ErrorMessages.EntityNotFound(nameof(AdditionalSkill)),
                StatusCodes.Status404NotFound);

        var normalizedName = dto.Name.Trim();

        if (await _context.AdditionalSkills.AnyAsync(s => s.Name.ToLower() == normalizedName.ToLower() && s.Id != id))
            return Result<LookupDto>.Failure(
                ErrorMessages.AlreadyExists(nameof(AdditionalSkill)), StatusCodes.Status400BadRequest);

        skill.Name = normalizedName;

        var result = await SaveChangesAsync<AdditionalSkill>();
        return result.IsSuccess
            ? Result<LookupDto>.Success(new LookupDto(skill.Id, skill.Name))
            : Result<LookupDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<bool>> DeleteSkillAsync(Guid id)
    {
        var skill = await _context.AdditionalSkills.FindAsync(id);
        if (skill == null)
            return Result<bool>.Failure(ErrorMessages.EntityNotFound(nameof(AdditionalSkill)), StatusCodes.Status404NotFound);

        _context.AdditionalSkills.Remove(skill);
        return await SaveChangesAsync<AdditionalSkill>();
    }
}