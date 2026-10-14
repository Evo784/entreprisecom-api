#!/bin/sh
test -r /import/data.sql
{
    printf 'SET autocommit=0;\n'
    cat /import/data.sql
    printf '\nCOMMIT;\n'
} | mysql -uroot "$MYSQL_DATABASE"
