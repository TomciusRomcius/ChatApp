FROM docker:dind as base
RUN apk add --no-cache dotnet-sdk-9.0

FROM base as build
WORKDIR /app
EXPOSE 5112
COPY ./ChatApp.sln .
COPY ./ChatApp.Server/ChatApp.Server.csproj ./ChatApp.Server/
COPY ./ChatApp.Server.Application/ChatApp.Server.Application.csproj ./ChatApp.Server.Application/
COPY ./ChatApp.Server.Domain/ChatApp.Server.Domain.csproj ./ChatApp.Server.Domain/
COPY ./ChatApp.Server.Application.Tests/ChatApp.Server.Application.Tests.csproj ./ChatApp.Server.Application.Tests/
RUN dotnet restore
COPY . .
RUN cd ChatApp.Server && ls
RUN dotnet build --no-restore

FROM build as test
CMD dotnet test --no-restore

# not ideal, but for now
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS run
WORKDIR /app
RUN dotnet tool install --global dotnet-ef --version 9.* && dotnet tool restore
COPY --from=build /app .
CMD "dotnet dotnet-ef database update --project ChatApp.Server.Application --startup-project ChatApp.Server --environment Production"
CMD [ "dotnet", "run", "--environment", "Production", "--project", "ChatApp.Server" ]
