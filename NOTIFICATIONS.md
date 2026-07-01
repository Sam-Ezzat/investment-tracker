# Web Push Notifications - Implementation Guide

## Overview

The Investment Tracker system includes a comprehensive Web Push Notifications feature that alerts users about critical events in real-time.

## Features

### Notification Types

1. **Payments Due Today** 
   - Alerts when scheduled payments are due on the current day
   - High priority notification
   - Icon: Calendar Day
   - Color: Warning (Yellow)

2. **Payments Due Tomorrow**
   - Reminds about upcoming payments the next day
   - Medium priority notification
   - Icon: Calendar Plus
   - Color: Info (Blue)

3. **Investments Ending Soon**
   - Notifies when investment cycles are ending within 3 days
   - Medium priority notification
   - Icon: Calendar Check
   - Color: Primary (Blue)

4. **Overdue Payments**
   - Critical alerts for payments that are past due
   - Highest priority notification
   - Requires user interaction
   - Icon: Exclamation Triangle
   - Color: Danger (Red)

## Architecture

### Service Worker (`wwwroot/sw.js`)

- **Purpose**: Runs in the background, independent of the web page
- **Functions**:
  - Handles push notifications from the server
  - Manages notification display
  - Handles notification clicks
  - Provides offline caching capabilities
  - Background sync for checking notifications

### Notification Manager (`wwwroot/js/notifications.js`)

- **Purpose**: Client-side notification management
- **Features**:
  - Requests browser notification permission (once)
  - Periodically checks for new notifications (every 5 minutes)
  - Updates notification badge in navbar
  - Renders notification dropdown
  - Manages permission state

### Notification Service (`Infrastructure/Services/NotificationService.cs`)

- **Purpose**: Server-side notification logic
- **Functions**:
  - Queries database for notification-worthy events
  - Generates notification DTOs
  - Stores permission preferences
  - Provides summary and detailed notifications

### Notification Controller (`Web/Controllers/NotificationController.cs`)

- **Purpose**: API endpoints for notifications
- **Endpoints**:
  - `GET /Notification/GetSummary` - Summary with counts
  - `GET /Notification/GetAll` - All notifications
  - `GET /Notification/GetDueToday` - Today's payments
  - `GET /Notification/GetDueTomorrow` - Tomorrow's payments
  - `GET /Notification/GetEndingSoon` - Investments ending soon
  - `GET /Notification/GetOverdue` - Overdue payments
  - `POST /Notification/SavePermission` - Save permission status
  - `GET /Notification/CheckPermission` - Check permission status

## User Experience Flow

### 1. First Visit

```
User lands on Dashboard
  ↓
2 seconds delay
  ↓
Permission banner appears (if not previously dismissed)
  ↓
User clicks "Enable Notifications"
  ↓
Browser permission prompt appears
  ↓
User grants/denies permission
  ↓
Permission saved to server
```

### 2. Permission Granted

```
Service Worker registered
  ↓
Notification checks start (every 5 minutes)
  ↓
New notifications detected
  ↓
Badge count updated in navbar
  ↓
Browser notifications shown for critical items
  ↓
User clicks notification
  ↓
Directed to relevant page
```

### 3. Notification Display

- **Navbar Bell Icon**: Shows badge with unread count
- **Dropdown Menu**: Lists recent notifications (top 5)
- **Browser Notifications**: System-level alerts for critical items

## Permission Handling

### Browser Permission States

1. **default** - Not yet asked (show permission banner)
2. **granted** - Permission given (enable notifications)
3. **denied** - Permission denied (hide banner, don't ask again)

### Storage

- **Client-side**: `localStorage` for banner dismissal
- **Server-side**: Settings table for permission status per user

### One-Time Request

The system ensures the permission prompt is shown only once:
- Checks browser permission status
- Checks server-side saved preference
- Shows banner only if permission is `default` and not dismissed
- Never asks again if denied

## Notification Checking Logic

### Periodic Checks (Client-side)

```javascript
// Check immediately on page load
checkNotifications();

// Then check every 5 minutes
setInterval(checkNotifications, 5 * 60 * 1000);
```

### Background Sync (Service Worker)

```javascript
// Register sync event
registration.sync.register('check-notifications');

// Service worker handles sync
self.addEventListener('sync', checkAndShowNotifications);
```

## Notification Priority

1. **Overdue** (Highest) - Requires interaction, persistent
2. **Due Today** (High) - Immediate attention needed
3. **Due Tomorrow** (Medium) - Advance warning
4. **Ending Soon** (Low) - Informational

## Browser Compatibility

- **Chrome/Edge**: Full support ✅
- **Firefox**: Full support ✅
- **Safari**: Full support (desktop) ✅
- **Mobile Safari**: Limited support ⚠️
- **Opera**: Full support ✅

## Security Considerations

1. **HTTPS Required**: Service Workers only work on HTTPS (or localhost)
2. **User Permission**: Cannot send notifications without explicit permission
3. **No Sensitive Data**: Notifications contain only summary information
4. **Server-side Validation**: All notification data validated server-side

## Testing

### Test Notification Permission

1. Open DevTools → Application → Service Workers
2. Check if service worker is registered
3. Check "Notification" permission in browser settings

### Test Notifications Manually

```javascript
// In browser console:
notificationManager.requestPermission();
notificationManager.checkNotifications();
```

### Simulate Notifications

1. Create test data (payments due today)
2. Refresh dashboard
3. Check badge count updates
4. Click notification bell
5. Verify dropdown shows notifications

## Troubleshooting

### Service Worker Not Registering

**Problem**: Console shows registration errors

**Solutions**:
- Ensure HTTPS or localhost
- Check `sw.js` path is correct
- Clear browser cache and reload
- Check browser console for errors

### Notifications Not Appearing

**Problem**: No browser notifications shown

**Solutions**:
- Check browser permission granted
- Check notification permission in browser settings
- Verify service worker is active
- Check notification data in API response
- Test with browser notification test:
  ```javascript
  new Notification('Test', { body: 'Testing' });
  ```

### Badge Not Updating

**Problem**: Navbar badge shows wrong count

**Solutions**:
- Check API endpoint returns data
- Verify JavaScript no errors in console
- Check `#notificationBadge` element exists
- Force refresh: Ctrl+F5

## Performance

- **Minimal Impact**: Checks run in background
- **Efficient Queries**: Database queries optimized with indexes
- **Caching**: Service worker caches static assets
- **Throttling**: Checks limited to once per 5 minutes

## Future Enhancements

- [ ] Push notifications from server (Web Push API)
- [ ] Notification preferences per user
- [ ] Email fallback for critical notifications
- [ ] SMS integration for urgent alerts
- [ ] Notification history/log
- [ ] Sound customization
- [ ] Do Not Disturb mode
- [ ] Scheduled quiet hours

## API Reference

### Get Summary

```http
GET /Notification/GetSummary
```

**Response:**
```json
{
  "success": true,
  "data": {
    "dueTodayCount": 3,
    "dueTomorrowCount": 2,
    "endingSoonCount": 1,
    "overdueCount": 5,
    "totalUnread": 11,
    "recentNotifications": [...]
  }
}
```

### Save Permission

```http
POST /Notification/SavePermission
Content-Type: application/json

{
  "granted": true
}
```

**Response:**
```json
{
  "success": true,
  "message": "Permission saved successfully"
}
```

## Development Notes

- Service worker updates automatically when `sw.js` changes
- Use `skipWaiting()` to force immediate activation
- Test in incognito mode to avoid cached permission state
- Use Chrome DevTools → Application to debug service workers

---

**Implementation Status**: ✅ Complete
**Last Updated**: Step 9
**Maintainer**: Development Team
