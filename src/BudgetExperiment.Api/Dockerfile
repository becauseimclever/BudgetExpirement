# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY *.sln .
COPY src/BudgetExperiment.Api/*.csproj src/BudgetExperiment.Api/
COPY src/BudgetExperiment.Client/*.csproj src/BudgetExperiment.Client/
RUN dotnet restore

# Copy the remaining files and build the application
COPY . .
WORKDIR /app/src/BudgetExperiment.Api
RUN dotnet publish -c Release -o /out

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /out ./

# Expose the port and run the application
EXPOSE 80
ENTRYPOINT ["dotnet", "BudgetExperiment.Api.dll"]
