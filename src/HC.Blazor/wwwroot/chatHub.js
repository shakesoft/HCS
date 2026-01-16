window.chatHub = {
    start: function (dotnetHelper) {
        // Prevent duplicate connections - check if connection already exists
        if (window._chatConnection) {
            console.log("Chat SignalR connection already exists, reusing...");
            // Store helper in array to support multiple components
            if (!window._chatConnection._dotnetHelpers) {
                window._chatConnection._dotnetHelpers = [];
            }
            // Check if this helper already exists
            if (!window._chatConnection._dotnetHelpers.includes(dotnetHelper)) {
                window._chatConnection._dotnetHelpers.push(dotnetHelper);
            }
            return;
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .withAutomaticReconnect()
            .build();

        // Store dotnetHelper references in array to support multiple components
        connection._dotnetHelpers = [dotnetHelper];

        // Register event handlers only once
        connection.on("ReceiveMessage", function (messageData) {
            console.log("Chat SignalR: Received message", messageData);
            console.log("Chat SignalR: Available helpers:", connection._dotnetHelpers ? connection._dotnetHelpers.length : 0);

            if (connection._dotnetHelpers) {
                connection._dotnetHelpers.forEach((helper, index) => {
                    console.log(`Chat SignalR: Helper ${index}:`, helper);
                    if (helper) {
                        // Call method in ChatHubConnectionService
                        console.log("Chat SignalR: Calling HandleSignalRMessage for helper");
                        helper.invokeMethodAsync("HandleSignalRMessage", messageData)
                            .then(() => console.log("Chat SignalR: HandleSignalRMessage call completed"))
                            .catch(err => {
                                console.error("Error calling HandleSignalRMessage:", err);

                                // Try with namespace
                                console.log("Chat SignalR: Trying with namespace...");
                                return helper.invokeMethodAsync("HC.Blazor.Components.Chat.ChatHubConnectionService.HandleSignalRMessage", messageData);
                            })
                            .then(() => console.log("Chat SignalR: Namespace call completed"))
                            .catch(err2 => console.error("Error calling with namespace:", err2));
                    } else {
                        console.log("Chat SignalR: Helper is null");
                    }
                });
            } else {
                console.log("Chat SignalR: No dotnetHelpers available");
            }
        });

        connection.on("MessageDeleted", function (messageId) {
            if (connection._dotnetHelpers) {
                connection._dotnetHelpers.forEach(helper => {
                    if (helper) {
                        helper.invokeMethodAsync("OnMessageDeleted", messageId)
                            .catch(err => console.error("Error calling OnMessageDeleted:", err));
                    }
                });
            }
        });

        connection.on("ConversationDeleted", function (userId) {
            if (connection._dotnetHelpers) {
                connection._dotnetHelpers.forEach(helper => {
                    if (helper) {
                        helper.invokeMethodAsync("OnConversationDeleted", userId)
                            .catch(err => console.error("Error calling OnConversationDeleted:", err));
                    }
                });
            }
        });

        connection.start()
            .then(() => {
                console.log("Chat SignalR connected successfully");
                console.log("Chat SignalR: Connection established, listening for messages...");
            })
            .catch(err => {
                console.error("Chat SignalR connection error:", err);
                console.error("Chat SignalR: Make sure the hub URL is correct and server is running");
            });

        window._chatConnection = connection;
    },

    stop: function () {
        if (window._chatConnection) {
            // Dispose all helpers
            if (window._chatConnection._dotnetHelpers) {
                window._chatConnection._dotnetHelpers.forEach(helper => {
                    if (helper && helper.dispose) {
                        helper.dispose();
                    }
                });
            }
            window._chatConnection.stop()
                .then(() => console.log("Chat SignalR disconnected"))
                .catch(err => console.error("Chat SignalR disconnect error:", err));
            window._chatConnection = null;
        }
    }
};