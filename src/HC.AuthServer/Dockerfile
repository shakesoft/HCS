FROM mcr.microsoft.com/dotnet/sdk:10.0
RUN echo "BUILD AT: $(date)"
COPY bin/Release/net10.0/publish/ app/
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "HC.AuthServer.dll"]