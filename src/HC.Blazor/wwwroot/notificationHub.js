window.notificationHub = {
    start: function (dotnetHelper) {
        // Prevent duplicate connections - check if connection already exists
        if (window._notificationConnection) {
            console.log("SignalR connection already exists, reusing...");
            // Update the dotnetHelper reference in case component was recreated
            if (window._notificationConnection._dotnetHelper) {
                window._notificationConnection._dotnetHelper.dispose();
            }
            window._notificationConnection._dotnetHelper = dotnetHelper;
            return;
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        // Store dotnetHelper reference
        connection._dotnetHelper = dotnetHelper;

        // Register event handler only once
        connection.on("ReceiveNotification", function (notificationId) {
            if (connection._dotnetHelper) {
                connection._dotnetHelper.invokeMethodAsync("OnNotificationReceived", notificationId);
            }
        });

        connection.start()
            .then(() => console.log("SignalR connected"))
            .catch(err => console.error("SignalR connection error:", err));

        window._notificationConnection = connection;
    },
    
    stop: function () {
        if (window._notificationConnection) {
            window._notificationConnection.stop()
                .then(() => console.log("SignalR disconnected"))
                .catch(err => console.error("SignalR disconnect error:", err));
            window._notificationConnection = null;
        }
    }
};
