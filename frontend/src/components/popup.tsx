import React, { ReactNode, useEffect, useRef } from "react";

interface PopupProps {
    children: ReactNode;
    className?: string;
    onClose: () => void;
}

export default function Popup(props: PopupProps) {
    const outsideContainerRef = useRef<HTMLDivElement | null>(null);
    const containerRef = useRef<HTMLDivElement | null>(null);

    const onOutsideClick = () => {
        props.onClose();
    };

    const onContainerMouseDown = (e: React.MouseEvent<HTMLDivElement>) => {
        e.stopPropagation();
    };

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
        <div
            onClick={onOutsideClick}
            ref={outsideContainerRef}
            className="fixed top-0 z-10 flex h-screen w-screen items-center justify-center"
        >
            <div className="absolute left-0 top-0 h-full w-full bg-background-popup opacity-75"></div>
            <div
                onClick={onContainerMouseDown}
                ref={containerRef}
                className={`z-10 w-1/6 rounded-md bg-background-0 p-4 ${props.className}`}
            >
                {props.children}
            </div>
        </div>
    );
}
