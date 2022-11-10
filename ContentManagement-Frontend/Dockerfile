FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY ["ContentManagement-Frontend/bin/Debug/net5.0/publish/", "/app"]  

ENTRYPOINT ["dotnet", "ContentManagement-Frontend.dll"]