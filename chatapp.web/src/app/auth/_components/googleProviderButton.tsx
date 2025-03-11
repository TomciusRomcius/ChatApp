import GoogleProviderService from "@/services/oidc/googleProviderService";
import axios from "axios";

export default function GoogleProviderButton({ text }: { text: string }) {
    const providerService = new GoogleProviderService(
        process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID!,
    );

    const handleClick = async () => {
        const csrf = (
            await axios.get(
                `${process.env.NEXT_PUBLIC_BACKEND_URL}/auth/security-key`,
            )
        ).data.csrf;

        const authorizationCode =
            await providerService.requestAuthorizationCode(csrf);
        alert(authorizationCode);
    };

    return <button onClick={handleClick}>{text}</button>;
}
