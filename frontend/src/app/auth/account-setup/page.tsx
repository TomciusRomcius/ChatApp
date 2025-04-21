"use client"

import AuthFlowLayout from "@/app/auth/_components/authFlowLayout";
import Input from "@/components/Input";
import ButtonWithPassword from "@/app/auth/_components/buttonWithPassword";
import { FormEvent } from "react";
import UserService from "@/services/userService";
import { useRouter } from "next/navigation";

export default function Page()
{
    const router = useRouter();
    
    const handleSetup = (ev: FormEvent) => {
        ev.preventDefault();
        
        const form = ev.target as HTMLFormElement;
        const username = form.username.value;
        
        if (!username) 
            return;
        
        UserService.SetUserInfo({
            username: username
        })
            .then(() => {
                router.replace("/application");
            })
            .catch((err) => {
                console.log(err);
            })
    }
    
    return <AuthFlowLayout>
        <form onSubmit={handleSetup} method="POST">
            <div className="p-8 rounded-md flex flex-col gap-4 bg-background-100">
                <h1 className="text-xl text-center">Setup your account</h1>
                <Input className="text-textLighter" name="username" placeholder="Username" />
                <ButtonWithPassword
                    type="submit"
                    value="Setup"
                    className="p-2 rounded-md bg-accent cursor-pointer"
                />
            </div>
        </form>
    </AuthFlowLayout>
}