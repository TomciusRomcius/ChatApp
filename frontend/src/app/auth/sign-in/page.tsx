"use client";

import { FormEvent, useState } from "react";
import { authService } from "../authService";
import GoogleProviderButton from "../_components/googleProviderButton";
import AuthFlowLayout from "../_components/authFlowLayout";
import Input from "@/components/Input";
import { useRouter } from "next/navigation";
import Link from "next/link";
import ErrorMessage from "@/components/errorMessage";
import AccentButton from "@/components/accentButton";

export default function SignInPage() {
    const router = useRouter();
    const [error, setError] = useState("");

    const handleSignIn = (ev: FormEvent) => {
        // TODO: validate input

        ev.preventDefault();
        const form = ev.target as HTMLFormElement;

        const email = form.email.value;
        const password = form.password.value;

        authService.SignInWithPassword(email, password).then((result) => {
            if (result.didSucceed) {
                router.replace("/application");
            } else {
                setError(result.error);
            }
        });
    };

    return (
        <AuthFlowLayout onSubmit={handleSignIn}>
            <h1 className="text-center text-xl">Login</h1>
            <Input
                className="text-textLighter"
                name="email"
                placeholder="Email"
            />
            <Input
                className="text-textLighter"
                type="password"
                name="password"
                placeholder="Password"
            />
            <AccentButton className="cursor-pointer rounded-md bg-accent p-2">
                Sign in with password
            </AccentButton>
            <GoogleProviderButton text="Sign in with google" />
            <p className="text-textLighter">
                Don&apos;t have an account?
                <Link className="text-accent" href="/auth/sign-up">
                    {" "}
                    Register.
                </Link>
            </p>
            <ErrorMessage message={error} />
        </AuthFlowLayout>
    );
}
