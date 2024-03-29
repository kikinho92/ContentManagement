FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["ContentManagement-Frontend/ContentManagement-Frontend.csproj", "ContentManagement-Frontend/"]
RUN dotnet restore "ContentManagement-Frontend/ContentManagement-Frontend.csproj"
COPY . .
WORKDIR "/src/ContentManagement-Frontend"
RUN dotnet build "ContentManagement-Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContentManagement-Frontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContentManagement-Frontend.dll"]