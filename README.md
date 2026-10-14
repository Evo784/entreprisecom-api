# API Entreprise.com

API REST en ASP.NET Core 10 sur le jeu SIRENE fourni. Les recherches et la suppression d'entreprises sont disponibles ; la modification reste à faire.

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

MySQL n'expose pas de port vers l'hôte. Le compte root sans mot de passe sert uniquement à cette démonstration locale, pas à un déploiement en production.
