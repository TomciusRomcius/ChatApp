import * as React from "react";
import { SVGProps } from "react";
export default function HamburgerMenu(props: SVGProps<SVGSVGElement>) {
    return (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            width={20}
            height={20}
            fill="#e3e3e3"
            viewBox="0 -960 960 960"
            {...props}
        >
            <path d="M144-264v-72h672v72H144Zm0-180v-72h672v72H144Zm0-180v-72h672v72H144Z" />
        </svg>
    );
}
