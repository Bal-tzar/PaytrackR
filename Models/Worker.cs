namespace PaytrackR.Models;

public class Worker
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation property
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}