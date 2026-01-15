window.notificationHub = {
    start: function (dotnetHelper) {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveNotification", function (notificationId) {
            dotnetHelper.invokeMethodAsync("OnNotificationReceived", notificationId);
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
