window.notificationHub = {
    start: function (dotnetHelper) {
        // Prevent duplicate connections - check if connection already exists
        if (window._notificationConnection) {
            console.log("SignalR connection already exists, reusing...");
            // Store helper in array to support multiple components
            if (!window._notificationConnection._dotnetHelpers) {
                window._notificationConnection._dotnetHelpers = [];
            }
            // Check if this helper already exists
            if (!window._notificationConnection._dotnetHelpers.includes(dotnetHelper)) {
                window._notificationConnection._dotnetHelpers.push(dotnetHelper);
            }
            return;
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        // Store dotnetHelper references in array to support multiple components
        connection._dotnetHelpers = [dotnetHelper];

        // Register event handler only once
        // Note: connection.on() can be called multiple times, but we only register once per connection
        connection.on("ReceiveNotification", function (notificationId) {
            if (connection._dotnetHelpers) {
                connection._dotnetHelpers.forEach(helper => {
                    if (helper) {
                        helper.invokeMethodAsync("OnNotificationReceived", notificationId)
                            .catch(err => console.error("Error calling OnNotificationReceived:", err));
                    }
                });
            }
        });

        // Listen for unread count changes
        connection.on("UnreadCountChanged", function () {
            if (connection._dotnetHelpers) {
                connection._dotnetHelpers.forEach(helper => {
                    if (helper) {
                        helper.invokeMethodAsync("OnUnreadCountChanged")
                            .catch(err => console.error("Error calling OnUnreadCountChanged:", err));
                    }
                });
            }
        });

        connection.start()
            .then(() => console.log("SignalR connected"))
            .catch(err => console.error("SignalR connection error:", err));

        window._notificationConnection = connection;
    },
    
    stop: function () {
        if (window._notificationConnection) {
            // Dispose all helpers
            if (window._notificationConnection._dotnetHelpers) {
                window._notificationConnection._dotnetHelpers.forEach(helper => {
                    if (helper && helper.dispose) {
                        helper.dispose();
                    }
                });
            }
            window._notificationConnection.stop()
                .then(() => console.log("SignalR disconnected"))
                .catch(err => console.error("SignalR disconnect error:", err));
            window._notificationConnection = null;
        }
    }
};
