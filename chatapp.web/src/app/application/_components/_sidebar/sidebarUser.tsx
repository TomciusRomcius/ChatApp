export interface SidebarUserProps {
    username: string;
}

export default function SidebarUser(props: SidebarUserProps) {
    return (
        <div className="w-full flex gap-4">
            <small className="text-base">I</small>
            <small className="text-base">{props.username}</small>
        </div>
    );
}
