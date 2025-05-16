export default interface TextMessage {
    senderId: string;
    receiverUserId?: string;
    chatRoomId?: string;
    content: string;
    createdAt: Date;
}

export interface ChatRoom {
    chatRoomId: string;
    name: string;
    adminUserId: string;
}
