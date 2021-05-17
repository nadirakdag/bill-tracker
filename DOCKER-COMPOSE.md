# Docker Compose

## What is it?

Docker-compose tool for defining and running multi-container docker applications. It uses [YAML](https://yaml.org) files to define configurations of application services and then with a single command, you can create and start all of the docker containers which you defined docker-compose file. 

## When and Why to use it? 

Let’s say you developed an application that uses a database, a cache (like Redis or Memcached), and finally an API gateway. 
When you want to deploy your application to docker you will need to deploy all of the needed containers. With the docker-compose, you can define all needed docker containers and network settings, volume settings, and these docker containers start order.

## What are we gonna do?

We are going to create two docker compose files one of them for the production environment and another one will be for the development environment which we are going to use docker build definition instead of image definition.

## Create docker-compose.dev.yaml

For the development environment, I am going to create PostgreSQL, Adminer, DataDog, and our application container. 

Let's create docker-compose.dev.yaml in our project directory besides Dockerfile.

### docker-compose version

After creating the docker-compose.dev.yaml first line, we are going to write is the version of the docker-compose file.

```
version: "3.3"
``` 

You can look at all of the versions, Docker Engine compatibility and how to upgrade the version from [here](https://docs.docker.com/compose/compose-file/compose-versioning/)

### network

After the define our docker-compose file version let's look at the network definition. I want to create a private network between my containers that's helping me to reach a container with a container name.

As an example, to reach my database container from my application container I am going to use my database container name. 

PS: If you do not describe a network in the docker-compose file, when you creating containers with the file it is going to create by default.

You can learn more about network definition from [here](https://docs.docker.com/compose/compose-file/compose-file-v3/#network-configuration-reference).

````
version: "3.3"

networks:
    bill-tracker:
````

This definition will create a default bridged network for us and it will be named like ```bill-tracker_bill-tracker```

### Service definition

Every container is a service. First service is gonna be PostgreSQL for our database.

````
services:
    postgres:
        image: postgres:13.2-alpine
        networks:
            - bill-tracker
        ports:
            - "5432:5432"
        environment:
            POSTGRES_PASSWORD: mysecretpassword
            POSTGRES_USER: billTracker
            POSTGRES_DB: BillTracker
    
````
Let's look at our PostgreSQL definition,
- ``services:`` is a list, and we are gonna define all of our containers as list items.
- ``Postgres:`` is our service and container name when we run it will be named like ```bill-tracker_postgres_1```
- ```image:``` is our container image in this case latest version of PostgreSQL
- ```networks:``` list of the network which we define before
- ```ports:``` list of ports we wanna open to the inside of the container
- ```environment:``` list of environment variables to add to the inside of the container

Let's create another service, this service will be our database management service, and also as same as PostgreSQL service only name, image, ports and enviroment variables will be different. 

````
    adminer:
        image: adminer
        networks:
            - bill-tracker
        ports:
            - "8080:8080"
        environment:
            ADMINER_DEFAULT_SERVER: postgresql
````

Let's create our application service,

````
    bill-tracker:
        build: .
        environment:
            ASPNETCORE_ENVIRONMENT: production
            DD_AGENT_HOST: datadog
            DD_TRACE_AGENT_PORT: 8126
            DatabaseConnection__UserName: billTracker
            DatabaseConnection__Password: mysecretpassword
            DatabaseConnection__Host: postgres
            DatabaseConnection__Port: 5432
            DatabaseConnection__Database: BillTracker
        labels:
            com.datadoghq.ad.logs: '[{"source": "csharp", "service": "bill-tracker"}]'
        ports:
            - "5000:80"
        networks:
            - bill-tracker
        depends_on:
            - postgres
            - adminer
            - datadog
````

Let's create our application service, and this service will contain three extra parameters
- ```build``` we use this parameter to build our docker image from Dockerfile
- ```labels``` this one used for adding metadata to the container
- ```depends_on``` and this one used for express dependencies to other containers


And the last service we are gonna create is a monitoring container which in this case we are gonna use DataDog. Furthermore, the difference from to other containers will be ```volumes``` with this parameter we will mount a couple of host path to the container.

````
    datadog:
        image: datadog/agent:latest
        networks:
            - bill-tracker
        ports:
            - "8126:8126/tcp"
            - "8125:8125/udp"
        volumes:
            - /var/run/docker.sock:/var/run/docker.sock:ro
            - /var/lib/docker/containers:/var/lib/docker/containers:ro
            - /proc/:/host/proc/:ro
            - /opt/datadog-agent/run:/opt/datadog-agent/run:rw
            - /sys/fs/cgroup/:/host/sys/fs/cgroup:ro
        environment:
            - "DD_API_KEY=e9e9012050b0ebd3e609e0ad10e088e8"
            - "DD_LOGS_ENABLED=true"
            - "DD_APM_ENABLED=true"
            - "DD_APM_NON_LOCAL_TRAFFIC=true"
            - "DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL=true"
            - "DD_AC_EXCLUDE=name:datadog"
            - "DD_SITE=datadoghq.eu"
````

The complete docker-compose file will be like this,

````
version: "3.3"

networks:
    bill-tracker:

services:
    postgres:
        image: postgres:13.2-alpine
        networks:
            - bill-tracker
        ports:
            - "5432:5432"
        environment:
            POSTGRES_PASSWORD: mysecretpassword
            POSTGRES_USER: billTracker
            POSTGRES_DB: BillTracker
    
    adminer:
        image: adminer
        networks:
            - bill-tracker
        ports:
            - "8080:8080"
        environment:
            ADMINER_DEFAULT_SERVER: postgresql
    
    bill-tracker:
        build: .
        environment:
            ASPNETCORE_ENVIRONMENT: production
            DD_AGENT_HOST: datadog
            DD_TRACE_AGENT_PORT: 8126
            DatabaseConnection__UserName: billTracker
            DatabaseConnection__Password: mysecretpassword
            DatabaseConnection__Host: postgres
            DatabaseConnection__Port: 5432
            DatabaseConnection__Database: BillTracker
        labels:
            com.datadoghq.ad.logs: '[{"source": "csharp", "service": "bill-tracker"}]'
        ports:
            - "5000:80"
        networks:
            - bill-tracker
        depends_on:
            - postgres
            - adminer
            - datadog

    datadog:
        image: datadog/agent:latest
        networks:
            - bill-tracker
        ports:
            - "8126:8126/tcp"
            - "8125:8125/udp"
        volumes:
            - /var/run/docker.sock:/var/run/docker.sock:ro
            - /var/lib/docker/containers:/var/lib/docker/containers:ro
            - /proc/:/host/proc/:ro
            - /opt/datadog-agent/run:/opt/datadog-agent/run:rw
            - /sys/fs/cgroup/:/host/sys/fs/cgroup:ro
        environment:
            - "DD_API_KEY=e9e9012050b0ebd3e609e0ad10e088e8"
            - "DD_LOGS_ENABLED=true"
            - "DD_APM_ENABLED=true"
            - "DD_APM_NON_LOCAL_TRAFFIC=true"
            - "DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL=true"
            - "DD_AC_EXCLUDE=name:datadog"
            - "DD_SITE=datadoghq.eu"
````

## Docker Compose Commands

### Up

The first command we will look into is ```up```.

We use up command like this 
```docker-compose -f docker-compose.dev.yaml up -d``` 

Let's break into commands,

- ```docker-compose``` is Docker Compose application
- ```-f docker-compose.dev.yaml``` with ```-f``` we say the docker-compose application use ```docker-compose.dev.yaml``` file 
- ```up``` we say the Docker Compose application to create and start containers and networks.
- ```-d``` with this parameter starts containers in the background as the detached mode

and another parameter often we use with ```docker-compose up``` is ```--build```, we use this parameter to build images before starting containers which we define in docker-compose file with ```build: .``` parameter.

### Down

The other command we will look in is ```down``` we use this command to stop and remove containers, networks, images, and volumes.

We use this command like ```docker-compose -f docker-compose.dev.yaml down``` this.



## References:

### Definitions
- [service](https://docs.docker.com/compose/compose-file/compose-file-v3/#service-configuration-reference)
- [image](https://docs.docker.com/compose/compose-file/compose-file-v3/#image)
- [networks](https://docs.docker.com/compose/compose-file/compose-file-v3/#networks)
- [ports](https://docs.docker.com/compose/compose-file/compose-file-v3/#ports)
- [environment](https://docs.docker.com/compose/compose-file/compose-file-v3/#environment)
- [build](https://docs.docker.com/compose/compose-file/compose-file-v3/#build)
- [labels](https://docs.docker.com/compose/compose-file/compose-file-v3/#labels-2)
- [depends_on](https://docs.docker.com/compose/compose-file/compose-file-v3/#depends_on)
- [volumes](https://docs.docker.com/compose/compose-file/compose-file-v3/#volumes)

### Commands
- [up](https://docs.docker.com/compose/reference/up/)
- [down](https://docs.docker.com/compose/reference/down/)
- [other commands](https://docs.docker.com/compose/reference/)