import type { Config } from "tailwindcss";

export default {
    content: [
        "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
        "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
        "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
    ],
    theme: {
        extend: {
            colors: {
                background: {
                    0: "var(--color-background-0)",
                    100: "var(--color-background-100)",
                    200: "var(--color-background-200)",
                    300: "var(--color-background-300)",
                    popup: "var(--color-background-popup)",
                },
                text: "var(--color-text)",
                textLighter: "var(--color-text-lighter)",
                accent: "var(--color-accent)",
                accentHover: "var(--color-accentHover)",
                accentLighter: "var(--color-accentLighter)",
            },
        },
    },
    plugins: [],
} satisfies Config;
