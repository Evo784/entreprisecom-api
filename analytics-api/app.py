import os
import pymysql
from flask import Flask
import pandas as pd

app = Flask(__name__)

def get_connection():
    return pymysql.connect(
        host=os.environ["DB_HOST"],
        user=os.environ["DB_USER"],
        database=os.environ["DB_NAME"],
        cursorclass=pymysql.cursors.DictCursor
    )

def load_activity_codes():
    with get_connection() as connection:
        with connection.cursor() as cursor:
            cursor.execute("SELECT activite_principale_unite_legale FROM mytable")
            return pd.DataFrame(cursor.fetchall())


def count_by_activity():
    data = load_activity_codes()
    counts = data["activite_principale_unite_legale"].value_counts()
    return counts



@app.get("/api/statistiques/activites")
def activity_statistics():
    counts = count_by_activity()
    return counts.to_dict()

def load_creation_dates():
    with get_connection() as connection:
        with connection.cursor() as cursor:
            cursor.execute("SELECT date_creation_unite_legale FROM mytable")
            return pd.DataFrame(cursor.fetchall())

def count_by_creation_year():
    data = load_creation_dates()
    dates = pd.to_datetime(data["date_creation_unite_legale"], errors="coerce")
    years = dates.dropna().dt.year
    counts = years.value_counts()
    return counts


@app.get("/api/statistiques/annees-creation")
def creation_year_statistics():
    counts = count_by_creation_year()
    return counts.to_dict()

