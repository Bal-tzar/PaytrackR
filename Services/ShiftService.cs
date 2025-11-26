using Microsoft.EntityFrameworkCore;
using PaytrackR.Data;
using PaytrackR.Models;

namespace PaytrackR.Services;

public class ShiftService(ApplicationDbContext context)
{
    public async Task<Shift> StartShiftAsync(int workerId)
    {
        var worker = await context.Workers.FindAsync(workerId)
            ?? throw new InvalidOperationException("Worker not found");

        var shift = new Shift
        {
            WorkerId = workerId,
            StartTime = DateTime.UtcNow,
            HourlyRateAtTimeOfShift = worker.HourlyRate
        };

        context.Shifts.Add(shift);
        await context.SaveChangesAsync();
        return shift;
    }

    public async Task<Shift> EndShiftAsync(int shiftId)
    {
        var shift = await context.Shifts.FindAsync(shiftId)
            ?? throw new InvalidOperationException("Shift not found");

        if (shift.EndTime.HasValue)
            throw new InvalidOperationException("Shift already ended");

        shift.EndTime = DateTime.UtcNow;
        var hoursWorked = (decimal)(shift.EndTime.Value - shift.StartTime).TotalHours;
        shift.TotalEarnings = hoursWorked * shift.HourlyRateAtTimeOfShift;

        await context.SaveChangesAsync();
        return shift;
    }

    public async Task<List<Shift>> GetWorkerShiftsAsync(int workerId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = context.Shifts
            .Include(s => s.Worker)
            .Where(s => s.WorkerId == workerId);

        if (startDate.HasValue)
            query = query.Where(s => s.StartTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.StartTime <= endDate.Value);

        return await query.OrderByDescending(s => s.StartTime).ToListAsync();
    }

    public async Task<decimal> GetTotalEarningsAsync(int workerId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var shifts = await GetWorkerShiftsAsync(workerId, startDate, endDate);
        return shifts.Where(s => s.EndTime.HasValue).Sum(s => s.TotalEarnings);
    }
}