FROM mcr.microsoft.com/dotnet/sdk:9.0 as restore
ARG GITHUB_NUGET_TOKEN
RUN dotnet nuget add source https://nuget.pkg.github.com/Liveron/index.json \
    --username Liveron \
    --store-password-in-clear-text \
    --password ${GITHUB_NUGET_TOKEN} \
    --name github
ARG BUILD_CONFIGURATION=Release
WORKDIR /app
COPY ["src/L.Heritage.Articles/*.csproj", "src/L.Heritage.Articles/"]
COPY ["tests/L.Heritage.Articles.FunctionalTests/*.csproj", "tests/L.Heritage.Articles.FunctionalTests/"]

RUN dotnet restore "tests/L.Heritage.Articles.FunctionalTests/L.Heritage.Articles.FunctionalTests.csproj"


FROM restore AS test
WORKDIR /app
COPY . .
WORKDIR /tests/L.Heritage.Articles.FunctionalTests
ENTRYPOINT [ "dotnet", "test", \
    "--logger", "trx;LogFileName=test-results.trx", \
    "--results-directory", "/app/TestResults", \
    "--no-restore" ]