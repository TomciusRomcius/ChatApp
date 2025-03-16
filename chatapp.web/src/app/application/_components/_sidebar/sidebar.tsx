import SidebarUser from "./sidebarUser";

export default function Sidebar() {
    return (
        <div className="p-8 flex flex-col items-start gap-12 bg-background-100">
            <div className="w-full flex flex-col gap-4 items-start">
                <button>Add friend</button>
                <button>Friend requests</button>
                <button>Create a group</button>
            </div>
            {/* Friends and group list */}
            <div className="w-full flex flex-col gap-4 items-start">
                <SidebarUser userId="id" username="name"></SidebarUser>
                <SidebarUser userId="id" username="name"></SidebarUser>
                <SidebarUser userId="id" username="name"></SidebarUser>
                <SidebarUser userId="id" username="name"></SidebarUser>
            </div>
        </div>
    );
}
