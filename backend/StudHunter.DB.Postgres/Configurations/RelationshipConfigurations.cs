using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class ResumeAdditionalSkillConfiguration : IEntityTypeConfiguration<ResumeAdditionalSkill>
{
    public void Configure(EntityTypeBuilder<ResumeAdditionalSkill> builder)
    {
        builder.HasKey(ras => new { ras.ResumeId, ras.AdditionalSkillId });
        builder.HasOne(ras => ras.Resume).WithMany(r => r.AdditionalSkills).HasForeignKey(ras => ras.ResumeId);
        builder.HasOne(ras => ras.AdditionalSkill).WithMany(s => s.ResumeAdditionalSkills)
               .HasForeignKey(ras => ras.AdditionalSkillId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StudyPlanCourseConfiguration : IEntityTypeConfiguration<StudyPlanCourse>
{
    public void Configure(EntityTypeBuilder<StudyPlanCourse> builder)
    {
        builder.HasKey(spc => new { spc.StudyPlanId, spc.CourseId });
        builder.HasOne(spc => spc.StudyPlan).WithMany(sp => sp.StudyPlanCourses).HasForeignKey(spc => spc.StudyPlanId);
        builder.HasOne(spc => spc.Course).WithMany(c => c.StudyPlanCourses)
               .HasForeignKey(spc => spc.CourseId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class VacancyAdditionalSkillConfiguration : IEntityTypeConfiguration<VacancyAdditionalSkill>
{
    public void Configure(EntityTypeBuilder<VacancyAdditionalSkill> builder)
    {
        builder.HasKey(vas => new { vas.VacancyId, vas.AdditionalSkillId });
        builder.HasOne(vas => vas.Vacancy).WithMany(v => v.AdditionalSkills).HasForeignKey(vas => vas.VacancyId);
        builder.HasOne(vas => vas.AdditionalSkill).WithMany(s => s.VacancyAdditionalSkills)
               .HasForeignKey(vas => vas.AdditionalSkillId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class VacancyCourseConfiguration : IEntityTypeConfiguration<VacancyCourse>
{
    public void Configure(EntityTypeBuilder<VacancyCourse> builder)
    {
        builder.HasKey(vc => new { vc.CourseId, vc.VacancyId });
        builder.HasOne(vc => vc.Course).WithMany(c => c.VacancyCourses).HasForeignKey(vc => vc.CourseId);
        builder.HasOne(vc => vc.Vacancy).WithMany(v => v.Courses).HasForeignKey(vc => vc.VacancyId);
    }
}