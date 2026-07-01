// Notification Manager for Investment Tracker
// Handles browser notification permissions and displays

class NotificationManager {
    constructor() {
        this.serviceWorker = null;
        this.permissionGranted = false;
        this.checkInterval = null;
        this.init();
    }

    // Initialize notification system
    async init() {
        // Check if service worker is supported
        if (!('serviceWorker' in navigator)) {
            console.warn('Service Worker not supported');
            return;
        }

        // Check if notifications are supported
        if (!('Notification' in window)) {
            console.warn('Notifications not supported');
            return;
        }

        // Register service worker
        try {
            const registration = await navigator.serviceWorker.register('/sw.js', {
                scope: '/'
            });
            console.log('Service Worker registered:', registration);
            this.serviceWorker = registration;

            // Check permission status
            await this.checkPermissionStatus();

            // Start periodic notification checks if permission granted
            if (this.permissionGranted) {
                this.startNotificationChecks();
            }
        } catch (error) {
            console.error('Service Worker registration failed:', error);
        }
    }

    // Check if permission was previously granted
    async checkPermissionStatus() {
        const currentPermission = Notification.permission;
        
        if (currentPermission === 'granted') {
            this.permissionGranted = true;
            return true;
        } else if (currentPermission === 'denied') {
            this.permissionGranted = false;
            return false;
        }

        // Check server-side setting
        try {
            const response = await fetch('/Notification/CheckPermission');
            const result = await response.json();
            if (result.success && result.granted) {
                // Permission was granted before, ask again
                await this.requestPermission();
            }
        } catch (error) {
            console.error('Error checking permission:', error);
        }

        return false;
    }

    // Request notification permission (only once)
    async requestPermission() {
        if (Notification.permission === 'granted') {
            this.permissionGranted = true;
            await this.savePermission(true);
            this.startNotificationChecks();
            return true;
        }

        if (Notification.permission === 'denied') {
            this.permissionGranted = false;
            await this.savePermission(false);
            return false;
        }

        // Ask user for permission
        try {
            const permission = await Notification.requestPermission();
            const granted = permission === 'granted';
            this.permissionGranted = granted;
            await this.savePermission(granted);

            if (granted) {
                this.startNotificationChecks();
                this.showSuccessMessage('Notifications enabled successfully!');
            }

            return granted;
        } catch (error) {
            console.error('Error requesting permission:', error);
            return false;
        }
    }

    // Save permission status to server
    async savePermission(granted) {
        try {
            await fetch('/Notification/SavePermission', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ granted })
            });
        } catch (error) {
            console.error('Error saving permission:', error);
        }
    }

    // Start periodic notification checks
    startNotificationChecks() {
        if (this.checkInterval) {
            clearInterval(this.checkInterval);
        }

        // Check immediately
        this.checkNotifications();

        // Check every 5 minutes
        this.checkInterval = setInterval(() => {
            this.checkNotifications();
        }, 5 * 60 * 1000);
    }

    // Check for new notifications
    async checkNotifications() {
        if (!this.permissionGranted) {
            return;
        }

        try {
            const response = await fetch('/Notification/GetSummary');
            const result = await response.json();

            if (result.success && result.data) {
                const summary = result.data;
                this.updateBadge(summary.totalUnread);
                
                // Show browser notifications for critical items
                if (summary.overdueCount > 0 || summary.dueTodayCount > 0) {
                    this.showBrowserNotifications(summary.recentNotifications);
                }
            }
        } catch (error) {
            console.error('Error checking notifications:', error);
        }
    }

    // Update notification badge in navbar
    updateBadge(count) {
        const badge = document.getElementById('notificationBadge');
        const bellIcon = document.getElementById('notificationBell');
        
        if (badge) {
            if (count > 0) {
                badge.textContent = count > 99 ? '99+' : count;
                badge.style.display = 'inline-block';
                if (bellIcon) {
                    bellIcon.classList.add('text-warning');
                }
            } else {
                badge.style.display = 'none';
                if (bellIcon) {
                    bellIcon.classList.remove('text-warning');
                }
            }
        }
    }

    // Show browser notifications
    async showBrowserNotifications(notifications) {
        if (!this.permissionGranted || !notifications || notifications.length === 0) {
            return;
        }

        // Show only high-priority notifications
        const highPriority = notifications.filter(n => 
            n.type === 'Overdue' || n.type === 'DueToday'
        );

        for (const notification of highPriority) {
            if (this.serviceWorker) {
                // Use service worker to show notification
                await this.serviceWorker.showNotification(notification.title, {
                    body: notification.message,
                    icon: '/images/notification-icon.png',
                    badge: '/images/notification-badge.png',
                    tag: notification.type,
                    requireInteraction: notification.type === 'Overdue',
                    data: {
                        url: notification.actionUrl
                    },
                    actions: [
                        { action: 'view', title: 'View' },
                        { action: 'close', title: 'Dismiss' }
                    ]
                });
            }
        }
    }

    // Load and display notifications in dropdown
    async loadNotificationDropdown() {
        try {
            const response = await fetch('/Notification/GetAll');
            const result = await response.json();

            if (result.success && result.data) {
                this.renderNotificationDropdown(result.data);
            }
        } catch (error) {
            console.error('Error loading notifications:', error);
        }
    }

    // Render notifications in dropdown menu
    renderNotificationDropdown(notifications) {
        const dropdown = document.getElementById('notificationDropdown');
        if (!dropdown) return;

        if (notifications.length === 0) {
            dropdown.innerHTML = `
                <div class="dropdown-item text-center text-muted">
                    <i class="fas fa-check-circle"></i> No new notifications
                </div>
            `;
            return;
        }

        let html = '';
        notifications.slice(0, 5).forEach(notification => {
            const iconClass = this.getIconClass(notification.type);
            const colorClass = this.getColorClass(notification.type);
            
            html += `
                <a href="${notification.actionUrl}" class="dropdown-item">
                    <i class="${iconClass} ${colorClass} mr-2"></i>
                    <div class="d-inline-block text-truncate" style="max-width: 250px;">
                        <strong>${notification.title}</strong><br>
                        <small class="text-muted">${notification.message}</small>
                    </div>
                </a>
                <div class="dropdown-divider"></div>
            `;
        });

        html += `
            <a href="/Dashboard/Index" class="dropdown-item dropdown-footer">
                View All Notifications
            </a>
        `;

        dropdown.innerHTML = html;
    }

    // Get icon class based on notification type
    getIconClass(type) {
        const icons = {
            'DueToday': 'fas fa-calendar-day',
            'DueTomorrow': 'fas fa-calendar-plus',
            'EndingSoon': 'fas fa-calendar-check',
            'Overdue': 'fas fa-exclamation-triangle'
        };
        return icons[type] || 'fas fa-bell';
    }

    // Get color class based on notification type
    getColorClass(type) {
        const colors = {
            'DueToday': 'text-warning',
            'DueTomorrow': 'text-info',
            'EndingSoon': 'text-primary',
            'Overdue': 'text-danger'
        };
        return colors[type] || 'text-secondary';
    }

    // Show success message
    showSuccessMessage(message) {
        // Use Toastr if available
        if (typeof toastr !== 'undefined') {
            toastr.success(message);
        } else {
            console.log(message);
        }
    }

    // Stop notification checks
    stop() {
        if (this.checkInterval) {
            clearInterval(this.checkInterval);
            this.checkInterval = null;
        }
    }
}

// Global instance
let notificationManager = null;

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    notificationManager = new NotificationManager();

    // Enable notifications button click
    const enableBtn = document.getElementById('enableNotificationsBtn');
    if (enableBtn) {
        enableBtn.addEventListener('click', async (e) => {
            e.preventDefault();
            await notificationManager.requestPermission();
        });
    }

    // Notification bell click
    const notificationBell = document.getElementById('notificationBell');
    if (notificationBell) {
        notificationBell.addEventListener('click', () => {
            notificationManager.loadNotificationDropdown();
        });
    }

    // Load notifications on page load
    notificationManager.loadNotificationDropdown();
});

// Cleanup on page unload
window.addEventListener('beforeunload', () => {
    if (notificationManager) {
        notificationManager.stop();
    }
});
