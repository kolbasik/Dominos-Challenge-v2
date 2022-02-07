#!/usr/bin/env bash
set -euox pipefail

APP=dominos-pizza-vouchers-api
docker build -t $APP -f Dockerfile .
docker run -it -p 5000:80 $APP
