#AS build

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as build
WORKDIR /build

# copy csproj and restore as distinct layers
COPY ./DateApp.API/DateApp.API.csproj ./DateApp.API/DateApp.API.csproj
RUN dotnet restore ./DateApp.API/DateApp.API.csproj

COPY ./DateApp.Business/DateApp.Business.csproj ./DateApp.Business/DateApp.Business.csproj
RUN dotnet restore ./DateApp.Business/DateApp.Business.csproj

COPY ./DateApp.Core/DateApp.Core.csproj ./DateApp.Core/DateApp.Core.csproj
RUN dotnet restore ./DateApp.Core/DateApp.Core.csproj

COPY ./DateApp.DAL/DateApp.DAL.csproj ./DateApp.DAL/DateApp.DAL.csproj
RUN dotnet restore ./DateApp.DAL/DateApp.DAL.csproj

COPY . .
RUN dotnet publish ./DateApp.API/DateApp.API.csproj --output /publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 as runtime
WORKDIR /publish
COPY --from=build /publish .
# CMD bash -c "dotnet DateApp.API.dll"


ENTRYPOINT ["dotnet", "DateApp.API.dll"]