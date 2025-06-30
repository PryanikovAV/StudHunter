using StudHunter.DB.Postgres.Models;
using System.ComponentModel.DataAnnotations;

public class Invitation
{
    public Guid Id { get; set; }
    
    public Guid SenderId { get; set; }
    
    public Guid ReceiverId { get; set; }
    
    public Guid? VacancyId { get; set; }
    
    public Guid? ResumeId { get; set; }
    
    public InvitationType Type { get; set; }
    
    public string? Message { get; set; }
    
    public InvitationStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
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
        Sent,
        Accepted,
        Rejected
    }
}
