#!/usr/bin/env bash
set -euox pipefail

APP=sk-dominos-pizza-vouchers-api
heroku container:login
heroku container:push web -a $APP
heroku container:release web -a $APP
