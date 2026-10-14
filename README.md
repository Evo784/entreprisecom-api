# API Entreprise.com

API REST en ASP.NET Core 10 sur le jeu SIRENE fourni. Les recherches et la suppression d'entreprises sont disponibles ; la modification reste à faire.

Un deuxième service, en Python avec Flask et pandas, expose les statistiques sur les mêmes données MySQL. Docker Compose lance trois conteneurs : `api` (ASP.NET Core), `analytics` (Flask) et `db` (MySQL).

## Lancer le projet

Prérequis : Docker Desktop avec Docker Compose. Depuis la racine du dépôt :

```powershell
docker compose up --build -d
```

Swagger UI : <http://localhost:8080/swagger>

Document OpenAPI : <http://localhost:8080/openapi/v1.json>


## Routes

| Méthode | URL | Usage |
| --- | --- | --- |
| GET | `/api/entreprises/{siren}` | Retrouver une entreprise par son SIREN à 9 chiffres |
| GET | `/api/entreprises?codeActivite=68.20B` | Chercher par code d'activité exact |
| GET | `/api/entreprises?denomination=FABRIQUE` | Chercher une dénomination contenant le texte |
| DELETE | `/api/entreprises/{siren}` | Supprimer une entreprise |

Les recherches acceptent `page=2`, `page=3`, etc. et renvoient 100 résultats au maximum par page. Une recherche sans résultat renvoie `200` et `[]`. Pour un SIREN absent, l'API renvoie `404`. Une requête invalide renvoie `400`. Une suppression réussie renvoie `204`.

Exemple : `GET http://localhost:8080/api/entreprises/000325175`

La route `PUT /api/entreprises/{siren}` prévue dans la spécification n'est pas implémentée. Le fichier fourni n'a pas de colonne d'adresse : les déménagements demanderaient aussi une autre source de données. Les requêtes SQL implémentées sont paramétrées. La création et le login ne font pas partie de cette version.

## API de statistiques

L'API Python est accessible sur le port 5001 :

| Méthode | URL | Usage |
| --- | --- | --- |
| GET | <http://localhost:5001/api/statistiques/activites> | Nombre d'entreprises par code d'activité |
| GET | <http://localhost:5001/api/statistiques/annees-creation> | Nombre d'entreprises par année de création (option) |

Chaque réponse est un objet JSON associant un code d'activité ou une année au nombre d'entreprises correspondant. Les requêtes SQL lisent uniquement les colonnes nécessaires : les regroupements et comptages sont réalisés avec pandas, sans `GROUP BY` SQL. Les codes d'activité manquants ainsi que les dates absentes ou non convertibles sont exclus des statistiques correspondantes. Les calculs sont refaits à chaque requête.

Pour appliquer une modification du code Python :

```powershell
docker compose up --build -d analytics
```

Le serveur Flask utilisé est destiné au développement local. L'environnement Python local `.venv` n'est pas nécessaire pour lancer le projet avec Docker.

## Base de données

Le fichier SQL fourni est importé automatiquement lors de l'initialisation d'un volume MySQL vide. Les données sont conservées dans le volume `mysql_data`.

Depuis Windows, MySQL est accessible sur `127.0.0.1:3307`, avec le compte `root`, sans mot de passe, et la base `sirene`. Entre conteneurs, l'adresse reste `db:3306`. Ce compte sert uniquement à cette démonstration locale, pas à un déploiement en production.
