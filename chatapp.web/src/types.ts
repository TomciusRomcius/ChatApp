export default interface TextMessage {
    senderId: string;
    receiverId: string;
    content: string;
    createdAt: Date;
}

export interface ChatRoom {
    chatRoomId: string;
    name: string;
    adminUserId: string;
}
