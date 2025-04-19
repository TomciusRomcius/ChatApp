import { ReactNode } from "react";

interface AuthFlowLayoutProps {
    children?: ReactNode;
}

export default function AuthFlowLayout(props: AuthFlowLayoutProps) {
    return <div className="w-screen min-h-screen flex items-center justify-center">
        {props.children}
    </div>  
        
}