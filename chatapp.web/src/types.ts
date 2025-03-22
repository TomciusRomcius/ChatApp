export default interface TextMessage {
    senderId: string;
    receiverId?: string;
    chatRoomId?: string;
    content: string;
    createdAt: Date;
}

export interface ChatRoom {
    chatRoomId: string;
    name: string;
    adminUserId: string;
}
