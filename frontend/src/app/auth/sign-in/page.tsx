"use client";

import { FormEvent } from "react";
import { authService } from "../authService";
import GoogleProviderButton from "../_components/googleProviderButton";
import AuthFlowLayout from "../_components/authFlowLayout";
import Input from "@/components/Input";
import ButtonWithPassword from "@/app/auth/_components/buttonWithPassword";

export default function SignInPage() {
    const handleSignIn = (ev: FormEvent) => {
        // TODO: validate input

        ev.preventDefault();
        const form = ev.target as HTMLFormElement;

        const username = form.username.value;
        const email = form.email.value;
        const password = form.password.value;

        authService.SignInWithPassword(username, email, password);
    };

    return (
        <AuthFlowLayout>
            <form onSubmit={handleSignIn} method="POST">
                <div className="p-8 rounded-md flex flex-col gap-4 bg-background-100">
                    <Input className="text-textLighter" name="username" placeholder="Username" />
                    <Input className="text-textLighter" name="email" placeholder="Email" />
                    <Input className="text-textLighter" name="password" placeholder="Password" />
                    <ButtonWithPassword
                        type="submit"
                        value="Sign in"
                        className="p-2 rounded-md bg-accent cursor-pointer"
                    />
                    <GoogleProviderButton text="Sign in with google!" />
                </div>
            </form>
        </AuthFlowLayout>
    );
}
