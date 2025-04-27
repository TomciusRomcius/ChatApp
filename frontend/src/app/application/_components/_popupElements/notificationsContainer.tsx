import { useEffect, useState } from "react";
import NotificationService from "@/app/application/_components/_notifications/notificationService";

export default function NotificationsContainer() {
    const [notifications, setNotifications] = useState<string[]>([]);

    useEffect(() => {
        NotificationService.AddCallback(() => {
            setNotifications(NotificationService.notifications);
        });
    }, []);

    let notificationKey = 0;

    return (
        <div className="fixed bottom-0 right-0 w-1/6 p-2">
            <div className="flex flex-col gap-2">
                {notifications.map((notification) => (
                    <div
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
