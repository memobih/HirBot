// Initialize Pusher
const pusher = new Pusher('YOUR_PUSHER_KEY', {
    cluster: 'YOUR_PUSHER_CLUSTER',
    encrypted: true
});

// Function to subscribe to user's notification channel
function subscribeToNotifications(userId) {
    const channel = pusher.subscribe(`user-${userId}`);
    
    channel.bind('new-notification', function(data) {
        // Show notification using browser's notification API
        if (Notification.permission === "granted") {
            new Notification("New Notification", {
                body: data.message,
                icon: "/path/to/your/icon.png"
            });
        }
        
        // Update notification count
        updateNotificationCount();
        
        // Refresh notification list
        refreshNotifications();
    });
}

// Request notification permission
function requestNotificationPermission() {
    if (Notification.permission !== "granted" && Notification.permission !== "denied") {
        Notification.requestPermission();
    }
}

// Update notification count
function updateNotificationCount() {
    fetch(`/api/Notification/unread-count/${currentUserId}`)
        .then(response => response.json())
        .then(data => {
            if (data.isSuccess) {
                const countElement = document.querySelector('.unread-count');
                if (countElement) {
                    countElement.textContent = `${data.data} unread`;
                }
            }
        });
}

// Refresh notification list
function refreshNotifications() {
    fetch(`/api/Notification/${currentUserId}`)
        .then(response => response.json())
        .then(data => {
            if (data.isSuccess) {
                // Dispatch custom event to notify Blazor component
                window.dispatchEvent(new CustomEvent('notifications-updated', {
                    detail: data.data
                }));
            }
        });
}

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', function() {
    requestNotificationPermission();
    // Replace with actual user ID
    const currentUserId = 'current-user-id';
    subscribeToNotifications(currentUserId);
}); 