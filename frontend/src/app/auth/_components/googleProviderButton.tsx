import OidcProviders from "@/services/oidc/oidcProviders";
import OidcProviderService from "@/services/oidc/oidcProviderService";
import axios from "axios";

export default function GoogleProviderButton({ text }: { text: string }) {
    const providerService = new OidcProviderService();

    const handleClick = async () => {
        const csrf = (
            await axios.get(
                `${process.env.NEXT_PUBLIC_BACKEND_URL}/auth/security-key`,
            )
        ).data.csrf;

        // This method redirects
        providerService.RequestAuthorizationCode(
            OidcProviders.GOOGLE,
            process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID!,
            csrf,
        );
    };

    return <button onClick={handleClick}>{text}</button>;
}
