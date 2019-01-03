FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 31296
EXPOSE 44379

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY DemoShop/DemoShop.UI.csproj DemoShop/
COPY DemoShop.Core/DemoShop.Core.csproj DemoShop.Core/
RUN dotnet restore DemoShop/DemoShop.UI.csproj
COPY . .
WORKDIR /src/DemoShop
RUN dotnet build DemoShop.UI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish DemoShop.UI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DemoShop.UI.dll"]
