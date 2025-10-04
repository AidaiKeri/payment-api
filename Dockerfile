FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY PaymentApi/PaymentApi.csproj ./
RUN dotnet restore PaymentApi.csproj

COPY PaymentApi/. ./

RUN dotnet publish PaymentApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "PaymentApi.dll"]

