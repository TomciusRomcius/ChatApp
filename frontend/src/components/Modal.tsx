import React, { useCallback, useEffect } from "react";
import { createPortal } from "react-dom";

interface ModalProps {
    children: React.ReactNode;
    x: number;
    y: number;
    onClose: () => void;
}

export default function Modal(props: ModalProps) {
    const handleClose = useCallback(() => {
        props.onClose();
    }, [props]);

    useEffect(() => {
        document.body.addEventListener("click", handleClose);
        document.body.addEventListener("contextmenu", handleClose);

        return () => {
            document.body.removeEventListener("click", handleClose);
            document.body.removeEventListener("contextmenu", handleClose);
        };
    }, [handleClose]);

    return createPortal(
        <div style={{ top: props.y, left: props.x }} className="fixed">
            {props.children}
        </div>,
        document.body,
    );
}
