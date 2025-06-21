import React from "react";

export default function AccentButton(
    props: React.ButtonHTMLAttributes<HTMLButtonElement>,
) {
    return (
        <button
            {...props}
            className={`rounded-md bg-accent px-4 py-2 transition hover:bg-accentHover ${props.className}`}
        ></button>
    );
}
