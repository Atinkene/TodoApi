
## Branches

[![main](https://img.shields.io/badge/branche_actuelle-main-blue?style=for-the-badge&logo=git)](https://github.com/Atinkene/TodoApi/tree/main)
[![v2-mongodb](https://img.shields.io/badge/v2--mongodb-MongoDB%20%2B%20JWT-green?style=for-the-badge&logo=mongodb)](https://github.com/Atinkene/TodoApi/tree/v2-mongodb)

# TodoApi v1 — ASP.NET Core Web API (InMemory)


## Description

API REST construite avec ASP.NET Core 9 permettant de gérer des tâches (TodoItems) stockées en mémoire (InMemory). Cette version suit le tutoriel officiel Microsoft pour la création d'une Web API avec ASP.NET Core.

### Fonctionnalités
- CRUD complet sur les TodoItems
- Base de données InMemory (Entity Framework Core)
- Exposition du champ `Secret` masqué via DTO
- Documentation interactive via Scalar UI

---

## Architecture

```
TodoApi/
├── Controllers/
│   └── TodoItemsController.cs  # CRUD TodoItems
├── Models/
│   ├── TodoItem.cs             # Modèle de données
│   ├── TodoItemDTO.cs          # DTO (masque le champ Secret)
│   └── TodoContext.cs          # DbContext EF Core InMemory
├── Program.cs                  # Configuration et pipeline
└── appsettings.json            # Configuration de l'application
```

---

## Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- Git

---

## Installation et exécution

### 1. Cloner le repository

```bash
git clone https://github.com/Atinkene/TodoApi.git
cd TodoApi
```

> La branche `main` est sélectionnée par défaut.

### 2. Restaurer les packages

```bash
dotnet restore
```

### 3. Lancer l'application

```bash
dotnet run
```

L'API démarre sur `http://localhost:5081`

### 4. Accéder à la documentation

Ouvrir dans le navigateur : `http://localhost:5081/scalar/v1`

---

## Endpoints

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| GET | `/api/todoitems` | Lister tous les todos |
| GET | `/api/todoitems/{id}` | Obtenir un todo par ID |
| POST | `/api/todoitems` | Créer un todo |
| PUT | `/api/todoitems/{id}` | Modifier un todo |
| DELETE | `/api/todoitems/{id}` | Supprimer un todo |

---

## Tests avec PowerShell

### Créer un Todo

```powershell
Invoke-RestMethod -Method POST `
  -Uri "http://localhost:5081/api/todoitems" `
  -ContentType "application/json" `
  -Body '{"name": "Mon premier todo", "isComplete": false}'
```

### Lire tous les Todos

```powershell
Invoke-RestMethod -Method GET -Uri "http://localhost:5081/api/todoitems"
```

### Modifier un Todo

```powershell
Invoke-RestMethod -Method PUT `
  -Uri "http://localhost:5081/api/todoitems/1" `
  -ContentType "application/json" `
  -Body '{"id": 1, "name": "Todo modifié", "isComplete": true}'
```

### Supprimer un Todo

```powershell
Invoke-RestMethod -Method DELETE -Uri "http://localhost:5081/api/todoitems/1"
```

---

## Notes

- Les données sont stockées en mémoire — elles sont perdues au redémarrage de l'application.
- Le champ `Secret` du modèle `TodoItem` est masqué via le `TodoItemDTO` et n'est jamais exposé dans les réponses de l'API.
- Pour la version avec MongoDB et sécurisation JWT, consulter la branche `v2-mongodb`.
