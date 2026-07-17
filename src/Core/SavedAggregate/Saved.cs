namespace Core.SavedAggregate;

public class Saved
{
    public Guid Id { get; set; }
    public Guid SaveableId { get; set; }
    public Guid SaverId { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
}
