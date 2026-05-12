using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Seeding;

public class DbSeeder(StudHunterDbContext context)
{
    private readonly StudHunterDbContext _context = context;

    public async Task SeedAsync()
    {
        foreach (var cityName in SeedDataConstants.Cities)
        {
            if (!await _context.Cities.AnyAsync(c => c.Name == cityName))
            {
                _context.Cities.Add(new City { Name = cityName });
            }
        }
        await _context.SaveChangesAsync();

        foreach (var courseName in SeedDataConstants.Courses)
        {
            if (!await _context.Courses.AnyAsync(c => c.Name == courseName))
            {
                _context.Courses.Add(new Course { Name = courseName });
            }
        }
        await _context.SaveChangesAsync();

        foreach (var specializationName in SeedDataConstants.Specialization)
        {
            if (!await _context.Specializations.AnyAsync(s => s.Name == specializationName))
            {
                _context.Specializations.Add(new Specialization { Name = specializationName });
            }
        }
        await _context.SaveChangesAsync();

        foreach (var skillName in SeedDataConstants.AdditionalSkills)
        {
            if (!await _context.AdditionalSkills.AnyAsync(s => s.Name == skillName))
            {
                _context.AdditionalSkills.Add(new AdditionalSkill { Name = skillName });
            }
        }
        await _context.SaveChangesAsync();

        var uniData = SeedDataConstants.Susu;

        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == uniData.CityName);
        
        if (city != null)
        {
            var university = await _context.Universities.FirstOrDefaultAsync(u => u.Name == uniData.Name);
            if (university == null)
            {
                university = new University
                {
                    Name = uniData.Name,
                    Abbreviation = uniData.Abbreviation,
                    CityId = city.Id
                };
                _context.Universities.Add(university);
                await _context.SaveChangesAsync();
            }

            foreach (var facData in uniData.Faculties)
            {
                var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.Name == facData.Name && f.UniversityId == university.Id);
                if (faculty == null)
                {
                    faculty = new Faculty
                    {
                        Name = facData.Name,
                        Abbreviation = facData.Abbreviation,
                        UniversityId = university.Id
                    };
                    _context.Faculties.Add(faculty);
                    await _context.SaveChangesAsync();
                }

                var createdDepartments = new List<Department>();
                foreach (var depName in facData.Departments)
                {
                    var department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == depName && d.FacultyId == faculty.Id);
                    if (department == null)
                    {
                        department = new Department { Name = depName, FacultyId = faculty.Id };
                        _context.Departments.Add(department);
                    }
                    createdDepartments.Add(department);
                }
                await _context.SaveChangesAsync();

                var defaultDepartment = createdDepartments.FirstOrDefault();
                if (defaultDepartment != null)
                {
                    foreach (var dirData in facData.Directions)
                    {
                        var direction = await _context.StudyDirections.FirstOrDefaultAsync(sd => sd.Name == dirData.Name && sd.Code == dirData.Code);
                        if (direction == null)
                        {
                            direction = new StudyDirection
                            {
                                Name = dirData.Name,
                                Code = dirData.Code,
                                DepartmentId = defaultDepartment.Id
                            };
                            _context.StudyDirections.Add(direction);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}