// Service Worker for Web Push Notifications
// This file runs in the background and handles push notifications

const CACHE_NAME = 'investment-tracker-v1';
const urlsToCache = [
    '/',
    '/css/site.css',
    '/js/site.js',
    '/lib/adminlte/dist/css/adminlte.min.css',
    '/lib/jquery/dist/jquery.min.js'
];

// Install Service Worker
self.addEventListener('install', event => {
    console.log('[Service Worker] Installing...');
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('[Service Worker] Caching app shell');
                return cache.addAll(urlsToCache);
            })
            .catch(err => {
                console.error('[Service Worker] Install failed:', err);
            })
    );
    self.skipWaiting();
});

// Activate Service Worker
self.addEventListener('activate', event => {
    console.log('[Service Worker] Activating...');
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== CACHE_NAME) {
                        console.log('[Service Worker] Deleting old cache:', cacheName);
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
    return self.clients.claim();
});

// Fetch event (serve from cache when offline)
self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                // Cache hit - return response
                if (response) {
                    return response;
                }
                return fetch(event.request);
            })
    );
});

// Push notification event
self.addEventListener('push', event => {
    console.log('[Service Worker] Push received');
    
    let notificationData = {
        title: 'Investment Tracker',
        body: 'You have new notifications',
        icon: '/images/notification-icon.png',
        badge: '/images/notification-badge.png',
        tag: 'investment-notification',
        requireInteraction: false,
        data: {
            url: '/'
        }
    };

    if (event.data) {
        try {
            const data = event.data.json();
            notificationData = {
                title: data.title || notificationData.title,
                body: data.message || notificationData.body,
                icon: notificationData.icon,
                badge: notificationData.badge,
                tag: data.type || notificationData.tag,
                requireInteraction: data.type === 'Overdue',
                data: {
                    url: data.actionUrl || '/'
                }
            };
        } catch (e) {
            console.error('[Service Worker] Error parsing push data:', e);
        }
    }

    event.waitUntil(
        self.registration.showNotification(notificationData.title, {
            body: notificationData.body,
            icon: notificationData.icon,
            badge: notificationData.badge,
            tag: notificationData.tag,
            requireInteraction: notificationData.requireInteraction,
            vibrate: [200, 100, 200],
            data: notificationData.data,
            actions: [
                {
                    action: 'view',
                    title: 'View Details'
                },
                {
                    action: 'close',
                    title: 'Dismiss'
                }
            ]
        })
    );
});

// Notification click event
self.addEventListener('notificationclick', event => {
    console.log('[Service Worker] Notification clicked');
    
    event.notification.close();

    if (event.action === 'close') {
        return;
    }

    // Open the URL specified in the notification data
    const urlToOpen = event.notification.data.url || '/';

    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true })
            .then(windowClients => {
                // Check if there's already a window open
                for (let i = 0; i < windowClients.length; i++) {
                    const client = windowClients[i];
                    if (client.url === urlToOpen && 'focus' in client) {
                        return client.focus();
                    }
                }
                // If not, open a new window
                if (clients.openWindow) {
                    return clients.openWindow(urlToOpen);
                }
            })
    );
});

// Background sync for checking notifications
self.addEventListener('sync', event => {
    console.log('[Service Worker] Background sync:', event.tag);
    
    if (event.tag === 'check-notifications') {
        event.waitUntil(checkAndShowNotifications());
    }
});

// Function to check and show notifications
async function checkAndShowNotifications() {
    try {
        const response = await fetch('/Notification/GetSummary');
        const result = await response.json();
        
        if (result.success && result.data) {
            const summary = result.data;
            
            // Show notification if there are unread items
            if (summary.totalUnread > 0) {
                const recentNotifications = summary.recentNotifications || [];
                
                // Show each recent notification
                for (const notification of recentNotifications) {
                    await self.registration.showNotification(notification.title, {
                        body: notification.message,
                        icon: '/images/notification-icon.png',
                        badge: '/images/notification-badge.png',
                        tag: notification.type,
                        requireInteraction: notification.type === 'Overdue',
                        data: {
                            url: notification.actionUrl
                        }
                    });
                }
            }
        }
    } catch (error) {
        console.error('[Service Worker] Error checking notifications:', error);
    }
}
