import "./globals.css";

export default function RootLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="en">
            {/* TODO: fix hydration, don't know why it happens.... */}
            <body suppressHydrationWarning>{children}</body>
        </html>
    );
}
