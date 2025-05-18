@echo off
setlocal enabledelayedexpansion
set /p "isGoogleSetup=Would you like to setup the Google OIDC provider? (y/n): "
cd ../backend

if /i "%isGoogleSetup%"=="y" (
    set /p "googleClientId=Enter Google client id: "
    set /p "googleSecretClientId=Enter Google secret client id: "

    dotnet user-secrets set CA_OIDC_GOOGLE_AUTHORITY "https://accounts.google.com" --project ChatApp.Presentation
    dotnet user-secrets set CA_OIDC_GOOGLE_CLIENT_ID "!googleClientId!" --project ChatApp.Presentation
    dotnet user-secrets set CA_OIDC_GOOGLE_SECRET_CLIENT_ID "!googleSecretClientId!" --project ChatApp.Presentation
)

dotnet user-secrets set CA_MSSQL_HOST "localhost,1433" --project ChatApp.Presentation
dotnet user-secrets set CA_MSSQL_PASSWORD "DevelopmentPassword.2025" --project ChatApp.Presentation

cd ../frontend
echo 'Initializing .env file...'
echo NEXT_PUBLIC_BACKEND_URL=https://localhost:8080/api > .env

if /i "%isGoogleSetup%"=="y" (
    echo NEXT_PUBLIC_GOOGLE_CLIENT_ID=%googleClientId%>> .env
)

echo "Finished."
exit /b