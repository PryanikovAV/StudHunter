namespace StudHunter.DB.Postgres.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
