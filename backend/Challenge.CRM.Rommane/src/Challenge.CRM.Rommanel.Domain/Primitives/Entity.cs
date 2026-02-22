namespace Challenge.CRM.Rommanel.Domain.Primitives;

public abstract class Entity
{
    public Guid Id { get; private init; }
    public DateTime CreatedAt { get; private init; }

    /// <summary>
    /// Construtor para criação de novas entidades.
    /// </summary>
    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}