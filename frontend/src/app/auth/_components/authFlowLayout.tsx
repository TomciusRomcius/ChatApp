import { ReactNode } from "react";

interface AuthFlowLayoutProps {
    children?: ReactNode;
}

export default function AuthFlowLayout(props: AuthFlowLayoutProps) {
    return (
        <div className="flex min-h-screen w-screen items-center justify-center">
            {props.children}
        </div>
    );
}
