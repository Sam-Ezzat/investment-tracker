using Microsoft.AspNetCore.Mvc;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Application.DTOs;

namespace InvestmentTracker.Web.Controllers;

/// <summary>
/// Controller for managing web push notifications
/// </summary>
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Get notification summary (AJAX endpoint)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var summary = await _notificationService.GetNotificationSummaryAsync();
            return Json(new { success = true, data = summary });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get all notifications (AJAX endpoint)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Json(new { success = true, data = notifications });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get payments due today
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDueToday()
    {
        try
        {
            var notifications = await _notificationService.GetPaymentsDueTodayAsync();
            return Json(new { success = true, data = notifications });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get payments due tomorrow
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDueTomorrow()
    {
        try
        {
            var notifications = await _notificationService.GetPaymentsDueTomorrowAsync();
            return Json(new { success = true, data = notifications });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get investments ending soon
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEndingSoon(int days = 3)
    {
        try
        {
            var notifications = await _notificationService.GetInvestmentsEndingSoonAsync(days);
            return Json(new { success = true, data = notifications });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get overdue payments
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOverdue()
    {
        try
        {
            var notifications = await _notificationService.GetOverduePaymentsAsync();
            return Json(new { success = true, data = notifications });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Save notification permission
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SavePermission([FromBody] NotificationPermissionRequest request)
    {
        try
        {
            // For now, use a default userId (in production, use authenticated user)
            var userId = "admin";
            await _notificationService.SaveNotificationPermissionAsync(userId, request.Granted);
            return Json(new { success = true, message = "Permission saved successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Check if permission is granted
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> CheckPermission()
    {
        try
        {
            var userId = "admin";
            var hasPermission = await _notificationService.HasNotificationPermissionAsync(userId);
            return Json(new { success = true, granted = hasPermission });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}

/// <summary>
/// Request model for notification permission
/// </summary>
public class NotificationPermissionRequest
{
    public bool Granted { get; set; }
}
