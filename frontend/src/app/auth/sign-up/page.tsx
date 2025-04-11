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
        <div className="flex min-h-screen w-screen items-center justify-center">
            <form onSubmit={handleSignIn} method="POST">
                <div className="p-4 gap-4 border-red-700 flex flex-col border-2">
                    <input name="username" placeholder="Username" />
                    <input name="email" placeholder="Email" />
                    <input name="password" placeholder="Password" />
                    <input
                        type="submit"
                        value="Sign up"
                        className="text-white p-2 bg-stone-700 border-red-700 cursor-pointer"
                    />
                    <button className="p-2 bg-stone-700">
                        Sign up with google
                    </button>
                </div>
            </form>
        </div>
    );
}
