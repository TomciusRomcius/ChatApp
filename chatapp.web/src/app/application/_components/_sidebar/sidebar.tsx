import User from "../../_utils/user";
import SidebarUser from "./sidebarUser";

interface SidebarProps {
    friends: User[];
}

export default function Sidebar(props: SidebarProps) {
    return (
        <div className="p-8 flex flex-col items-start gap-12 bg-background-100">
            <div className="w-full flex flex-col gap-4 items-start">
                <button>Add friend</button>
                <button>Friend requests</button>
                <button>Create a group</button>
            </div>
            {/* Friends and group list */}
            <div className="w-full flex flex-col gap-4 items-start">
                {props.friends.map((friend) => (
                    <SidebarUser
                        key={friend.userId}
                        userId={friend.userId}
                        username={friend.userName}
                    ></SidebarUser>
                ))}
            </div>
        </div>
    );
}
