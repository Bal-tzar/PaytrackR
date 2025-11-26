namespace PaytrackR.Models;

public class Shift
{
    public int Id { get; set; }
    public int WorkerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal HourlyRateAtTimeOfShift { get; set; }
    public decimal TotalEarnings { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Worker Worker { get; set; } = null!;
    
    // Calculated property
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
}