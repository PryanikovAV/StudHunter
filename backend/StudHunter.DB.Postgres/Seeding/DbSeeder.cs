using Bogus;
using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Seeding;

public class DbSeeder(StudHunterDbContext context, Func<string, string> hashPasswordFunc)
{

    public async Task SeedAsync()
    {
        // Если в БД уже есть сгенерированные пользователи (Bogus) -> выход
        if (await context.Users.AnyAsync(u => u.Email.Contains("@bogus.com")))
            return;

        foreach (var cityName in SeedDataConstants.Cities)
        {
            if (!await context.Cities.AnyAsync(c => c.Name == cityName))
            {
                context.Cities.Add(new City { Name = cityName });
            }
        }
        await context.SaveChangesAsync();

        foreach (var courseName in SeedDataConstants.Courses)
        {
            if (!await context.Courses.AnyAsync(c => c.Name == courseName))
            {
                context.Courses.Add(new Course { Name = courseName });
            }
        }
        await context.SaveChangesAsync();

        foreach (var specializationName in SeedDataConstants.Specialization)
        {
            if (!await context.Specializations.AnyAsync(s => s.Name == specializationName))
            {
                context.Specializations.Add(new Specialization { Name = specializationName });
            }
        }
        await context.SaveChangesAsync();

        foreach (var skillName in SeedDataConstants.AdditionalSkills)
        {
            if (!await context.AdditionalSkills.AnyAsync(s => s.Name == skillName))
            {
                context.AdditionalSkills.Add(new AdditionalSkill { Name = skillName });
            }
        }
        await context.SaveChangesAsync();

        var uniData = SeedDataConstants.Susu;

        var city = await context.Cities.FirstOrDefaultAsync(c => c.Name == uniData.CityName);

        if (city != null)
        {
            var university = await context.Universities.FirstOrDefaultAsync(u => u.Name == uniData.Name);
            if (university == null)
            {
                university = new University
                {
                    Name = uniData.Name,
                    Abbreviation = uniData.Abbreviation,
                    CityId = city.Id
                };
                context.Universities.Add(university);
                await context.SaveChangesAsync();
            }

            foreach (var facData in uniData.Faculties)
            {
                var faculty = await context.Faculties.FirstOrDefaultAsync(f => f.Name == facData.Name && f.UniversityId == university.Id);
                if (faculty == null)
                {
                    faculty = new Faculty
                    {
                        Name = facData.Name,
                        Abbreviation = facData.Abbreviation,
                        UniversityId = university.Id
                    };
                    context.Faculties.Add(faculty);
                    await context.SaveChangesAsync();
                }

                var createdDepartments = new List<Department>();
                foreach (var depName in facData.Departments)
                {
                    var department = await context.Departments.FirstOrDefaultAsync(d => d.Name == depName && d.FacultyId == faculty.Id);
                    if (department == null)
                    {
                        department = new Department { Name = depName, FacultyId = faculty.Id };
                        context.Departments.Add(department);
                    }
                    createdDepartments.Add(department);
                }
                await context.SaveChangesAsync();

                var defaultDepartment = createdDepartments.FirstOrDefault();
                if (defaultDepartment != null)
                {
                    foreach (var dirData in facData.Directions)
                    {
                        var direction = await context.StudyDirections.FirstOrDefaultAsync(sd => sd.Name == dirData.Name && sd.Code == dirData.Code);
                        if (direction == null)
                        {
                            direction = new StudyDirection
                            {
                                Name = dirData.Name,
                                Code = dirData.Code,
                                DepartmentId = defaultDepartment.Id
                            };
                            context.StudyDirections.Add(direction);
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        var chelyabinskId = await context.Cities.Where(c => c.Name == "Челябинск").Select(c => c.Id).FirstOrDefaultAsync();
        var susuId = await context.Universities.Where(u => u.Abbreviation == "ЮУрГУ").Select(u => u.Id).FirstOrDefaultAsync();

        var specializationIds = await context.Specializations.Select(s => s.Id).ToListAsync();
        var facultyIds = await context.Faculties.Select(f => f.Id).ToListAsync();
        var departmentIds = await context.Departments.Select(d => d.Id).ToListAsync();
        var directionIds = await context.StudyDirections.Select(sd => sd.Id).ToListAsync();
        var courseIds = await context.Courses.Select(c => c.Id).ToListAsync();
        var skillIds = await context.AdditionalSkills.Select(s => s.Id).ToListAsync();

        var locale = "ru";
        var defaultPasswordHash = hashPasswordFunc("password123");

        string[] malePatronymics = { "Иванович", "Петрович", "Сергеевич", "Александрович", "Алексеевич", "Дмитриевич", "Андреевич" };
        string[] femalePatronymics = { "Ивановна", "Петровна", "Сергеевна", "Александровна", "Алексеевна", "Дмитриевна", "Андреевна" };

        // ГЕНЕРАТОР РАБОТОДАТЕЛЕЙ
        int employerCounter = 1;
        var employerFaker = new Faker<Employer>(locale)
            .RuleFor(e => e.Id, f => Guid.NewGuid())
            .RuleFor(e => e.Email, f => $"employer{employerCounter++}@bogus.com")
            .RuleFor(e => e.PasswordHash, f => defaultPasswordHash)
            .RuleFor(e => e.RegistrationStage, f => User.AccountStatus.FullyActivated)
            .RuleFor(e => e.CityId, f => chelyabinskId)
            .RuleFor(e => e.ContactEmail, f => "test@email.ru")
            .RuleFor(e => e.ContactPhone, f => f.Phone.PhoneNumber("79#########"))

            .RuleFor(e => e.Name, f => $"ООО «{f.Commerce.Department()} {f.Commerce.ProductName()}»")
            .RuleFor(e => e.Description, f => f.Company.CatchPhrase() + ". " + f.Lorem.Paragraph())
            .RuleFor(e => e.Website, f => $"https://{f.Internet.DomainWord()}.ru")
            .RuleFor(e => e.SpecializationId, f => f.PickRandom(specializationIds))

            .RuleFor(e => e.OrganizationDetails, f => new OrganizationDetail
            {
                Id = Guid.NewGuid(),
                Inn = f.Random.Replace("##########"),
                Ogrn = f.Random.Replace("#############"),
                Kpp = f.Random.Replace("#########"),
                LegalAddress = f.Address.FullAddress(),
                ActualAddress = f.Address.FullAddress()
            });

        var employers = employerFaker.Generate(20);

        // ГЕНЕРАТОР ВАКАНСИЙ
        var vacancyFaker = new Faker<Vacancy>(locale)
            .RuleFor(v => v.Id, f => Guid.NewGuid())
            .RuleFor(v => v.EmployerId, (f, v) => f.PickRandom(employers).Id)
            .RuleFor(v => v.Title, f => f.Name.JobTitle())
            .RuleFor(v => v.Description, f => f.Lorem.Paragraphs(2))
            .RuleFor(v => v.Salary, f => f.Random.Number(30, 200) * 1000)
            .RuleFor(v => v.Type, f => f.PickRandom<Vacancy.VacancyType>());

        var vacancies = vacancyFaker.Generate(100);

        foreach (var vac in vacancies)
        {
            var randomSkillIds = new Faker().PickRandom(skillIds, 3).Distinct();
            foreach (var sId in randomSkillIds)
                vac.AdditionalSkills.Add(new VacancyAdditionalSkill { VacancyId = vac.Id, AdditionalSkillId = sId });

            var randomCourseIds = new Faker().PickRandom(courseIds, 2).Distinct();
            foreach (var cId in randomCourseIds)
                vac.Courses.Add(new VacancyCourse { VacancyId = vac.Id, CourseId = cId });
        }

        // ГЕНЕРАТОР СТУДЕНТОВ
        int studentCounter = 1;
        var studentFaker = new Faker<Student>(locale)
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.Email, f => $"student{studentCounter++}@bogus.com")
            .RuleFor(s => s.PasswordHash, f => defaultPasswordHash)
            .RuleFor(s => s.RegistrationStage, f => User.AccountStatus.FullyActivated)
            .RuleFor(s => s.CityId, f => chelyabinskId)
            .RuleFor(s => s.ContactEmail, f => "test@email.ru")
            .RuleFor(s => s.ContactPhone, f => f.Phone.PhoneNumber("79#########"))
            .RuleFor(s => s.Gender, f => f.PickRandom<Student.StudentGender>())
            .RuleFor(s => s.FirstName, (f, s) => f.Name.FirstName(s.Gender == Student.StudentGender.Male ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female))
            .RuleFor(s => s.LastName, (f, s) => f.Name.LastName(s.Gender == Student.StudentGender.Male ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female))

            .RuleFor(s => s.Patronymic, (f, s) => s.Gender == Student.StudentGender.Male ? f.PickRandom(malePatronymics) : f.PickRandom(femalePatronymics))
            .RuleFor(s => s.BirthDate, f => DateOnly.FromDateTime(f.Date.Past(10, DateTime.Now.AddYears(-18))))
            .RuleFor(s => s.Status, f => f.PickRandom<Student.StudentStatus>())

            .RuleFor(s => s.StudyPlan, (f, s) => new StudyPlan
            {
                Id = Guid.NewGuid(),
                StudentId = s.Id,
                UniversityId = susuId,
                FacultyId = f.PickRandom(facultyIds),
                DepartmentId = f.PickRandom(departmentIds),
                StudyDirectionId = f.PickRandom(directionIds),
                CourseNumber = f.Random.Int(1, 5),
                StudyForm = f.PickRandom<StudyPlan.StudyPlanForm>()
            });

        var students = studentFaker.Generate(50);

        foreach (var student in students)
        {
            var randomCourseIds = new Faker().PickRandom(courseIds, 2).Distinct();
            foreach (var cId in randomCourseIds)
            {

                student.StudyPlan!.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = student.StudyPlan.Id, CourseId = cId });
            }
        }

        // ГЕНЕРАТОР РЕЗЮМЕ
        var resumeFaker = new Faker<Resume>(locale)
            .RuleFor(r => r.Id, f => Guid.NewGuid())
            .RuleFor(r => r.Title, f => f.Name.JobTitle())
            .RuleFor(r => r.Description, f => f.Lorem.Paragraph());

        var resumes = new List<Resume>();

        foreach (var student in students)
        {
            var resume = resumeFaker.Generate();
            resume.StudentId = student.Id;

            var randomSkillIds = new Faker().PickRandom(skillIds, 4).Distinct();
            foreach (var sId in randomSkillIds)
            {
                resume.AdditionalSkills.Add(new ResumeAdditionalSkill { ResumeId = resume.Id, AdditionalSkillId = sId });
            }

            resumes.Add(resume);
        }

        await context.Employers.AddRangeAsync(employers);
        await context.Vacancies.AddRangeAsync(vacancies);
        await context.Students.AddRangeAsync(students);
        await context.Resumes.AddRangeAsync(resumes);

        await context.SaveChangesAsync();
    }
}