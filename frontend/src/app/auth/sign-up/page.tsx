"use client";

import { FormEvent, useState } from "react";
import { authService } from "../authService";
import Input from "@/components/Input";
import GoogleProviderButton from "@/app/auth/_components/googleProviderButton";
import ButtonWithPassword from "@/app/auth/_components/buttonWithPassword";
import Link from "next/link";
import AuthFlowLayout from "@/app/auth/_components/authFlowLayout";
import { useRouter } from "next/navigation";
import ErrorMessage from "@/components/errorMessage";
import { Result } from "@/utils/Result";

export default function SignUpPage() {
    const router = useRouter();
    const [error, setError] = useState<string>("");

    const handleSignUp = (ev: FormEvent) => {
        // TODO: validate input

        ev.preventDefault();
        const form = ev.target as HTMLFormElement;

        const email = form.email.value;
        const password = form.password.value;
        const repeatPassword = form.repeatPassword.value;

        // TODO: error message
        if (!email || !password || !repeatPassword) {
            return;
        }

        if (password !== repeatPassword) {
            return;
        }

        authService
            .SignUpWithPassword(email, password)
            .then((result: Result<null, string>) => {
                if (!result.didSucceed) {
                    setError(result.error);
                } else {
                    router.replace("/auth/account-setup");
                }
            });
    };

    return (
        <AuthFlowLayout onSubmit={handleSignUp}>
            <h1 className="text-center text-xl">Register</h1>
            <Input name="email" placeholder="Email" />
            <Input name="password" placeholder="Password" />
            <Input name="repeatPassword" placeholder="Repeat password" />
            <ButtonWithPassword type="submit" value="Sign up" />
            <GoogleProviderButton text="Sign up with Google" />
            <p className="text-textLighter">
                Have an account?
                <Link className="text-accent" href="/auth/sign-in">
                    {" "}
                    Login.
                </Link>
            </p>
            <ErrorMessage message={error} />
        </AuthFlowLayout>
    );
}
