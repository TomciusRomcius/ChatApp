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
RUN cd ChatApp.Server && lsx
RUN dotnet build --no-restore

FROM build as test
CMD dotnet test --no-restore

FROM test as RUN
CMD [ "dotnet", "run", "--environment", "production", "--project", "ChatApp.Server" ]

