## Branches

[![v2-mongodb](https://img.shields.io/badge/branche_actuelle-v2--mongodb-green?style=for-the-badge&logo=mongodb)](https://github.com/Atinkene/TodoApi/tree/v2-mongodb)
[![main](https://img.shields.io/badge/main-InMemory%20API-blue?style=for-the-badge&logo=git)](https://github.com/Atinkene/TodoApi/tree/main)


# TodoApi v2 — ASP.NET Core Web API (MongoDB + JWT)

API REST sécurisée construite avec ASP.NET Core 9 et MongoDB. Elle permet de gérer des tâches (TodoItems) avec un système d'authentification JWT et de gestion des rôles (`admin` / `user`).

### Fonctionnalités
- CRUD complet sur les TodoItems (stockés dans MongoDB Atlas)
- Authentification JWT avec ASP.NET Core Identity
- Deux rôles : `admin` (lecture + écriture) et `user` (lecture seule)
- Documentation interactive via Scalar UI
- Déploiement cloud sur Render via Docker

---

## Architecture

```
TodoApi/
├── Controllers/
│   ├── AuthController.cs       # Register / Login → JWT
│   └── TodoItemsController.cs  # CRUD TodoItems sécurisé
├── Models/
│   ├── TodoItem.cs             # Modèle MongoDB
│   ├── TodoItemDTO.cs          # DTO exposé à l'API
│   ├── TodoDatabaseSettings.cs # Config MongoDB
│   ├── AppUser.cs              # Utilisateur Identity
│   ├── AppRole.cs              # Rôle Identity
│   └── AuthDTOs.cs             # RegisterDTO / LoginDTO
├── Services/
│   └── TodoItemsService.cs     # Logique CRUD MongoDB
├── Data/                       # A supprimer
│   └── AppDbContext.cs         # DbContext Identity (InMemory)
├── Program.cs                  # Configuration et pipeline
├── appsettings.example.json    # Template de configuration
└── Dockerfile                  # Conteneurisation
```

---

## Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [MongoDB](https://www.mongodb.com/try/download/community) local **ou** un compte [MongoDB Atlas](https://www.mongodb.com/atlas) (gratuit)
- [Git](https://git-scm.com/install/)
- [Render](https://render.com/)

---

## Installation et exécution locale

### 1. Cloner le repository

```bash
git clone https://github.com/Atinkene/TodoApi.git
cd TodoApi
git checkout v2-mongodb
```

### 2. Configurer l'environnement

Copier le fichier exemple et renseigner les valeurs :

```bash
cp appsettings.example.json appsettings.json
```

Modifier `appsettings.json` :

```json
{
  "BookStoreDatabase": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TodoDb",
    "TodoItemsCollectionName": "TodoItems"
  },
  "Jwt": {
    "Key": "VOTRE_CLE_SECRETE_MINIMUM_32_CARACTERES",
    "Issuer": "TodoApi",
    "Audience": "TodoApiUsers",
    "ExpiresInMinutes": 60
  }
}
```

> Pour MongoDB Atlas, remplacer `ConnectionString` par votre `mongodb+srv://...`

### 3. Restaurer les packages

```bash
dotnet restore
```

### 4. Lancer l'application

```bash
dotnet run
```

L'API démarre sur `http://localhost:5081`

### 5. Accéder à la documentation

Ouvrir dans le navigateur : `http://localhost:5081/scalar/v1`

---

## Endpoints

### Authentification (public)

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| POST | `/api/auth/register` | Créer un compte utilisateur |
| POST | `/api/auth/login` | Se connecter et obtenir un JWT |

### TodoItems (protégés)

| Méthode | Endpoint | Description | Rôle requis |
|---------|----------|-------------|-------------|
| GET | `/api/todoitems` | Lister tous les todos | `user` ou `admin` |
| GET | `/api/todoitems/{id}` | Obtenir un todo par ID | `user` ou `admin` |
| POST | `/api/todoitems` | Créer un todo | `admin` uniquement |
| PUT | `/api/todoitems/{id}` | Modifier un todo | `admin` uniquement |
| DELETE | `/api/todoitems/{id}` | Supprimer un todo | `admin` uniquement |

---

## Compte admin par défaut

Au démarrage, un compte administrateur est automatiquement créé :

```
Email    : admin@todoapi.com
Password : Admin123!
```

---

## Tests avec PowerShell

### 1. Login et récupération du token

```powershell
$response = Invoke-RestMethod -Method POST `
  -Uri "http://localhost:5081/api/auth/login" `
  -ContentType "application/json" `
  -Body '{"email": "admin@todoapi.com", "password": "Admin123!"}'

$token = $response.token
```

### 2. Créer un Todo (admin)

```powershell
Invoke-RestMethod -Method POST `
  -Uri "http://localhost:5081/api/todoitems" `
  -ContentType "application/json" `
  -Headers @{ Authorization = "Bearer $token" } `
  -Body '{"name": "Mon premier todo", "isComplete": false}'
```

### 3. Lire les Todos

```powershell
Invoke-RestMethod -Method GET `
  -Uri "http://localhost:5081/api/todoitems" `
  -Headers @{ Authorization = "Bearer $token" }
```

### 4. Tenter un accès sans token (doit retourner 401)

```powershell
Invoke-RestMethod -Method GET -Uri "http://localhost:5081/api/todoitems"
```

### 5. Tenter un POST avec un compte user (doit retourner 403)

```powershell
# D'abord créer un compte user
Invoke-RestMethod -Method POST `
  -Uri "http://localhost:5081/api/auth/register" `
  -ContentType "application/json" `
  -Body '{"email": "user@todoapi.com", "password": "User123!"}'

# Login user
$userResponse = Invoke-RestMethod -Method POST `
  -Uri "http://localhost:5081/api/auth/login" `
  -ContentType "application/json" `
  -Body '{"email": "user@todoapi.com", "password": "User123!"}'

$userToken = $userResponse.token

# Tenter un POST → doit retourner 403 Forbidden
Invoke-RestMethod -Method POST `
  -Uri "http://localhost:5081/api/todoitems" `
  -ContentType "application/json" `
  -Headers @{ Authorization = "Bearer $userToken" } `
  -Body '{"name": "Test interdit", "isComplete": false}'
```

---

## Déploiement Production (Render)

L'API est déployée sur Render à l'adresse :

**[https://todoapi-6bin.onrender.com](https://todoapi-6bin.onrender.com)**

Documentation Scalar : **[https://todoapi-6bin.onrender.com/scalar/v1](https://todoapi-6bin.onrender.com/scalar/v1)**

### Variables d'environnement requises sur Render

| Clé | Description |
|-----|-------------|
| `BookStoreDatabase__ConnectionString` | Connection string MongoDB Atlas |
| `BookStoreDatabase__DatabaseName` | Nom de la base de données |
| `BookStoreDatabase__TodoItemsCollectionName` | Nom de la collection |
| `Jwt__Key` | Clé secrète JWT (min. 32 caractères) |
| `Jwt__Issuer` | Issuer JWT |
| `Jwt__Audience` | Audience JWT |
| `Jwt__ExpiresInMinutes` | Durée de validité du token (minutes) |

---

## Exécution avec Docker

```bash
# Builder l'image
docker build -t todoapi .

# Lancer le conteneur
docker run -p 8080:8080 \
  -e BookStoreDatabase__ConnectionString="mongodb+srv://..." \
  -e BookStoreDatabase__DatabaseName="TodoDb" \
  -e BookStoreDatabase__TodoItemsCollectionName="TodoItems" \
  -e Jwt__Key="VOTRE_CLE_SECRETE" \
  -e Jwt__Issuer="TodoApi" \
  -e Jwt__Audience="TodoApiUsers" \
  -e Jwt__ExpiresInMinutes="60" \
  todoapi
```

---

## Technologies utilisées

| Technologie | Usage |
|-------------|-------|
| ASP.NET Core 9 | Framework principal |
| MongoDB Atlas | Base de données cloud |
| ASP.NET Core Identity | Gestion des utilisateurs et rôles |
| JWT Bearer | Authentification et autorisation |
| Scalar.AspNetCore | Documentation interactive |
| Docker | Conteneurisation |
| Render | Hébergement cloud |