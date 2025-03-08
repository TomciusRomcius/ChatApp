export default class GoogleProviderService {
    private _clientId: string;

    constructor(clientId: string) {
        this._clientId = clientId;
    }

    async requestAuthorizationCode() {
        const url = new URL("https://accounts.google.com/o/oauth2/v2/auth");
        url.searchParams.append("client_id", this._clientId);
        url.searchParams.append("response_type", "code");
        url.searchParams.append("scope", "openid");
        url.searchParams.append(
            "redirect_uri",
            "https://localhost:3000/auth/code",
        );

        window.location.href = url.toString();
    }
}
