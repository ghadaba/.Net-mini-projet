-- ============================================
-- Script SQL Server - Projet tp2_oolab (Articles / Catégories)
-- ============================================
-- Utilisation :
-- 1. Créer la base (si besoin) : voir commandes ci-dessous
-- 2. Soit exécuter les migrations EF depuis l'app (recommandé)
-- 3. Soit exécuter manuellement ce script après avoir créé la base
-- ============================================

-- Option A : Créer la base de données (si elle n'existe pas)
-- Exécuter dans une requête sur le serveur SQL Server (master ou autre)
/*
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'tp2oolab')
BEGIN
    CREATE DATABASE tp2oolab;
END
GO
USE tp2oolab;
GO
*/

-- Option B : Si vous utilisez LocalDB (chaîne par défaut du projet)
-- La base est créée automatiquement au premier run.
-- Pour créer manuellement la base LocalDB :
--   1. Ouvrir une invite de commandes
--   2. Exécuter : sqllocaldb create "MSSQLLocalDB" (si pas déjà créé)
--   3. sqllocaldb start "MSSQLLocalDB"
-- Puis lancer l'app ou appliquer les migrations (voir commandes en bas).

-- ============================================
-- Tables (équivalent des migrations EF)
-- Exécuter seulement si vous n'utilisez PAS dotnet ef database update
-- ============================================

-- Supprimer les anciennes tables si vous migrez depuis Films/Genres
IF OBJECT_ID(N'dbo.Movies', N'U') IS NOT NULL
    DROP TABLE dbo.Movies;
IF OBJECT_ID(N'dbo.Genres', N'U') IS NOT NULL
    DROP TABLE dbo.Genres;

-- Table Catégories
IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories (
        Id   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(MAX)    NOT NULL
    );
END
GO

-- Table Articles
IF OBJECT_ID(N'dbo.Articles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Articles (
        Id          INT              NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Title       NVARCHAR(200)    NOT NULL,
        Description NVARCHAR(2000)   NULL,
        DateCreated DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        CategoryId  UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT FK_Articles_Categories FOREIGN KEY (CategoryId)
            REFERENCES dbo.Categories(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_Articles_CategoryId ON dbo.Articles (CategoryId);
END
GO

-- ============================================
-- Données de démonstration (optionnel)
-- ============================================
/*
INSERT INTO dbo.Categories (Id, Name) VALUES
    (NEWID(), N'Actualité'),
    (NEWID(), N'Technologie'),
    (NEWID(), N'Sport');

DECLARE @catId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM dbo.Categories);

INSERT INTO dbo.Articles (Title, Description, DateCreated, CategoryId) VALUES
    (N'Premier article', N'Description du premier article.', GETUTCDATE(), @catId);
GO
*/

-- ============================================
-- Commandes à taper (PowerShell / CMD) pour la base de données
-- ============================================
--
-- 1) Créer la migration (déjà fait dans le projet) :
--    dotnet ef migrations add ReplaceWithArticlesAndCategories
--
-- 2) Mettre à jour la base de données (applique les migrations) :
--    dotnet ef database update
--
-- 3) Connexion dans appsettings.json (déjà présente) :
--    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=tp2oolab;Trusted_Connection=True;"
--
-- 4) Pour SQL Server complet (pas LocalDB), modifier appsettings.json par exemple :
--    "Server=.;Database=tp2oolab;Trusted_Connection=True;"
--    ou
--    "Server=localhost;Database=tp2oolab;User Id=sa;Password=VotreMotDePasse;TrustServerCertificate=True;"
--
-- 5) Générer un script SQL à partir des migrations (sans exécuter) :
--    dotnet ef migrations script -o Scripts/Generated_Migration.sql
--
