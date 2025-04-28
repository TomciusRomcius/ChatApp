import { InputHTMLAttributes } from "react";
import Input from "@/components/Input";

export default function ButtonWithPassword(
    props: InputHTMLAttributes<HTMLInputElement>,
) {
    return (
        <Input className="cursor-pointer rounded-md bg-accent p-2" {...props} />
    );
}
