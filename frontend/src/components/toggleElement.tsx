import { ReactNode, useState } from "react";

interface ToggleElementProps {
    containerClassName?: string;
    buttonChildren: ReactNode;
    children: ReactNode;
}

export default function ToggleElement(props: ToggleElementProps) {
    const [isOpen, setIsOpen] = useState(false);

    const onToggle = () => {
        setIsOpen(!isOpen);
    };

    return (
        <div className={props.containerClassName}>
            <button onClick={onToggle}>{props.buttonChildren}</button>
            {isOpen && props.children}
        </div>
    );
}
