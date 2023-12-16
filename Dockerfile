FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY ./VidyaBot.sln ./
COPY ./GitVersion.yml ./
COPY ./global.json ./
COPY ./.config/ ./.config/
COPY ./src/App/ ./src/App/

RUN dotnet restore ./src/App/
RUN dotnet publish ./src/App/ --configuration "Release" --output "out" --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0

RUN apt update; \
    apt install -y ffmpeg python3 python3-pip python3-venv; \
    apt upgrade -y; \
    apt clean -y

RUN adduser --disabled-login vidyabot

USER vidyabot

WORKDIR /app

COPY ./requirements.txt ./

ENV VIRTUAL_ENV=/app/.venv
RUN python3 -m venv $VIRTUAL_ENV
ENV PATH="${VIRTUAL_ENV}/bin:${PATH}"

RUN pip3 install -r requirements.txt

COPY --from=build-env /app/out/ ./

ENTRYPOINT ["dotnet", "VidyaBot.dll"]