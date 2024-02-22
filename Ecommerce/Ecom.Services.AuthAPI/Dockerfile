FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
# Copying the proj file then adding packages
COPY "Ecom.Services.AuthAPI.csproj" .
RUN dotnet restore "Ecom.Services.AuthAPI.csproj"
# copy project files
COPY . .
RUN dotnet publish "Ecom.Services.AuthAPI.csproj"  -c Release -o /publish
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /publish .
#ENV ASPNETCORE_ENVIRONMENT=
ENTRYPOINT ["dotnet", "Ecom.Services.AuthAPI.dll"];