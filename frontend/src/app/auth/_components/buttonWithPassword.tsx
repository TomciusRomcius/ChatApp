import { InputHTMLAttributes } from "react";
import Input from "@/components/Input";

export default function ButtonWithPassword(
    props: InputHTMLAttributes<HTMLInputElement>,
) {
    return <Input className="bg-accent" {...props} />;
}
