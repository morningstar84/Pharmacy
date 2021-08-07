#!/bin/sh
# wait-db.sh

set -e

host="$1"
shift
cmd="$@"

echo "Checking if postgres is up and running"
echo "[CMD]: Executing -> ${cmd}"

# Wait until postgres logs that it's ready (or timeout after 60s)
COUNTER=0
while !(nc -z ${host} 5432) && [[ ${COUNTER} -lt 60 ]] ; do
    sleep 2
    let COUNTER+=2
    echo "Waiting for postgres to initialize... ($COUNTER seconds so far)"
done

echo "Postgres DB is up - now starting webservice"
exec ${cmd}

