import { Suspense } from "react";
import { PageFunctionality } from "./codeFunctionality";

export default function AuthorizationCodeCallbackPage() {
    return (
        <Suspense>
            <PageFunctionality />
        </Suspense>
    );
}
