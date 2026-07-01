using InvestmentTracker.Application.DTOs;

namespace InvestmentTracker.Application.Interfaces;

/// <summary>
/// Service interface for managing notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Get notification summary with counts
    /// </summary>
    Task<NotificationSummaryDto> GetNotificationSummaryAsync();

    /// <summary>
    /// Get payments due today
    /// </summary>
    Task<List<NotificationDto>> GetPaymentsDueTodayAsync();

    /// <summary>
    /// Get payments due tomorrow
    /// </summary>
    Task<List<NotificationDto>> GetPaymentsDueTomorrowAsync();

    /// <summary>
    /// Get investments ending within specified days
    /// </summary>
    Task<List<NotificationDto>> GetInvestmentsEndingSoonAsync(int days = 3);

    /// <summary>
    /// Get overdue payments
    /// </summary>
    Task<List<NotificationDto>> GetOverduePaymentsAsync();

    /// <summary>
    /// Get all active notifications
    /// </summary>
    Task<List<NotificationDto>> GetAllNotificationsAsync();

    /// <summary>
    /// Check if user has granted notification permission
    /// </summary>
    Task<bool> HasNotificationPermissionAsync(string userId);

    /// <summary>
    /// Save notification permission status
    /// </summary>
    Task SaveNotificationPermissionAsync(string userId, bool granted);
}
