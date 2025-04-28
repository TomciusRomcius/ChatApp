class _NotificationService {
    public readonly notifications: string[] = [];
    private readonly notificationLifetimeMs: number;
    private readonly updateCallbacks: (() => void)[] = [];

    constructor(notificationLifetimeMs: number) {
        this.notificationLifetimeMs = notificationLifetimeMs;
    }

    public AddNotification(notification: string): void {
        this.notifications.push(notification);
        // Remove notification after some time
        setTimeout(() => {
            this.notifications.splice(
                this.notifications.indexOf(notification),
                1,
            );
            this.CallCallbacks();
        }, this.notificationLifetimeMs);

        this.CallCallbacks();
    }

    public AddCallback(callback: () => void): void {
        this.updateCallbacks.push(callback);
    }

    public RemoveCallback(callback: () => void): void {
        this.updateCallbacks.splice(this.updateCallbacks.indexOf(callback), 1);
    }

    private CallCallbacks(): void {
        this.updateCallbacks.forEach((callback) => callback());
    }
}

const notificationLifetimeMs = 5000;
const NotificationService = new _NotificationService(notificationLifetimeMs);

export default NotificationService;
