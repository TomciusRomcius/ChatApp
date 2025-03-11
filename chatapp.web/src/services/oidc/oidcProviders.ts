export interface OidcProvider {
    providerName: string;
    oAuthUrl: string;
}

class _OidcProviders {
    GOOGLE: OidcProvider = {
        providerName: "google",
        oAuthUrl: "https://accounts.google.com/o/oauth2/v2/auth",
    };
}

const OidcProviders = new _OidcProviders();

export default OidcProviders;
