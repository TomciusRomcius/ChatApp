import TextMessage from "@/types";
import { CurrentChat } from "@/context/currentChatContext";
import UserMessagingService from "@/services/userMessagingService";
import ChatRoomMessagingService from "@/services/chatRoomMessagingService";

export default class MessageSystem {
    public readonly chat: CurrentChat;
    public readonly messageSystemId: string;
    private messages: TextMessage[] = [];
    private listeners: (() => void)[] = [];

    constructor(chat: CurrentChat, messageSystemId: string) {
        this.chat = chat;
        this.messageSystemId = messageSystemId;
    }

    public GetMessages() {
        return this.messages;
    }

    public async LoadMessages(currentChat: CurrentChat) {
        if (currentChat.type == "user") {
            await UserMessagingService.GetMessagesFromFriend(
                currentChat.id,
            ).then((messages) => {
                this.messages = messages;
            });
        } else {
            await ChatRoomMessagingService.GetMessages(
                currentChat.id,
                0,
                20,
            ).then((messages) => {
                this.messages = messages;
            });
        }

        this.CallListeners();
    }

    public AddNewMessage(message: TextMessage): void {
        this.messages.push(message);
        this.CallListeners();
    }

    public AddUpdateListener(listener: () => void) {
        this.listeners.push(listener);
    }

    public RemoveListener(targetListener: () => void) {
        const index = this.listeners.findIndex(
            (listener) => listener === targetListener,
        );
        if (index === -1) {
            throw new Error(`Listener does not exist`);
        }

        this.listeners.splice(index, 1);
    }

    private CallListeners() {
        this.listeners.forEach((listener) => listener());
    }
}
