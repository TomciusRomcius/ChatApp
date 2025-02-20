"use client";

import { FormEvent } from "react";
import { authService } from "../authService";

export default function SignUpPage() {
    const handleSignIn = (ev: FormEvent) => {
        // TODO: validate input

        ev.preventDefault();
        const form = ev.target as HTMLFormElement;

        const username = form.username.value;
        const email = form.email.value;
        const password = form.password.value;

        authService.SignUpWithPassword(username, email, password);
    };

    return (
        <div className="w-screen min-h-screen flex items-center justify-center">
            <form onSubmit={handleSignIn} method="POST">
                <div className="p-4 flex flex-col gap-4 border-red-700 border-2">
                    <input name="username" placeholder="Username" />
                    <input name="email" placeholder="Email" />
                    <input name="password" placeholder="Password" />
                    <input
                        type="submit"
                        value="Sign in"
                        className="bg-stone-700 border-red-700 cursor-pointer"
                    />
                </div>
            </form>
        </div>
    );
}
