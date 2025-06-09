using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

public class Invitation
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid SenderId { get; set; }
    
    [Required]
    public Guid ReceiverId { get; set; }
    
    public Guid? VacancyId { get; set; }
    
    public Guid? ResumeId { get; set; }
    
    [Required]
    public InvitationType Type { get; set; }
    
    [StringLength(1000)]
    public string? Message { get; set; }
    
    [Required]
    public InvitationStatus Status { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime UpdatedAt { get; set; }
   
    public virtual User Sender { get; set; } = null!;
    public virtual User Receiver { get; set; } = null!;
    public virtual Vacancy? Vacancy { get; set; }
    public virtual Resume? Resume { get; set; }

    public enum InvitationType
    {
        EmployerToStudent,
        StudentToEmployer
    }

    public enum InvitationStatus
    {
        [Display(Name = "Отправлено")]
        Sent,
        [Display(Name = "Принято")]
        Accepted,
        [Display(Name = "Отклонено")]
        Rejected
    }
}
