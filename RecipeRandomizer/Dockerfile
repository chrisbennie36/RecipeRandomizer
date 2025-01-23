FROM mcr.microsoft.com/dotnet/sdk:8.0.404 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5179

# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:8000

FROM mcr.microsoft.com/dotnet/sdk:8.0.404 AS build
WORKDIR /src
RUN mkdir -p src/Refs
COPY ["Api/RecipeRandomizer.Api.Data/RecipeRandomizer.Api.Data.csproj", "Api/Data"]
COPY ["Api/RecipeRandomizer.Api.Domain/RecipeRandomizer.Api.Domain.csproj", "Api/Domain"]
COPY ["Api/RecipeRandomizer.Api.WebApplication/RecipeRandomizer.Api.WebApplication.csproj", "Api/WebApplication"]
COPY ["Shared/RecipeRandomizer.Shared.Enums/RecipeRandomizer.Shared.Enums.csproj", "Shared/Enums"]
COPY ["Shared/RecipeRandomizer.Shared.Configuration/RecipeRandomizer.Shared.Configuration.csproj", "Shared/Configuration"]
COPY ["Shared/RecipeRandomizer.Shared.Constants/RecipeRandomizer.Shared.Constants.csproj", "Shared/Constants"]
COPY ["Refs/Utilities.dll", "Refs/Utilities"]
COPY ["Refs/Utilities.RecipeRandomizer.dll", "Refs/Utilities.RecipeRandomizer"]
COPY . .
WORKDIR "/src"
RUN dotnet restore "Api/RecipeRandomizer.Api.WebApplication/RecipeRandomizer.Api.WebApplication.csproj"
RUN dotnet build "Api/RecipeRandomizer.Api.WebApplication/RecipeRandomizer.Api.WebApplication.csproj" -c Release -o /app/build -v diag

FROM build AS publish
RUN dotnet publish "Api/RecipeRandomizer.Api.WebApplication/RecipeRandomizer.Api.WebApplication.csproj" -c Release -o /app/publish /p:UseAppHost=false -v diag

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "RecipeRandomizer.Api.WebApplication.dll" ]
