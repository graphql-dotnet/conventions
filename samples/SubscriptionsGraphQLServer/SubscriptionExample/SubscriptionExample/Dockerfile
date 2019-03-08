FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY ["SubscriptionExample/SubscriptionExample.csproj", "SubscriptionExample/"]
RUN dotnet restore "SubscriptionExample/SubscriptionExample.csproj"
COPY . .
WORKDIR "/src/SubscriptionExample"
RUN dotnet build "SubscriptionExample.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SubscriptionExample.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SubscriptionExample.dll"]