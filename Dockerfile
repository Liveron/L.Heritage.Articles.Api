FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG GITHUB_NUGET_TOKEN
RUN dotnet nuget add source https://nuget.pkg.github.com/Liveron/index.json \
    --username Liveron \
    --store-password-in-clear-text \
    --password ${GITHUB_NUGET_TOKEN} \
    --name github

WORKDIR /app

COPY [ "*.sln", "." ]
COPY [ "*.dcproj", "." ]
COPY [ "src/L.Heritage.Articles/*.csproj", "src/L.Heritage.Articles/" ]
COPY [ "tests/L.Heritage.Articles.FunctionalTests/*.csproj", "tests/L.Heritage.Articles.FunctionalTests/" ]

RUN dotnet restore "L.Heritage.Articles.sln"

COPY . .

RUN dotnet build --configuration Release --no-restore "L.Heritage.Articles.sln"

FROM build AS test
WORKDIR /app
ENTRYPOINT [ "dotnet", "test", \
    "--no-build", \
    "--configuration", "Release", \
    "--logger", "trx;LogFileName=test-results.trx", \
    "--results-directory", "/app/TestResults", \
    "L.Heritage.Articles.sln"]