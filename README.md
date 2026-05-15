# Tp2-oolab — Mini-projet ASP.NET Core MVC

Application web de gestion d’**articles** et de **catégories**, développée en **ASP.NET Core 8 MVC** avec **Entity Framework Core**, **SQL Server** et **ASP.NET Core Identity** pour l’authentification et l’autorisation par rôles.

---

## Table des matières

1. [Vue d’ensemble](#vue-densemble)
2. [Technologies](#technologies)
3. [Architecture du projet](#architecture-du-projet)
4. [Modèle de données](#modèle-de-données)
5. [Authentification et autorisation (TP5 & TP6)](#authentification-et-autorisation-tp5--tp6)
6. [Travaux réalisés par TP](#travaux-réalisés-par-tp)
7. [Installation et exécution](#installation-et-exécution)
8. [Comptes de test](#comptes-de-test)
9. [Structure des dossiers](#structure-des-dossiers)

---

## Fonctionnalité bonus (+)

- **Tableau de bord** (`/`) — statistiques LINQ, articles par catégorie, 5 derniers articles
- **Mon profil** (`/Account/Profile`) — identifiant, rôle, droits (lecture ou CRUD)

---

## Vue d’ensemble

Ce projet regroupe les compétences des TP de **Frameworks de développement** :

| TP | Thème | Intégration dans le projet |
|----|--------|----------------------------|
| **TP4** | Architecture services, LINQ, layout | Base MVC, EF Core, personnalisation du layout |
| **TP5** | ASP.NET Core Identity | Connexion, inscription, tables Identity, `AccountController` |
| **TP6** | Autorisation par rôles | Rôles **Admin** / **User**, CRUD restreint aux admins |

L’utilisateur **non connecté** peut accéder à l’accueil et aux pages de connexion/inscription.  
Une fois connecté :

- **User** (utilisateur simple) : lecture seule — liste et détails des **articles** et **catégories**.
- **Admin** : CRUD complet sur articles et catégories.

---

## Technologies

- **.NET 8** — ASP.NET Core MVC
- **Entity Framework Core 8** — ORM, migrations
- **SQL Server** (LocalDB) — base `tp2oolab`
- **ASP.NET Core Identity** — utilisateurs, mots de passe, rôles
- **Bootstrap 5** — interface

---

## Architecture du projet

```
Tp2-oolab/
├── Controllers/          # MVC : Home, Articles, Categories, Account
├── Models/               # Article, Category, ViewModels (Login, Register)
├── data/                 # ApplicationDbContext (Identity + métier), DbInitializer
├── Constants/            # RoleNames (Admin, User)
├── Views/                # Razor (.cshtml)
├── Migrations/           # Historique EF Core
├── Program.cs            # DI, Identity, pipeline HTTP, seed des rôles
└── appsettings.json      # Chaîne de connexion SQL Server
```

Le **DbContext** hérite de `IdentityDbContext<IdentityUser>` : une seule base contient les tables métier (`Articles`, `Categories`) et les tables Identity (`AspNetUsers`, `AspNetRoles`, etc.).

---

## Modèle de données

### Category

| Propriété | Type | Description |
|-----------|------|-------------|
| `Id` | `Guid` | Clé primaire |
| `Name` | `string` | Nom de la catégorie |
| `Articles` | collection | Articles liés (1-N) |

### Article

| Propriété | Type | Description |
|-----------|------|-------------|
| `Id` | `int` | Clé primaire (identity) |
| `Title` | `string` | Titre |
| `Description` | `string?` | Description |
| `DateCreated` | `DateTime` | Date de création |
| `CategoryId` | `Guid` | FK vers Category |

---

## Authentification et autorisation (TP5 & TP6)

### Configuration Identity (`Program.cs`)

Alignée sur le **TP5** :

- `AddIdentity<IdentityUser, IdentityRole>` avec règles de mot de passe (6 caractères, chiffre, minuscule, etc.)
- `SignIn.RequireConfirmedAccount = false` pour une connexion immédiate sans e-mail
- `UseAuthentication()` puis `UseAuthorization()` dans le bon ordre
- Cookies : redirection vers `/Account/Login` et `/Account/AccessDenied`

### Rôles (TP6)

| Rôle | Droits |
|------|--------|
| **Admin** | Create, Read, Update, Delete sur Articles et Categories |
| **User** | Read seulement (Index, Details) |

### Implémentation

1. **`Constants/RoleNames.cs`** — noms des rôles `Admin` et `User`.
2. **`data/DbInitializer.cs`** — au démarrage : création des rôles + compte administrateur par défaut.
3. **Contrôleurs** — `[Authorize]` sur `ArticlesController` et `CategoriesController` ; `[Authorize(Roles = RoleNames.Admin)]` sur les actions Create, Edit, Delete.
4. **Inscription** — tout nouvel utilisateur reçoit automatiquement le rôle **User**.
5. **Vues** — boutons Créer / Modifier / Supprimer masqués si l’utilisateur n’est pas Admin.

### Schéma des droits

```
                    ┌─────────────┐
                    │  Anonyme    │
                    └──────┬──────┘
                           │ Login / Register
              ┌────────────┴────────────┐
              ▼                         ▼
       ┌─────────────┐           ┌─────────────┐
       │    User     │           │    Admin    │
       │  (lecture)  │           │    CRUD     │
       └─────────────┘           └─────────────┘
```

---

## Travaux réalisés par TP

### TP4 — Changement d’architecture et personnalisation du layout

**Objectifs du TP :** découpler la logique via des services, utiliser LINQ, personnaliser le layout.

**Dans ce projet :**

- Application **MVC** avec séparation **Controllers / Models / Views**.
- Connexion **EF Core + SQL Server** (`appsettings.json`, `AddDbContext` dans `Program.cs`).
- **Layout** personnalisé (`Views/Shared/_Layout.cshtml`) : navigation Articles / Catégories, zone connexion/déconnexion, badges de rôle.
- Évolution du domaine : passage d’un modèle type Films/Genres vers **Articles / Categories** avec relations et recherche (filtre par titre, catégorie).

*Note :* le TP4 prévoit un dossier `Services/` avec interfaces (ex. `IMovieService`). Le projet actuel accède au contexte depuis les contrôleurs ; une évolution possible serait d’extraire `IArticleService` / `ICategoryService` comme dans le TP4.

### TP5 — User Identity with ASP.NET Core

**Objectifs du TP :** Identity, migrations, register/login, extension éventuelle d’`IdentityUser`.

**Dans ce projet :**

- Packages `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.
- `ApplicationDbContext : IdentityDbContext<IdentityUser>`.
- Migration `AddIdentity` — tables `AspNetUsers`, `AspNetRoles`, etc.
- **`AccountController`** : Login, Register, Logout (vues dédiées, pas de scaffold Identity area).
- ViewModels `LoginViewModel`, `RegisterViewModel`.
- Politique mot de passe configurée dans `Program.cs` (comme l’énoncé TP5).

### TP6 — Authentification et autorisation

**Objectifs du TP :** rôles, `[Authorize]`, restriction CRUD aux administrateurs.

**Dans ce projet (partie MVC, sans JWT Web API) :**

- `RoleManager<IdentityRole>` injecté dans `AccountController` et utilisé au seed.
- Rôles **Admin** et **User** créés au démarrage.
- Autorisation **déclarative** sur les contrôleurs (`[Authorize]`, `[Authorize(Roles = "Admin")]`).
- Page **Accès refusé** (`/Account/AccessDenied`).
- Comportement conforme à l’énoncé : *« un utilisateur administratif peut créer/modifier/supprimer ; un non-administrateur est autorisé uniquement à lire »*.

*Le TP6 décrit aussi une Web API avec JWT ; ce projet est en MVC avec cookies. La même logique de rôles s’applique.*

---

## Installation et exécution

### Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (inclus avec Visual Studio) ou SQL Server Express

### Commandes

```powershell
cd "C:\Users\ThinkPad E15\source\repos\Tp2-oolab"

# Restaurer et compiler
dotnet restore
dotnet build

# Appliquer les migrations (si la base n'existe pas)
dotnet ef database update

# Lancer l'application
dotnet run
```

Ouvrir l’URL affichée (souvent `https://localhost:7xxx`).

### Chaîne de connexion

Fichier `appsettings.json` :

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=tp2oolab;Trusted_Connection=True;"
}
```

Adapter le serveur si vous utilisez une instance SQL Server différente.

---

## Comptes de test

| Compte | Mot de passe | Rôle | Usage |
|--------|--------------|------|--------|
| `admin` | `Admin123!` | **Admin** | CRUD complet (créé au premier démarrage) |
| *(inscription)* | *(votre choix)* | **User** | Lecture seule après inscription |

**Scénario de test :**

1. Se connecter en **admin** → créer/modifier/supprimer un article.
2. S’inscrire avec un nouveau compte → vérifier le badge **User** dans la barre de navigation.
3. Tenter d’ouvrir `/Articles/Create` en tant que User → redirection vers **Accès refusé**.
4. Consulter Index et Details en tant que User → OK.

---

## Structure des dossiers

| Dossier / fichier | Rôle |
|-------------------|------|
| `Controllers/ArticlesController.cs` | CRUD articles + recherche |
| `Controllers/CategoriesController.cs` | CRUD catégories + recherche |
| `Controllers/AccountController.cs` | Authentification |
| `Controllers/HomeController.cs` | Page d’accueil (publique) |
| `data/ApplicationDbContext.cs` | Contexte EF + Identity |
| `data/DbInitializer.cs` | Seed rôles et admin |
| `Constants/RoleNames.cs` | Constantes de rôles |
| `Migrations/` | Schéma base de données |
| `Views/Account/` | Login, Register, AccessDenied |
| `Views/Articles/`, `Views/Categories/` | Vues CRUD avec UI adaptée au rôle |

---

## Auteur et contexte

Projet réalisé dans le cadre des TP **Frameworks de développement** (TP4, TP5, TP6) — gestion d’articles avec authentification Identity et autorisation par rôles Admin/User.
