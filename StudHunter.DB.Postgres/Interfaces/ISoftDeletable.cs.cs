namespace StudHunter.DB.Postgres.Interfaces;

public interface ISoftDeletable : IEntity
{
    bool IsDeleted { get; set; }
}
