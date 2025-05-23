"use client";

import AuthFlowLayout from "@/app/auth/_components/authFlowLayout";
import Input from "@/components/Input";
import ButtonWithPassword from "@/app/auth/_components/buttonWithPassword";
import { FormEvent, useState } from "react";
import UserService from "@/services/userService";
import { useRouter } from "next/navigation";
import ErrorMessage from "@/components/errorMessage";

export default function Page() {
    const router = useRouter();
    const [error, setError] = useState<string>("");
    
    const handleSetup = (ev: FormEvent) => {
        ev.preventDefault();

        const form = ev.target as HTMLFormElement;
        const username = form.username.value;

        if (!username) {
            return;
        }
        
        if (username.length < 2) {
            setError("Username must be at least 3 characters long");
            return;
        }
        if (username.length > 20) {
            setError("Username length must not exceed 20 characters");
            return;
        }

        UserService.SetUserInfo({
            username: username,
        })
            .then(() => {
                router.replace("/application");
            })
            .catch((err) => {
                console.log(err);
            });
    };

    return (
        <AuthFlowLayout onSubmit={handleSetup}>
            <h1 className="text-center text-xl">Setup your account</h1>
            <Input
                className="text-textLighter"
                name="username"
                placeholder="Username"
            />
            <ButtonWithPassword
                type="submit"
                value="Setup"
                className="cursor-pointer rounded-md bg-accent p-2"
            />
            <ErrorMessage message={error}/>
        </AuthFlowLayout>
    );
}
