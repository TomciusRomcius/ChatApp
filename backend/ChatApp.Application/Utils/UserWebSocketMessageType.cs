namespace ChatApp.Application.Utils;

public static class UserWebSocketMessageType
{
    public static string AddedToChatRoom { get; } = "added-to-chat-room";
    public static string RemovedFromChatRoom { get; } = "removed-from-chat-room";
    public static string NewMessage { get; } = "new-message";
    public static string NewFriendRequest { get; } = "new-friend-request";
    public static string AcceptedFriendRequest { get; } = "accepted-friend-request";

}