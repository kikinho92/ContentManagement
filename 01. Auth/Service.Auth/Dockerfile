FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["01. Auth/Api.Auth/Api.Auth.csproj", "01. Auth/Api.Auth/"]  
COPY ["01. Auth/Sdk.Auth/Sdk.Auth.csproj", "01. Auth/Sdk.Auth/"]  
COPY ["01. Auth/Service.Auth/Service.Auth.csproj", "01. Auth/Service.Auth/"]  
COPY ["01. Auth/Test.Service.Auth/Test.Service.Auth.csproj", "01. Auth/Test.Service.Auth/"]

COPY ["02. User/Api.User/Api.User.csproj", "02. User/Api.User/"]  
COPY ["02. User/Sdk.User/Sdk.User.csproj", "02. User/Sdk.User/"]  
COPY ["02. User/Service.User/Service.User.csproj", "02. User/Service.User/"]  
COPY ["02. User/Test.Service.User/Test.Service.User.csproj", "02. User/Test.Service.User/"]

COPY ["03. Content/Api.Content/Api.Content.csproj", "03. Content/Api.Content/"]  
COPY ["03. Content/Sdk.Content/Sdk.Content.csproj", "03. Content/Sdk.Content/"]  
COPY ["03. Content/Service.Content/Service.Content.csproj", "03. Content/Service.Content/"]  
COPY ["03. Content/Test.Service.Content/Test.Service.Content.csproj", "03. Content/Test.Service.Content/"]

COPY ["04. Integration/Api.Integration/Api.Integration.csproj", "04. Integration/Api.Integration/"]  
COPY ["04. Integration/Sdk.Integration/Sdk.Integration.csproj", "04. Integration/Sdk.Integration/"]  
COPY ["04. Integration/Service.Integration/Service.Integration.csproj", "04. Integration/Service.Integration/"]  
COPY ["04. Integration/Test.Service.Integration/Test.Service.Integration.csproj", "04. Integration/Test.Service.Integration/"]

COPY ["Library.Common/Library.Common.csproj", "Library.Common/"]
COPY ["ContentManagement-Frontend/ContentManagement-Frontend.csproj", "ContentManagement-Frontend/"]

RUN dotnet restore "01. Auth/Service.Auth/Service.Auth.csproj"  
COPY . .

WORKDIR "/src/01. Auth/Service.Auth"  
RUN dotnet build "Service.Auth.csproj" -c Release -o /app/build

FROM build AS publish  
RUN dotnet publish "Service.Auth.csproj" -c Release -o /app/publish  
FROM base AS final  
WORKDIR /app  
COPY --from=publish /app/publish .  
ENTRYPOINT ["dotnet", "Service.Auth.dll"]

