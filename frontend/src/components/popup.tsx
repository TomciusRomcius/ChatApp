import { ReactNode, useEffect, useRef } from "react";

interface PopupProps {
    children: ReactNode;
    className?: string;
    onClose: () => void;
}

export default function Popup(props: PopupProps) {
    let containerRef = useRef<HTMLDivElement | null>(null);
    
    useEffect(() => {
        const onKeyDown = (ev: KeyboardEvent) => {
            if (ev.key == "Escape") {
                props.onClose();
                window.removeEventListener("keydown", onKeyDown);
            }
        };
        
        const onContainerMouseDown = (e: MouseEvent) => {
            props.onClose();

        }

        window.addEventListener("keydown", onKeyDown);
        containerRef.current!.addEventListener("click", onContainerMouseDown);

        return () => {
            window.removeEventListener("keydown", onKeyDown);
        };
    }, [props]);

    return (
        <div ref={containerRef} className="fixed flex h-screen w-screen items-center justify-center">
            <div className="absolute left-0 top-0 h-full w-full bg-background-popup opacity-75"></div>
            <div
                className={`z-10 rounded-sm bg-background-0 p-4 ${props.className}`}
            >
                {props.children}
            </div>
        </div>
    );
}
