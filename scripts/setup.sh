cd ../backend
echo 'Would you like to setup the Google OIDC provider? (y/n):'
read isGoogleSetup
if [ "$isGoogleSetup" == 'y' ]; then 
    echo 'Enter Google client id: '
    read googleClientId
    echo 'Enter Google secret client id(input is hidden):'
    read -s googleSecretClientId
    dotnet user-secrets set CA_OIDC_GOOGLE_AUTHORITY 'https://accounts.google.com' --project ChatApp.Presentation 
    dotnet user-secrets set CA_OIDC_GOOGLE_CLIENT_ID "$googleClientId" --project ChatApp.Presentation 
    dotnet user-secrets set CA_OIDC_GOOGLE_SECRET_CLIENT_ID "$googleSecretClientId" --project ChatApp.Presentation 
fi

dotnet user-secrets set CA_MSSQL_HOST 'localhost,1433' --project ChatApp.Presentation 
dotnet user-secrets set CA_MSSQL_PASSWORD 'DevelopmentPassword.2025' --project ChatApp.Presentation

cd ../frontend
echo 'Initializing .env file...'
echo 'NEXT_PUBLIC_BACKEND_URL=https://localhost:8080/api' > .env

if [ "$isGoogleSetup" == 'y' ]; then
    echo "NEXT_PUBLIC_GOOGLE_CLIENT_ID=$googleClientId" >> .env
fi
echo 'Finished.'
