FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app
COPY TraceIpWebApi/TraceIpWebApi.csproj ./TraceIpWebApi/
COPY . .
WORKDIR /app/TraceIpWebApi
RUN dotnet restore
RUN dotnet publish -c Debug -o /publish

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime-env
WORKDIR /publish
EXPOSE 5000
COPY --from=build-env /publish .
ENTRYPOINT [ "dotnet","TraceIpWebApi.dll" ]
