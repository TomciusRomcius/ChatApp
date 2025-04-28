import { useEffect, useState } from "react";
import NotificationService from "@/app/application/_components/_notifications/notificationService";

export default function NotificationsContainer() {
    const [_, setUpdate] = useState([]);

    useEffect(() => {
        NotificationService.AddCallback(() => {
            // Change reference to trigger update
            setUpdate([]);
        });
    }, []);

    const notifications = NotificationService.notifications;
    let notificationKey = 0;

    return (
        <div className="fixed bottom-0 right-0 w-1/6 p-2">
            <div className="flex flex-col gap-2">
                {notifications.map((notification) => (
                    <div
                        /* TODO: not the most efficient. Attach keys to notifications
                            because when notification is deleted the order and as a result the
                            key gets changed.                        
                        */
                        key={notificationKey++}
                        className="rounded-md bg-background-200 p-2"
                    >
                        <small className="text-base">{notification}</small>
                    </div>
                ))}
            </div>
        </div>
    );
}
