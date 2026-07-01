using Microsoft.EntityFrameworkCore;
using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Core.Enums;
using InvestmentTracker.Infrastructure.Data;

namespace InvestmentTracker.Infrastructure.Services;

/// <summary>
/// Service for managing notifications
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationSummaryDto> GetNotificationSummaryAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var tomorrow = today.AddDays(1);
        var threeDaysFromNow = today.AddDays(3);

        var summary = new NotificationSummaryDto();

        // Get counts
        var allSchedules = await _context.PaymentSchedules
            .Where(s => s.Status == PaymentStatus.Pending || s.Status == PaymentStatus.Due || s.Status == PaymentStatus.Overdue)
            .ToListAsync();

        summary.DueTodayCount = allSchedules.Count(s => s.ScheduledDate == today);
        summary.DueTomorrowCount = allSchedules.Count(s => s.ScheduledDate == tomorrow);
        summary.OverdueCount = allSchedules.Count(s => s.Status == PaymentStatus.Overdue);

        var endingSoonCycles = await _context.InvestmentCycles
            .Where(c => c.Status == InvestmentStatus.Active && c.EndDate >= today && c.EndDate <= threeDaysFromNow)
            .CountAsync();
        summary.EndingSoonCount = endingSoonCycles;

        summary.TotalUnread = summary.DueTodayCount + summary.DueTomorrowCount + 
                               summary.EndingSoonCount + summary.OverdueCount;

        // Get recent notifications
        var recentNotifications = new List<NotificationDto>();

        // Add due today
        if (summary.DueTodayCount > 0)
        {
            recentNotifications.Add(new NotificationDto
            {
                Title = "Payments Due Today",
                Message = $"{summary.DueTodayCount} payment(s) are due today",
                Type = "DueToday",
                Icon = "fas fa-calendar-day",
                ActionUrl = "/Dashboard/Index#todaysDueSection",
                CreatedDate = DateTime.Now,
                IsRead = false
            });
        }

        // Add due tomorrow
        if (summary.DueTomorrowCount > 0)
        {
            recentNotifications.Add(new NotificationDto
            {
                Title = "Payments Due Tomorrow",
                Message = $"{summary.DueTomorrowCount} payment(s) are due tomorrow",
                Type = "DueTomorrow",
                Icon = "fas fa-calendar-plus",
                ActionUrl = "/Dashboard/Index#dueSoonSection",
                CreatedDate = DateTime.Now,
                IsRead = false
            });
        }

        // Add ending soon
        if (summary.EndingSoonCount > 0)
        {
            recentNotifications.Add(new NotificationDto
            {
                Title = "Investments Ending Soon",
                Message = $"{summary.EndingSoonCount} investment(s) ending within 3 days",
                Type = "EndingSoon",
                Icon = "fas fa-calendar-check",
                ActionUrl = "/Dashboard/Index",
                CreatedDate = DateTime.Now,
                IsRead = false
            });
        }

        // Add overdue
        if (summary.OverdueCount > 0)
        {
            recentNotifications.Add(new NotificationDto
            {
                Title = "Overdue Payments",
                Message = $"{summary.OverdueCount} payment(s) are overdue",
                Type = "Overdue",
                Icon = "fas fa-exclamation-triangle",
                ActionUrl = "/Dashboard/Index#overdueSection",
                CreatedDate = DateTime.Now,
                IsRead = false
            });
        }

        summary.RecentNotifications = recentNotifications;
        return summary;
    }

    public async Task<List<NotificationDto>> GetPaymentsDueTodayAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var schedules = await _context.PaymentSchedules
            .Include(s => s.InvestmentCycle)
                .ThenInclude(c => c.Participant)
            .Where(s => s.ScheduledDate == today && 
                       (s.Status == PaymentStatus.Pending || s.Status == PaymentStatus.Due))
            .ToListAsync();

        return schedules.Select(s => new NotificationDto
        {
            Id = s.Id,
            Title = "Payment Due Today",
            Message = $"{s.InvestmentCycle.Participant.FullName} - {s.ScheduledAmount:C} due today",
            Type = "DueToday",
            Icon = "fas fa-calendar-day",
            ActionUrl = $"/Payment/Create?scheduleId={s.Id}",
            CreatedDate = DateTime.Now,
            IsRead = false
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetPaymentsDueTomorrowAsync()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var schedules = await _context.PaymentSchedules
            .Include(s => s.InvestmentCycle)
                .ThenInclude(c => c.Participant)
            .Where(s => s.ScheduledDate == tomorrow && 
                       (s.Status == PaymentStatus.Pending || s.Status == PaymentStatus.Due))
            .ToListAsync();

        return schedules.Select(s => new NotificationDto
        {
            Id = s.Id,
            Title = "Payment Due Tomorrow",
            Message = $"{s.InvestmentCycle.Participant.FullName} - {s.ScheduledAmount:C} due tomorrow",
            Type = "DueTomorrow",
            Icon = "fas fa-calendar-plus",
            ActionUrl = $"/Payment/Create?scheduleId={s.Id}",
            CreatedDate = DateTime.Now,
            IsRead = false
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetInvestmentsEndingSoonAsync(int days = 3)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var endDate = today.AddDays(days);

        var cycles = await _context.InvestmentCycles
            .Include(c => c.Participant)
            .Where(c => c.Status == InvestmentStatus.Active && 
                       c.EndDate >= today && 
                       c.EndDate <= endDate)
            .ToListAsync();

        return cycles.Select(c => new NotificationDto
        {
            Id = c.Id,
            Title = "Investment Ending Soon",
            Message = $"{c.Participant.FullName} - Investment ending on {c.EndDate:MMM dd, yyyy}",
            Type = "EndingSoon",
            Icon = "fas fa-calendar-check",
            ActionUrl = $"/InvestmentCycle/Details/{c.Id}",
            CreatedDate = DateTime.Now,
            IsRead = false
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetOverduePaymentsAsync()
    {
        var schedules = await _context.PaymentSchedules
            .Include(s => s.InvestmentCycle)
                .ThenInclude(c => c.Participant)
            .Where(s => s.Status == PaymentStatus.Overdue)
            .ToListAsync();

        return schedules.Select(s => new NotificationDto
        {
            Id = s.Id,
            Title = "Overdue Payment",
            Message = $"{s.InvestmentCycle.Participant.FullName} - {(s.ScheduledAmount - s.PaidAmount):C} overdue since {s.ScheduledDate:MMM dd}",
            Type = "Overdue",
            Icon = "fas fa-exclamation-triangle",
            ActionUrl = $"/Payment/Create?scheduleId={s.Id}",
            CreatedDate = DateTime.Now,
            IsRead = false
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetAllNotificationsAsync()
    {
        var notifications = new List<NotificationDto>();

        // Combine all notification types
        notifications.AddRange(await GetPaymentsDueTodayAsync());
        notifications.AddRange(await GetPaymentsDueTomorrowAsync());
        notifications.AddRange(await GetInvestmentsEndingSoonAsync(3));
        notifications.AddRange(await GetOverduePaymentsAsync());

        return notifications.OrderBy(n => n.Type == "Overdue" ? 0 : 
                                          n.Type == "DueToday" ? 1 : 
                                          n.Type == "DueTomorrow" ? 2 : 3)
                            .ToList();
    }

    public async Task<bool> HasNotificationPermissionAsync(string userId)
    {
        // Check if notification permission is stored in settings
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == $"NotificationPermission_{userId}");
        
        return setting != null && setting.Value == "true";
    }

    public async Task SaveNotificationPermissionAsync(string userId, bool granted)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == $"NotificationPermission_{userId}");

        if (setting == null)
        {
            setting = new Core.Entities.Setting
            {
                Key = $"NotificationPermission_{userId}",
                Value = granted.ToString().ToLower(),
                Category = "Notifications",
                Description = "Web push notification permission status",
                DataType = "Boolean",
                IsVisible = false,
                DisplayOrder = 999
            };
            _context.Settings.Add(setting);
        }
        else
        {
            setting.Value = granted.ToString().ToLower();
            setting.ModifiedDate = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }
}
