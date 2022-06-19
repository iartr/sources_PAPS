FROM mcr.microsoft.com/dotnet/sdk:5.0 AS publish
WORKDIR /src
COPY ["ChatAppServer/ChatAppServer.csproj", "ChatAppServer/"]
RUN dotnet restore "ChatAppServer/ChatAppServer.csproj"
COPY . .
WORKDIR "/src/ChatAppServer"
RUN dotnet publish "ChatAppServer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatAppServer.dll"]