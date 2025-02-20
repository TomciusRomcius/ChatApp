export interface PublicConfiguration {
    BACKEND_URL: string;
}

const backendUrl = process.env.NEXT_PUBLIC_BACKEND_URL;

if (!backendUrl) {
    throw new Error("Backend url is undefined");
}

export const publicConfiguration: PublicConfiguration = {
    BACKEND_URL: backendUrl,
};
