#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/InglemoorCodingComputing.Evaluator/InglemoorCodingComputing.Evaluator.csproj", "src/InglemoorCodingComputing.Evaluator/"]
COPY ["src/InglemoorCodingComputing.Evaluator.Shared/InglemoorCodingComputing.Evaluator.Shared.csproj", "src/InglemoorCodingComputing.Evaluator.Shared/"]
RUN dotnet restore "src/InglemoorCodingComputing.Evaluator/InglemoorCodingComputing.Evaluator.csproj"
COPY . .
WORKDIR "/src/src/InglemoorCodingComputing.Evaluator"
RUN curl https://github.com/tailwindlabs/tailwindcss/releases/download/v3.2.4/tailwindcss-linux-x64 -o /tailwindcss
RUN sh /tailwindcss -i Styles/app.css -o wwwroot/tailwind.css --watch
RUN dotnet build "InglemoorCodingComputing.Evaluator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InglemoorCodingComputing.Evaluator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InglemoorCodingComputing.Evaluator.dll"]