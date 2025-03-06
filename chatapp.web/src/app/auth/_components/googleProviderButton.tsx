import GoogleProviderService from "@/services/oidc/googleProviderService";

export default function GoogleProviderButton({ text }: { text: string }) {
    const providerService = new GoogleProviderService(
        process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID!,
    );

    const handleClick = async () => {
        const authorizationCode =
            await providerService.requestAuthorizationCode();
        alert(authorizationCode);
    };

    return <button onClick={handleClick}>{text}</button>;
}
