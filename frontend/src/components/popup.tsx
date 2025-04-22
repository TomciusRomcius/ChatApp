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
        <div className="fixed flex h-screen w-screen items-center justify-center">
            <div className="absolute left-0 top-0 h-full w-full bg-background-popup opacity-75"></div>
            <div
                className={`z-10 rounded-sm bg-background-0 p-4 ${props.className}`}
            >
                {props.children}
            </div>
        </div>
    );
}
