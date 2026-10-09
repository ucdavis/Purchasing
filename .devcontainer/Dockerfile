FROM mcr.microsoft.com/devcontainers/dotnet:1-10.0-noble

USER root

# The base image currently includes a prerelease .NET 10 SDK. Install the exact
# stable SDK requested by the repository's global.json so builds and VS Code's
# C# tooling use the same version.
ARG DOTNET_SDK_VERSION=10.0.401
RUN curl --fail --silent --show-error --location https://dot.net/v1/dotnet-install.sh \
      --output /tmp/dotnet-install.sh \
    && bash /tmp/dotnet-install.sh \
      --version "${DOTNET_SDK_VERSION}" \
      --install-dir /usr/share/dotnet \
      --no-path \
    && rm /tmp/dotnet-install.sh

USER vscode

# see https://github.com/devcontainers/templates/blob/main/src/dotnet-mssql/.devcontainer/Dockerfile
# if you want to setup some more advance sql server stuff
