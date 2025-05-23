import React from "react";

export default function AuthFlowLayout(
    props: React.FormHTMLAttributes<HTMLFormElement>,
) {
    return (
        <form
            className="flex min-h-screen w-screen items-center justify-center"
            {...props}
        >
            <div className="flex w-1/2 flex-col gap-4 rounded-md bg-background-100 p-8 lg:w-1/4 xl:w-1/5">
                {props.children}
            </div>
        </form>
    );
}
