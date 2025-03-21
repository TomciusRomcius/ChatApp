"use client";

import UserFriendsService from "@/services/userFriendsService";
import ChatView from "./_components/chat/chatView";
import { useEffect, useState } from "react";
import User from "./_utils/user";
import Sidebar from "./_components/_sidebar/sidebar";
import Popup from "@/components/popup";
import AddFriend from "./_components/_popupElements/addFriend";
import { AppState, AppStateContext } from "@/context/appStateContext";
import FriendRequests from "./_components/_popupElements/friendRequests";

export default function ApplicationPage() {
    const [appState, setAppState] = useState<AppState>(AppState.DEFAULT);

    const [friends, setFriends] = useState<User[]>([]);
    const [friendRequests, setFriendRequests] = useState<User[]>([]);

    useEffect(() => {
        UserFriendsService.GetAllFriends().then((friends) =>
            setFriends(friends),
        );

        UserFriendsService.GetAllFriendRequests().then((requests) => {
            setFriendRequests(requests);
        });
    }, []);

    console.log(friends);

    return (
        <div className="w-screen min-h-screen grid grid-cols-6 grid-rows-1 gap-0">
            <AppStateContext.Provider
                value={{ appState: appState, setAppState: setAppState }}
            >
                {appState == AppState.ADD_FRIEND ? (
                    <Popup
                        onClose={() => setAppState(AppState.DEFAULT)}
                        className="flex flex-col gap-2"
                    >
                        <AddFriend
                            onSendFriendRequest={() =>
                                setAppState(AppState.DEFAULT)
                            }
                        />
                    </Popup>
                ) : null}

                {appState == AppState.ACCEPT_FRIEND_REQUEST ? (
                    <Popup
                        onClose={() => setAppState(AppState.DEFAULT)}
                        className="flex flex-col gap-2"
                    >
                        <FriendRequests friendRequests={friendRequests} />
                    </Popup>
                ) : null}
                <Sidebar friends={friends} />
                <div className="px-64 py-8 col-span-5 row-span flex flex-col">
                    <div className="flex flex-col gap-4">
                        <div className="flex flex-col gap-2">
                            <small>Username</small>
                            <small>Date</small>
                        </div>
                        <p>Long Long Long message</p>
                    </div>
                    <ChatView />
                </div>
            </AppStateContext.Provider>
        </div>
    );
}
