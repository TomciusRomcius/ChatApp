"use client";

import UserFriendsService from "@/services/userFriendsService";
import ChatView from "./_components/chat/chatView";
import { useEffect, useState } from "react";
import User from "./_utils/user";
import Sidebar from "./_components/_sidebar/sidebar";

export default function ApplicationPage() {
    const [friends, setFriends] = useState<User[]>([]);

    useEffect(() => {
        UserFriendsService.GetAllFriends().then((friends) =>
            setFriends(friends),
        );
    }, []);

    console.log(friends);

    return (
        <div className="w-screen min-h-screen grid grid-cols-6 grid-rows-1 gap-0">
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
        </div>
    );
}
