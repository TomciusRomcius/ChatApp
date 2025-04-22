"use client";

import { FormEvent } from "react";
import { authService } from "../authService";
import Input from "@/components/Input";

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
                <div className="flex flex-col gap-4 border-2 border-red-700 p-4">
                    <Input name="username" placeholder="Username" />
                    <Input name="email" placeholder="Email" />
                    <Input name="password" placeholder="Password" />
                    <Input
                        type="submit"
                        value="Sign up"
                        className="cursor-pointer border-red-700 bg-stone-700 p-2 text-white"
                    />
                    <button className="bg-stone-700 p-2">
                        Sign up with google
                    </button>
                </div>
            </form>
        </div>
    );
}
