#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p 27a19a3a-9aff-42c5-b8c9-56517f1a626d -t
    fi
    cd ../
fi

docker-compose up -d
