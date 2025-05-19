FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
EXPOSE 5112
COPY ./backend.sln .
COPY ./ChatApp.Presentation/ChatApp.Presentation.csproj ./ChatApp.Presentation/
COPY ./ChatApp.Application/ChatApp.Application.csproj ./ChatApp.Application/
COPY ./ChatApp.Domain/ChatApp.Domain.csproj ./ChatApp.Domain/
COPY ./ChatApp.Application.Tests/ChatApp.Application.Tests.csproj ./ChatApp.Application.Tests/
COPY ./ChatApp.Infrastructure/ChatApp.Infrastructure.csproj ./ChatApp.Infrastructure/
COPY ./ChatApp.Infrastructure.Tests/ChatApp.Infrastructure.Tests.csproj ./ChatApp.Infrastructure.Tests/
RUN dotnet restore
COPY . .
RUN dotnet build --no-restore

FROM build AS test
WORKDIR /app
COPY --from=build /app .
CMD ["dotnet", "test", "--no-build", "--no-restore"]

FROM build AS development
WORKDIR /app
RUN dotnet dev-certs https 
CMD [ "dotnet", "watch", "--project", "ChatApp.Presentation", "--launch-profile", "https", "--no-restore" ]

FROM build AS publish
WORKDIR /app
RUN dotnet publish -o ./publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatApp.Presentation.dll"]
