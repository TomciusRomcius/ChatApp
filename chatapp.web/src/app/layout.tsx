import "./globals.css";

export default function RootLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="en">
            {/* TODO: fix hydration, don't know why it happens.... */}
            <body className="bg-background-0" suppressHydrationWarning>
                {children}
            </body>
        </html>
    );
}
