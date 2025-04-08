FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
EXPOSE 5112
COPY ./ChatApp.sln .
COPY ./ChatApp.Server/ChatApp.Server.csproj ./ChatApp.Server/
COPY ./ChatApp.Server.Application/ChatApp.Server.Application.csproj ./ChatApp.Server.Application/
COPY ./ChatApp.Server.Domain/ChatApp.Server.Domain.csproj ./ChatApp.Server.Domain/
COPY ./ChatApp.Server.Application.Tests/ChatApp.Server.Application.Tests.csproj ./ChatApp.Server.Application.Tests/
COPY ./ChatApp.Infrastructure/ChatApp.Server.Infrastructure.csproj ./ChatApp.Infrastructure/
COPY ./ChatApp.Infrastructure.Tests/ChatApp.Infrastructure.Tests.csproj ./ChatApp.Infrastructure.Tests/
RUN dotnet restore
COPY . .
RUN dotnet build --no-restore

FROM build AS test
WORKDIR /app
COPY --from=build /app .
CMD ["dotnet", "test", "--no-build", "--no-restore"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
WORKDIR /app
COPY --from=build /app .
CMD [ "dotnet", "watch", "--project", "ChatApp.Server" ]

FROM build AS publish
WORKDIR /app
RUN dotnet publish -o ./publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatApp.Server.dll"]
