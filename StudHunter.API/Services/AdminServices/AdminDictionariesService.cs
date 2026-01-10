using Microsoft.EntityFrameworkCore;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;
// TODO: добавить пагинацию
public interface IAdminDictionariesService : IDictionariesService
{
    Task<Result<List<SkillDto>>> GetAllSkillsAsync();
    Task<Result<SkillDto>> CreateSkillAsync(CreateSkillDto dto);
    Task<Result<SkillDto>> UpdateSkillAsync(Guid id, UpdateSkillDto dto);
    Task<Result<bool>> DeleteSkillAsync(Guid id);
}

public class AdminDictionariesService(StudHunterDbContext context) : DictionariesService(context), IAdminDictionariesService
{
    public async Task<Result<List<SkillDto>>> GetAllSkillsAsync()
    {
        var skills = await _context.AdditionalSkills
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SkillDto(s.Id, s.Name))
            .ToListAsync();

        return Result<List<SkillDto>>.Success(skills);
    }

    public async Task<Result<SkillDto>> CreateSkillAsync(CreateSkillDto dto)
    {
        var normalizedName = dto.Name.Trim();

        if (await _context.AdditionalSkills.AnyAsync(s => s.Name.ToLower() == normalizedName.ToLower()))
            return Result<SkillDto>.Failure(ErrorMessages.AlreadyExists(nameof(AdditionalSkill)), StatusCodes.Status400BadRequest);

        var skill = new AdditionalSkill { Name = normalizedName };
        _context.AdditionalSkills.Add(skill);

        var result = await SaveChangesAsync<AdditionalSkill>();
        return result.IsSuccess
            ? Result<SkillDto>.Success(new SkillDto(skill.Id, skill.Name))
            : Result<SkillDto>.Failure(result.ErrorMessage!);
    }

    public async Task<Result<SkillDto>> UpdateSkillAsync(Guid id, UpdateSkillDto dto)
    {
        var skill = await _context.AdditionalSkills.FindAsync(id);
        if (skill == null)
            return Result<SkillDto>.Failure(
                ErrorMessages.EntityNotFound(nameof(AdditionalSkill)),
                StatusCodes.Status404NotFound);

        var normalizedName = dto.Name.Trim();

        if (await _context.AdditionalSkills.AnyAsync(s => s.Name.ToLower() == normalizedName.ToLower() && s.Id != id))
            return Result<SkillDto>.Failure(
                ErrorMessages.AlreadyExists(nameof(AdditionalSkill)), StatusCodes.Status400BadRequest);

        skill.Name = normalizedName;

        var result = await SaveChangesAsync<AdditionalSkill>();
        return result.IsSuccess
            ? Result<SkillDto>.Success(new SkillDto(skill.Id, skill.Name))
            : Result<SkillDto>.Failure(result.ErrorMessage!);
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