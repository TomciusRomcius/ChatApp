import { ReactNode, useEffect } from "react";

interface PopupProps {
    children: ReactNode;
    className?: string;
    onClose: () => void;
}

export default function Popup(props: PopupProps) {
    useEffect(() => {
        const onKeyDown = (ev: KeyboardEvent) => {
            if (ev.key == "Escape") {
                props.onClose();
                window.removeEventListener("keydown", onKeyDown);
            }
        };

        window.addEventListener("keydown", onKeyDown);

        return () => {
            window.removeEventListener("keydown", onKeyDown);
        };
    }, [props]);

    return (
        <div className="fixed w-screen h-screen flex items-center justify-center">
            <div className="absolute top-0 left-0 w-full h-full bg-text opacity-50"></div>
            <div
                className={`z-10 p-4 bg-background-0 rounded-sm ${props.className}`}
            >
                {props.children}
            </div>
        </div>
    );
}
