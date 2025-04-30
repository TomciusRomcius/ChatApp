import UserFriendsService from "@/services/userFriendsService";
import { useRef } from "react";
import AccentButton from "@/components/accentButton";

interface AddFriendProps {
    onSendFriendRequest: () => void;
}

export default function AddFriend(props: AddFriendProps) {
    const usernameRef = useRef<HTMLInputElement>(null);

    const onSendFriendRequest = () => {
        if (!usernameRef.current?.value) {
            throw new Error("Username ref is not defined");
        }

        UserFriendsService.SendFriendRequest(usernameRef.current.value).then(
            () => {
                props.onSendFriendRequest();
            },
        );
    };

    return (
        <>
            <h1 className="text-center text-xl">Add a friend</h1>
            <div className="flex flex-col gap-1">
                <label className="text-sm text-textLighter">Username</label>
                <input
                    className="text-base"
                    ref={usernameRef}
                    placeholder="Enter username"
                />
            </div>
            <AccentButton onClick={onSendFriendRequest}>
                Send friend request
            </AccentButton>
        </>
    );
}
