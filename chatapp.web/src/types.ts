export default interface TextMessage {
    senderId: string;
    receiverId: string;
    content: string;
    createdAt: Date;
}
