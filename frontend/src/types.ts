export default interface TextMessage {
    senderId: string;
    receiverUserId?: string;
    chatRoomId?: string;
    content: string;
    createdAt: string;
}

export interface ChatRoom {
    chatRoomId: string;
    name: string;
    adminUserId: string;
}
