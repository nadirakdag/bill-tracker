
# Creating small and secure containers

To create a small and secure container for this project, I will follow [this article](https://www.thorsten-hans.com/how-to-build-smaller-and-secure-docker-images-for-net5/).

## Starting with default Dockerfile

Let's build Dockerfile which was created by Visual Studio and look at the size of the first image size.

````
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/API/API.csproj", "API/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "API/API.csproj"
COPY . .
WORKDIR "src/API/"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
````

to build the docker container 

````
docker build . -t bill-tracker:0.0.2
````

After build the container image let's look at the size with 

````
docker images
````

| REPOSITORY     | TAG     | SIZE     |
| :------------- | :------ | :------- |
| `bill-tracker` | `0.0.1` | `217 MB` |

First, build with default Dockerfile 217 MB, let's move forward and start optimizing.

## Optimizing Docker base image

To create a small and secure docker image we will use [Alpine Linux](https://alpinelinux.org).
Alpine Linux is a security-oriented, lightweight Linux distribution based on musl, libc and busybox.

Switching to Alpine Linux base is very simple one only thing we need to do is change this line 

````
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
````

to this

````
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
````

and also we need to change our build base image 

````
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
````

to 

````
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
````

Our final Dockerfile will something like this

````
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["src/API/API.csproj", "API/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "API/API.csproj"
COPY . .
WORKDIR "src/API/"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
````

as the last step let build our image and look at the image size

````
docker build . -t bill-tracker:0.0.2
````

And when we look at the image size as we see that we save 105MB by switching to Alpine Linux base image

| REPOSITORY     | TAG     | SIZE     |
| :------------- | :------ | :------- |
| `bill-tracker` | `0.0.1` | `217 MB` |
| `bill-tracker` | `0.0.2` | `115 MB` |


## Optimizing dotnet publish 

In this step, we will change dotnet publish and change the output. 
To Optimize the published artifacts first thing we do build the project for Alpine-x64 runtime.
To do this we add ```--runtime alpine-x64``` this parameter to ````dotnet publish```` command.
And as the last step, we will add two more parameters, first one is ```/p:PublishTrimmed=true``` with this parameter we told to dotnet trim all dependencies that not used in the project. The second parameter we will add is ```/p:PublishSingleFile=true``` with this we were told to dotnet create a single file publish artifact and add everything that needed by the project. Last but not least to improve our docker image build time we add ```--no-restore``` with this parameter we told to dotnet do not restore package while publishing the project.

Let's look at the final version of ```dotnet publish``` command 

````
RUN dotnet publish "API.csproj"  -c Release -o /app/publish --no-restore  --runtime alpine-x64  /p:PublishTrimmed=true /p:PublishSingleFile=true
````

Also we need change our ```dotnet restore``` and ```dotnet build``` commands to use alpine-x64 runtime.
To do this we simply add a new parameter both. You can see latest versions of both.

dotnet restore 

````
RUN dotnet restore "API/API.csproj"  --runtime alpine-x64
````

dotnet build

````
RUN dotnet build "API.csproj" -c Release -o /app/build  --runtime alpine-x64
````


Our final Dockerfile will something like this
````
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["src/API/API.csproj", "API/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "API/API.csproj"  --runtime alpine-x64
COPY . .
WORKDIR "src/API/"
RUN dotnet build "API.csproj" -c Release -o /app/build  --runtime alpine-x64

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish --no-restore --runtime alpine-x64 /p:PublishTrimmed=true /p:PublishSingleFile=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
````

as the last step let build our image and look at the image size

````
docker build . -t bill-tracker:0.0.3
````

And when we look at the image size

| REPOSITORY     | TAG     | SIZE     |
| :------------- | :------ | :------- |
| `bill-tracker` | `0.0.1` | `217 MB` |
| `bill-tracker` | `0.0.2` | `115 MB` |
| `bill-tracker` | `0.0.3` | `155 MB` |


## Switching SDK base to runtime dependencies base

We are going to use ```mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine```base image as our finan image.
To do that we are changing ```FROM base AS final``` to ```mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine``` this and now we are not using our base image so we do not need that let's delete 
```
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
``` 
these definations and add WORKDIR and EXPOSE to our final stage

```
FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine AS final
EXPOSE 80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./API", "--urls", "http://0.0.0.0:80"]
```

And this is our latest Dockerfile

````
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["src/API/API.csproj", "API/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "API/API.csproj" 
COPY . .
WORKDIR "src/API/"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish --no-restore --runtime alpine-x64 /p:PublishTrimmed=true /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine AS final
EXPOSE 80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./API", "--urls", "http://0.0.0.0:80"]
````
as the last step let build our image and look at the image size

````
docker build . -t bill-tracker:0.0.4
````

And when we look at the image size and as we see that we save almost 94MB by using ```mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine``` image

| REPOSITORY     | TAG     | SIZE     |
| :------------- | :------ | :------- |
| `bill-tracker` | `0.0.1` | `217 MB` |
| `bill-tracker` | `0.0.2` | `115 MB` |
| `bill-tracker` | `0.0.3` | `155 MB` |
| `bill-tracker` | `0.0.4` | `61.7 MB`|

## Security

To see vulnerabilities of our docker image, we will use ```docker scan imagename```. 
For instance, to check our first version of the docker image, we will run ```docker scan bill-tracker:0.0.1``` command, and this command return to us something like below 

````
docker scan bill-tracker:0.0.1

Testing bill-tracker:0.0.1...

✗ Low severity vulnerability found in tar
  Description: Out-of-bounds Read
  Info: https://snyk.io/vuln/SNYK-DEBIAN10-TAR-1063001
  Introduced through: meta-common-packages@meta
  From: meta-common-packages@meta > tar@1.30+dfsg-6

✗ Low severity vulnerability found in tar
  Description: CVE-2005-2541
  Info: https://snyk.io/vuln/SNYK-DEBIAN10-TAR-312331
  Introduced through: meta-common-packages@meta
  From: meta-common-packages@meta > tar@1.30+dfsg-6

✗ Low severity vulnerability found in tar
  Description: NULL Pointer Dereference
  Info: https://snyk.io/vuln/SNYK-DEBIAN10-TAR-341203
  Introduced through: meta-common-packages@meta
  From: meta-common-packages@meta > tar@1.30+dfsg-6

✗ Low severity vulnerability found in systemd/libsystemd0
  Description: Link Following
  Info: https://snyk.io/vuln/SNYK-DEBIAN10-SYSTEMD-305144
  Introduced through: systemd/libsystemd0@241-7~deb10u7, util-linux/bsdutils@1:2.33.1-0.1, apt@1.8.2.2, util-linux/mount@2.33.1-0.1, systemd/libudev1@241-7~deb10u7
  From: systemd/libsystemd0@241-7~deb10u7
  From: util-linux/bsdutils@1:2.33.1-0.1 > systemd/libsystemd0@241-7~deb10u7
  From: apt@1.8.2.2 > apt/libapt-pkg5.0@1.8.2.2 > systemd/libsystemd0@241-7~deb10u7
  and 4 more...

Organization:      akdag.nadir
Package manager:   deb
Project name:      docker-image|bill-tracker
Docker image:      bill-tracker:0.0.1
Platform:          linux/amd64
Licenses:          enabled

Tested 94 dependencies for known issues, found 61 issues.
````

After lookup every version of our docker image, I created the table in the bellow and the table shows us that after we converted to alpine all of our vulnerabilities is solved also shows us that our dependencies count is less than before


| VERSION             | TESTED DEPENDENCIES | FOUND ISSUES   |
| :------------------ | :------------------ | :------------- |
| `bill-tracker:0.0.1`| 94                  | 51             |
| `bill-tracker:0.0.2`| 23                  | 0              |
| `bill-tracker:0.0.3`| 23                  | 0              |
| `bill-tracker:0.0.4`| 23                  | 0              |