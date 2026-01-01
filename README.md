# Doc.ECM Windows Service Example - Visual Studio C#

This is a simple example of a **Windows Service** designed to interact with the `doc.ecm` API.  
`doc.ecm` is a Document Electronic Management system that allows organizations to manage, modify, and integrate documents programmatically.

This project is aimed at advanced users and partners who want to automate tasks or integrate `doc.ecm` with external systems, such as:

- **SAP**
- **Digital signature providers**
- **Custom document workflows**
- **External databases or ERP systems**

## 🛠 What This Example Shows

- How to create and run a basic Windows Service in .NET
- How to authenticate and communicate with the `doc.ecm` API
- A simple structure to customize your own service logic

## 📁 Project Structure
* WindowsServiceExample is your windows service. Check in program.cs the start routine. Use Debug to code and always compile in Release (x64).
  
* Doc.ECM.ApiHelper.Static is our example API Helper. With this class you can query and use our API endpoints. Check the API swagger for more information about our endpoints.

## Limits
Be sure to use one token until the expiration date before asking for a new one. This project already handle this for you. There is a limit that we are going to introduce in the future.

Respect the call limit. Do not perform lots of API calls (1440 max per day). The only exception is the Object Save endpoint, that can be called as many times as you want, but only to perform real updates (that is, changing at least one value).

## 🚀 Getting Started
1. Clone the repository:
   ```bash
   git clone https://github.com/DocSeriesSA/WindowsServiceExampleProject.git
2. Run the project once, then update the configuration file with your actual doc.ecm API credentials and endpoints.
3. Build and install the service using sc.exe or PowerShell.
4. Start the service and monitor the logs to see how it interacts with doc.ecm.

---- English details----

# Doc.ECM Windows Service Example — Visual Studio C#

An example project showing how to build a Windows Service to integrate and automate actions with the Doc.ECM API. This service demonstrates advanced integration patterns: authentication and token management, synchronization of internal tables, document export to an external API, and scheduling via FluentScheduler.

This README provides a comprehensive explanation of the project, plus instructions to configure, build, deploy, run and extend the service.

---

## Table of contents
- [Project goal](#project-goal)
- [Typical use cases](#typical-use-cases)
- [Prerequisites](#prerequisites)
- [Repository structure](#repository-structure)
- [Configuration](#configuration)
  - [Example config.json](#example-configjson)
  - [Important fields explained](#important-fields-explained)
- [Build and run](#build-and-run)
  - [Run in Debug mode (development)](#run-in-debug-mode-development)
  - [Build in Release](#build-in-release)
  - [Install the Windows Service](#install-the-windows-service)
  - [Start / Stop / Remove the service](#start--stop--remove-the-service)
- [Architecture and components](#architecture-and-components)
  - [DocECMApiHelper](#docecmapihelper)
  - [Jobs and Processes](#jobs-and-processes)
  - [MyExternalApiService](#myexternalapiservice)
  - [ConfigHelper](#confighelper)
  - [ServiceLogger](#servicelogger)
- [Practical examples](#practical-examples)
  - [Add a new job](#add-a-new-job)
  - [Example invoice export](#example-invoice-export)
- [Best practices and constraints](#best-practices-and-constraints)
- [Troubleshooting](#troubleshooting)
- [Contribution](#contribution)

---

## Project goal
This repository shows a Windows Service pattern able to:
- Connect to Doc.ECM with token management.
- Schedule recurring processing (FluentScheduler).
- Execute business jobs: export documents, synchronize internal tables.
- Communicate with a secure external API (authentication + retries with Polly).
- Gracefully handle shutdown and startup.

The code is a skeleton intended to be adapted (e.g. connection to SAP, signature provider integration, specific business processes).

---

## Typical use cases
- Periodically export invoices from Doc.ECM to an external ERP.
- Synchronize Doc.ECM internal tables with data from an external API.
- Trigger business workflows on documents (status changes, comments, etc.).
- Advanced integrations (digital signature, document transformations).

---

## Prerequisites
- Visual Studio (e.g. 2019/2022) compatible with .NET Framework 4.8.1
- .NET Framework Developer Pack 4.8.1 installed
- Network access to the Doc.ECM API and to the external API (if used)
- Administrator rights to install a Windows Service

The file `WindowsServiceExample/App.config` indicates the target runtime: .NETFramework,Version=v4.8.1.

---

## Repository structure (essential)
- WindowsServiceExample/  
  - Program.cs — entrypoint; in DEBUG runs a processing method for development.
  - WindowsService.cs — service lifecycle (OnStart, OnStop, InitService).
  - Jobs/ — JobRegistry and job classes (JobExportInvoices, JobSyncTables).
  - Traitments/ — concrete business logic (TraitmentExportInvoice, TraitmentSyncInternalTableData).
  - Services/MyExternalApiService.cs — wrapper to an external API (token, retries).
  - ConfigHelper/ — read/write `config.json`.
  - ServiceLogger/ — logging helper (LogHelper).
- Doc.ECM.ApiHelper.Static/ — static helper DocECMApiHelper and DTOs (operations towards Doc.ECM).

---

## Configuration

The service reads a `config.json` file located in the executable directory (created automatically if missing). The `YourCompanyConfig` class contains notably:
- `DocECMParameters` (DocECMAPIParametersDTO) — access parameters for Doc.ECM.
- `ProcessParameters` — e.g. JobScheduleInMinutes, LastUpdatedAt.
- `YourAPIConfig` — parameters for the external API (MyAPIApiUrl, MyAPIUsername, MyAPIPassword).

### Example config.json
Copy and paste this JSON into the `config.json` file and fill the values:

```json
{
  "DocECMParameters": {
    "ApiUrl": "https://your-docecm.example.com",
    "Username": "yourDocECMUser",
    "Password": "yourPassword",
    "WebSiteUrl": "https://your-website.example.com"
  },
  "ProcessParameters": {
    "JobScheduleInMinutes": 30,
    "LastUpdatedAt": "1940-01-01T00:00:00.0000000+00:00"
  },
  "YourAPIConfig": {
    "MyAPIUsername": "apiUser",
    "MyAPIPassword": "apiPassword",
    "MyAPIApiUrl": "https://external.api.example.com"
  }
}
```

### Important fields explained
- DocECMParameters.ApiUrl : base URL of the Doc.ECM API (e.g. https://api.docecm.example.com)
- DocECMParameters.Username / Password : credentials to obtain a token via /token
- DocECMParameters.WebSiteUrl : the Doc.ECM site URL (used by the library if required)
- ProcessParameters.JobScheduleInMinutes : interval (minutes) for job scheduling
- YourAPIConfig.* : URL and credentials for your external API

---

## Build and run

### Run in Debug mode (development)
In DEBUG (Program.cs), the service runs a processing method directly for local development:
- Open Visual Studio in Debug mode.
- Place a breakpoint in `TraitmentSyncInternalTableData.SyncTables()` or `TraitmentExportInvoice.ProcessDocumentsToExport()`.
- Run the application (F5). The code calls the chosen method and then stays blocked (Thread.Sleep Infinite) for inspection.

Note: This mode avoids installing the service for development.

### Build in Release
- Choose the Release configuration.
- Platform: x64 (recommended for production per the original README).
- Build > Build Solution

### Install the Windows Service
After building, you will get an executable in `bin\Release
et48` (or `bin\Release` depending on configuration). To install the service:

- With sc.exe (Command Prompt as administrator):
  sc create YourCompanyService binPath= "C:\path\to\WindowsServiceExample.exe" start= auto DisplayName= "YourCompanyService"

- Or with PowerShell (run as admin):
  New-Service -Name "YourCompanyService" -BinaryPathName "C:\path\to\WindowsServiceExample.exe" -DisplayName "YourCompanyService" -StartupType Automatic

After creation:
- Start:  net start YourCompanyService
- Stop:   net stop YourCompanyService
- Remove: sc delete YourCompanyService  (or Remove-Service in PowerShell)

Note: Before installing, ensure `config.json` is present next to the executable and properly filled.

---

## Architecture and components

### DocECMApiHelper
- Main file: `Doc.ECM.ApiHelper.Static/DocECMApiHelper.cs`.
- Role: centralize calls to the Doc.ECM API (authentication, search, read/save objects, internal tables, comments, imputations, etc.).
- Token management: GetToken() uses the endpoint `${ApiURL}/token` (grant_type=password). The token is stored in `ApiToken` and refreshed when necessary.
- Recommendation: Always call `DocECMApiHelper.SetParameters(...)` at startup to initialize the URL and credentials.

### Jobs and Processes
- `JobRegistry` (FluentScheduler) registers jobs and their intervals.
- Example jobs:
  - `JobExportInvoices` : triggers `TraitmentExportInvoice.ProcessDocumentsToExport()`.
  - `JobSyncTables` : triggers `TraitmentSyncInternalTableData.SyncTables()`.
- Processes: contain the business logic. They load config via `YourCompanyConfigHelper.LoadConfig()` and use `DocECMApiHelper` to interact with Doc.ECM.

Important: `JobRegistry` schedules the next job to wait one minute before starting to avoid requesting multiple Doc.ECM tokens simultaneously.

### MyExternalApiService
- Example integration to an external API.
- Pattern: Initialize URL and credentials, obtain a token via an internal endpoint, then use RestSharp to execute requests.
- Retry policy: Polly is used to detect 401 Unauthorized, renew the token, and retry automatically.
- Central method: `ExecuteApiRequest<T>(string url, Method method, object body = null)`.

### ConfigHelper
- `YourCompanyConfigHelper.LoadConfig()` reads `config.json`. If it is missing, the helper creates a file with default values (edit it and run again).
- `SaveConfig()` serializes the configuration into formatted JSON.

### ServiceLogger
- `ServiceLogger/LogHelper` centralizes logs. Jobs and services use `LogHelper.Log(LogLevel, message)`.

---

## Practical examples

### Add a new job
1. Create `Jobs/MyNewJob.cs`:
```csharp
using FluentScheduler;
using System.Web.Hosting;
using WindowsServiceExample.ServiceLogger;

internal class MyNewJob : IJob, IRegisteredObject
{
    private readonly LogHelper serviceLog = new LogHelper("MyNewJob");
    private readonly object _lock = new object();
    private bool _shuttingDown;

    public MyNewJob() => HostingEnvironment.RegisterObject(this);

    public void Execute()
    {
        lock (_lock)
        {
            if (_shuttingDown) return;
            serviceLog.Log(LogLevel.Info, "MyNewJob started");
            // Your logic...
            serviceLog.Log(LogLevel.Info, "MyNewJob finished");
        }
    }

    public void Stop(bool immediate)
    {
        lock (_lock) { _shuttingDown = true; }
        HostingEnvironment.UnregisterObject(this);
    }
}
```

2. Register it in `JobRegistry` (e.g. every 15 minutes):
```csharp
Schedule<MyNewJob>().ToRunNow().AndEvery(15).Minutes();
```

3. Rebuild and redeploy the service.

Notes:
- Always estimate job execution time to avoid overlaps.
- Use locks or a distributed mechanism if multiple instances may run.

### Example export (flow present in TraitmentExportInvoice)
- Search documents with `DocECMApiHelper.AdvancedSearch(searchPattern)`
- Get imputations with `DocECMApiHelper.GetImputations(objectId, "imputations")`
- Build your business object and call `MyExternalApiService.ExecuteApiRequest<bool>("/invoices", Method.POST, invoice)`
- If successful, change document state in Doc.ECM: `DocECMApiHelper.SaveDocument(...)`
- Add comments on errors: `DocECMApiHelper.SaveComment(...)`

---

## Best practices and constraints
- Doc.ECM token: the code is designed to reuse a token until it expires. Do not request a new token too frequently.
- Quotas: The README notes a limit (e.g. 1440 calls/day) — respect these limits to avoid being blocked.
- Save object endpoint: allowed at higher frequency only for real changes.
- Schedule jobs to avoid overlaps: know the average duration of a job and add a buffer.
- Logs: ensure the execution folder has write permissions to persist logs.
- Security: store sensitive credentials in a vault if possible. At minimum protect `config.json` with filesystem ACLs.

---

## Troubleshooting (common errors)
- Doc.ECM authentication error:
  - Verify `DocECMParameters.ApiUrl`, `Username`, `Password` in `config.json`.
  - Ensure the endpoint `${ApiUrl}/token` is reachable from the machine.
  - Check logs for the API's returned message.
- Jobs not triggering:
  - Ensure the Windows Service is started.
  - Verify `JobScheduleInMinutes` in `config.json`.
  - Check the service logs.
- Network issues to external API:
  - Test the URL with curl/Postman from the same machine.
  - Check for proxy/firewall blocking the request.
- Permissions / service installation:
  - Installing a service requires admin rights.
  - If the service cannot write logs, verify folder permissions.
- Unhandled exceptions:
  - Jobs log exceptions. It is recommended to add more try/catch around critical calls.

---

## Contribution
- Fork the project, create a feature/bugfix branch, test in DEBUG and open a PR.
- Clearly document added business logic.
- Respect existing patterns: centralize API calls via DocECMApiHelper, externalize services, reuse ServiceLogger.

---

If you want, I can:
- Generate a custom example `config.json` with your URLs.
- Write a PowerShell script to install/uninstall the service.
- Explain in detail a specific method of DocECMApiHelper (e.g. SaveDocument, AdvancedSearch) or translate further code comments into English.

---- French details----

# Exemple Windows Service doc.ecm — Visual Studio C#

Un projet exemple qui montre comment construire un Windows Service pour intégrer et automatiser des actions avec l'API Doc.ECM. Ce service illustre des patterns d'intégration avancés : authentification et gestion de token, synchronisation de tables internes, export de documents vers une API externe et gestion d'ordonnancement via FluentScheduler.

Ce README fournit une explication complète du projet, des instructions pour configurer, construire, déployer, exécuter et étendre le service.

---

## Table des matières
- [Objectif du projet](#objectif-du-projet)
- [Cas d'usage typiques](#cas-dusage-typiques)
- [Prérequis](#prérequis)
- [Structure du dépôt](#structure-du-dépôt)
- [Configuration](#configuration)
  - [Exemple config.json](#exemple-configjson)
  - [Champs importants expliqués](#champs-importants-expliqués)
- [Compilation et exécution](#compilation-et-exécution)
  - [Exécution en mode Debug (développement)](#exécution-en-mode-debug-développement)
  - [Construire en Release](#construire-en-release)
  - [Installer le service Windows](#installer-le-service-windows)
  - [Démarrer / Arrêter / Supprimer le service](#démarrer--arrêter--supprimer-le-service)
- [Architecture et composants](#architecture-et-composants)
  - [DocECMApiHelper](#docecmapihelper)
  - [Jobs et Traitements](#jobs-et-traitements)
  - [MyExternalApiService](#myexternalapiservice)
  - [ConfigHelper](#confighelper)
  - [ServiceLogger](#servicelogger)
- [Exemples pratiques](#exemples-pratiques)
  - [Ajouter un nouveau job](#ajouter-un-nouveau-job)
  - [Exemple d'export d'une facture](#exemple-dexport-dune-facture)
- [Bonnes pratiques et limites](#bonnes-pratiques-et-limites)
- [Dépannage](#dépannage)
- [Contribution](#contribution)

---

## Objectif du projet
Ce dépôt montre un modèle de Windows Service capable de :
- Se connecter à Doc.ECM avec gestion de token.
- Planifier des traitements récurrents (FluentScheduler).
- Exécuter des jobs métiers : exporter des documents, synchroniser des tables internes.
- Communiquer avec une API externe sécurisée (authentification + retry avec Polly).
- Gérer proprement l'arrêt et le démarrage (graceful shutdown).

Le code est un squelette destiné à être adapté (ex : connexion à SAP, intégration fournisseur de signature, traitements métiers spécifiques).

---

## Cas d'usage typiques
- Exporter périodiquement des factures depuis Doc.ECM vers un ERP externe.
- Synchroniser des tables internes Doc.ECM avec des données venant d'une API externe.
- Déclencher des processus métier sur des documents (changement d'état, commentaires, etc.).
- Intégration avancée (signature numérique, transformation de documents).

---

## Prérequis
- Visual Studio (ex. 2019/2022) compatible .NET Framework 4.8.1
- .NET Framework Developer Pack 4.8.1 installé
- Accès réseau à l'API Doc.ECM et à l'API externe (si utilisée)
- Droits administrateur pour installer un service Windows

Le fichier `WindowsServiceExample/App.config` indique le runtime cible : .NETFramework,Version=v4.8.1.

---

## Structure du dépôt (essentiel)
- WindowsServiceExample/  
  - Program.cs — entrypoint; en DEBUG exécute directement un traitement pour développement.
  - WindowsService.cs — cycle de vie du service (OnStart, OnStop, InitService).
  - Jobs/ — JobRegistry et classes de jobs (JobExportInvoices, JobSyncTables).
  - Traitments/ — logique métier concrète (TraitmentExportInvoice, TraitmentSyncInternalTableData).
  - Services/MyExternalApiService.cs — wrapper pour une API externe (token, retries).
  - ConfigHelper/ — lecture/écriture de `config.json`.
  - ServiceLogger/ — helper de logging (LogHelper).
- Doc.ECM.ApiHelper.Static/ — helper statique DocECMApiHelper et DTOs (opérations vers Doc.ECM).

---

## Configuration

Le service lit un fichier `config.json` situé dans le répertoire de l'exécutable (créé automatiquement si absent). La classe `YourCompanyConfig` contient notamment :
- `DocECMParameters` (DocECMAPIParametersDTO) — paramètres d'accès à Doc.ECM.
- `ProcessParameters` — ex : JobScheduleInMinutes, LastUpdatedAt.
- `YourAPIConfig` — paramètres pour l'API externe (MyAPIApiUrl, MyAPIUsername, MyAPIPassword).

### Exemple config.json
Copiez-collez ce JSON dans le fichier `config.json` et remplissez les valeurs :

```json
{
  "DocECMParameters": {
    "ApiUrl": "https://votre-docecm.example.com",
    "Username": "votreUserDocECM",
    "Password": "votreMotDePasse",
    "WebSiteUrl": "https://votre-site-web.example.com"
  },
  "ProcessParameters": {
    "JobScheduleInMinutes": 30,
    "LastUpdatedAt": "1940-01-01T00:00:00.0000000+00:00"
  },
  "YourAPIConfig": {
    "MyAPIUsername": "apiUser",
    "MyAPIPassword": "apiPassword",
    "MyAPIApiUrl": "https://api.externe.example.com"
  }
}
```

### Champs importants expliqués
- DocECMParameters.ApiUrl : base URL de l'API Doc.ECM (ex: https://api.docecm.example.com)
- DocECMParameters.Username / Password : identifiants pour obtenir le token via /token
- DocECMParameters.WebSiteUrl : URL du site Doc.ECM (utilisé par la lib si nécessaire)
- ProcessParameters.JobScheduleInMinutes : intervalle (minutes) pour la planification des jobs
- YourAPIConfig.* : URL et identifiants pour votre API externe

---

## Compilation et exécution

### Exécution en mode Debug (développement)
En DEBUG (Program.cs), le service exécute directement un traitement utile pour le développement local :
- Ouvrez Visual Studio en mode Debug.
- Placez un point d'arrêt dans `TraitmentSyncInternalTableData.SyncTables()` ou `TraitmentExportInvoice.ProcessDocumentsToExport()`.
- Lancez l'application (F5). Le code appelle la méthode choisie et ensuite reste bloqué (Thread.Sleep Infinite) pour inspecter.

Note : Ce mode évite d'installer le service pour le développement.

### Construire en Release
- Sélectionnez la configuration Release.
- Platform: x64 (conseillé pour la production selon le README initial).
- Build > Build Solution

### Installer le service Windows
Après build, vous obtiendrez un exécutable dans `bin\Release
et48` (ou `bin\Release` suivant la configuration). Pour installer le service :

- Avec sc.exe (Invite de commandes en administrateur) :
  sc create YourCompanyService binPath= "C:\chemin\vers\WindowsServiceExample.exe" start= auto DisplayName= "YourCompanyService"

- Ou avec PowerShell (exécuté en tant qu'admin) :
  New-Service -Name "YourCompanyService" -BinaryPathName "C:\chemin\vers\WindowsServiceExample.exe" -DisplayName "YourCompanyService" -StartupType Automatic

Après création :
- Démarrer :  net start YourCompanyService
- Arrêter :   net stop YourCompanyService
- Supprimer : sc delete YourCompanyService  (ou Remove-Service en PowerShell)

Remarque : Avant d'installer, vérifiez que `config.json` est présent à côté de l'exécutable et correctement rempli.

---

## Architecture et composants

### DocECMApiHelper
- Fichier principal : `Doc.ECM.ApiHelper.Static/DocECMApiHelper.cs`.
- Rôle : centraliser les appels à l'API Doc.ECM (authentification, recherches, lecture/sauvegarde d'objets, tables internes, commentaires, imputations, etc.).
- Token management : GetToken() utilise l'endpoint `${ApiURL}/token` (grant_type=password). Le token est stocké dans `ApiToken` et recalculé si nécessaire.
- Recommandation : Toujours appeler `DocECMApiHelper.SetParameters(...)` au démarrage pour initialiser l'URL et identifiants.

### Jobs et Traitements
- `JobRegistry` (FluentScheduler) inscrit les jobs et leur intervalle.
- Jobs Exemple :
  - `JobExportInvoices` : déclenche `TraitmentExportInvoice.ProcessDocumentsToExport()`.
  - `JobSyncTables` : déclenche `TraitmentSyncInternalTableData.SyncTables()`.
- Traitements : contiennent la logique métier. Ils lisent la config via `YourCompanyConfigHelper.LoadConfig()` et utilisent `DocECMApiHelper` pour interactions Doc.ECM.

Important : `JobRegistry` attend une minute entre deux jobs différents pour éviter de réclamer plusieurs token Doc.ECM simultanément.

### MyExternalApiService
- Exemple d'intégration vers une API externe.
- Pattern : Initialise l'URL et identifiants, récupère un token via un endpoint interne, puis utilise RestSharp pour exécuter des requêtes.
- Retry policy : Polly est utilisé pour reconnaître 401 Unauthorized et renouveler le token, puis réessayer automatiquement.
- Méthode centrale : `ExecuteApiRequest<T>(string url, Method method, object body = null)`.

### ConfigHelper
- `YourCompanyConfigHelper.LoadConfig()` lit `config.json`. Si absent, il crée un fichier avec valeurs par défaut (videz et remplissez).
- `SaveConfig()` sérialise la configuration en JSON formaté.

### ServiceLogger
- `ServiceLogger/LogHelper` centralise les logs. Les jobs et services utilisent `LogHelper.Log(LogLevel, message)`.

---

## Exemples pratiques

### Ajouter un nouveau job
1. Créer un fichier `Jobs/MyNewJob.cs` :
```csharp
using FluentScheduler;
using System.Web.Hosting;
using WindowsServiceExample.ServiceLogger;

internal class MyNewJob : IJob, IRegisteredObject
{
    private readonly LogHelper serviceLog = new LogHelper("MyNewJob");
    private readonly object _lock = new object();
    private bool _shuttingDown;

    public MyNewJob() => HostingEnvironment.RegisterObject(this);

    public void Execute()
    {
        lock (_lock)
        {
            if (_shuttingDown) return;
            serviceLog.Log(LogLevel.Info, "MyNewJob started");
            // Votre logique...
            serviceLog.Log(LogLevel.Info, "MyNewJob finished");
        }
    }

    public void Stop(bool immediate)
    {
        lock (_lock) { _shuttingDown = true; }
        HostingEnvironment.UnregisterObject(this);
    }
}
```

2. Enregistrer dans `JobRegistry` (ex: toutes les 15 minutes) :
```csharp
Schedule<MyNewJob>().ToRunNow().AndEvery(15).Minutes();
```

3. Rebuild et redéployez le service.

Remarques :
- Toujours estimer le temps d'exécution d'un job pour éviter chevauchements.
- Utilisez des locks ou un mécanisme distribué si plusieurs instances peuvent s'exécuter.

### Exemple d'export (flux présent dans TraitmentExportInvoice)
- Rechercher documents avec `DocECMApiHelper.AdvancedSearch(searchPattern)`
- Récupérer imputations `DocECMApiHelper.GetImputations(objectId, "imputations")`
- Construire l'objet métier puis appeler `MyExternalApiService.ExecuteApiRequest<bool>("/invoices", Method.POST, invoice)`
- Si succès, changer l'état dans Doc.ECM : `DocECMApiHelper.SaveDocument(...)`
- Ajouter commentaires en cas d'erreur : `DocECMApiHelper.SaveComment(...)`

---

## Bonnes pratiques et limites
- Token Doc.ECM : le code est conçu pour réutiliser un token jusqu'à expiration. Ne demandez pas un nouveau token trop fréquemment.
- Quotas : Le README initial indique une limite (ex : 1440 appels/jour) — respectez ces limites pour éviter blocage.
- Objet Save (sauvegarde d'objet) : autorisé en fréquence élevée uniquement pour de vrais changements.
- Planifiez les jobs pour éviter chevauchement : connaître la durée moyenne d'un job et ajouter un buffer.
- Logs : assurez-vous que le répertoire d'exécution possède les droits d'écriture pour persister les logs.
- Sécurité : stockez les mots de passe/config sensibles dans un vault si possible. `config.json` vaut mieux la protection (ACL) sur le dossier.

---

## Dépannage (erreurs communes)
- Erreur d'authentification Doc.ECM :
  - Vérifiez `DocECMParameters.ApiUrl`, `Username`, `Password` dans `config.json`.
  - Assurez-vous que l'endpoint `${ApiUrl}/token` est accessible depuis la machine.
  - Vérifiez les logs pour le message renvoyé par l'API.
- Jobs qui ne se déclenchent pas :
  - Vérifiez que le service Windows est démarré.
  - Vérifiez `JobScheduleInMinutes` dans `config.json`.
  - Consultez les logs du service.
- Problèmes réseau vers API externe :
  - Testez l'URL via curl/Postman depuis la même machine.
  - Vérifiez si un proxy / firewall bloque l'accès.
- Permissions / installation service :
  - Installer/enregistrer un service requiert des droits administrateur.
  - Si le service ne peut pas écrire les logs, assurez-vous des permissions sur le dossier.
- Exceptions non gérées :
  - Les jobs loggent les exceptions. Il est recommandé d'ajouter davantage de try/catch autour des appels critiques.

---

## Contribution
- Forkez le projet, créez une branche feature/bugfix, testez en DEBUG et ouvrez une PR.
- Documentez clairement la logique métier ajoutée.
- Respectez les patterns existants : centralisez les appels API via DocECMApiHelper, externalisez les services, réutilisez ServiceLogger.

---

Si vous souhaitez, je peux :
- Générer un example `config.json` personnalisé avec vos URLs.
- Rédiger un script PowerShell d'installation/désinstallation du service.
- Expliquer en détail une méthode particulière du DocECMApiHelper (ex: SaveDocument, AdvancedSearch) ou traduire davantage de commentaires en français.

---- Spanish details----

# Doc.ECM Windows Service Example — Visual Studio C#

Un proyecto de ejemplo que muestra cómo construir un Servicio de Windows para integrar y automatizar acciones con la API de Doc.ECM. Este servicio demuestra patrones de integración avanzados: autenticación y gestión de tokens, sincronización de tablas internas, exportación de documentos a una API externa y programación mediante FluentScheduler.

Este README proporciona una explicación completa del proyecto, además de instrucciones para configurar, compilar, desplegar, ejecutar y ampliar el servicio.

---

## Tabla de contenidos
- [Características](#caracter%C3%ADsticas)
- [Objetivo del proyecto](#objetivo-del-proyecto)
- [Casos de uso típicos](#casos-de-uso-t%C3%ADpicos)
- [Requisitos previos](#requisitos-previos)
- [Estructura del repositorio](#estructura-del-repositorio)
- [Configuración](#configuraci%C3%B3n)
  - [Ejemplo config.json](#ejemplo-configjson)
  - [Campos importantes explicados](#campos-importantes-explicados)
- [Instalación](#instalaci%C3%B3n)
  - [Compilar en Release](#compilar-en-release)
  - [Instalar el Servicio de Windows](#instalar-el-servicio-de-windows)
  - [Iniciar / Detener / Eliminar el servicio](#iniciar--detener--eliminar-el-servicio)
- [Uso](#uso)
  - [Ejecutar en modo Debug (desarrollo)](#ejecutar-en-modo-debug-desarrollo)
- [Arquitectura y componentes](#arquitectura-y-componentes)
  - [DocECMApiHelper](#docecmapihelper)
  - [Jobs y Procesos](#jobs-y-procesos)
  - [MyExternalApiService](#myexternalapiservice)
  - [ConfigHelper](#confighelper)
  - [ServiceLogger](#servicelogger)
- [Ejemplos prácticos](#ejemplos-pr%C3%A1cticos)
  - [Agregar un nuevo job](#agregar-un-nuevo-job)
  - [Ejemplo de exportación de factura](#ejemplo-de-exportaci%C3%B3n-de-factura)
- [Buenas prácticas y limitaciones](#buenas-pr%C3%A1cticas-y-limitaciones)
- [Solución de problemas](#soluci%C3%B3n-de-problemas)
- [Contribución](#contribuci%C3%B3n)

---

## Características
- Autenticación basada en token y reutilización/actualización de token para Doc.ECM.
- Procesamiento recurrente programado usando FluentScheduler.
- Ejemplos de jobs de negocio: exportación de documentos (facturas) y sincronización de tablas internas.
- Integración con una API externa (autenticación por token + reintentos con Polly).
- Arranque/apagado graceful y soporte de logging.
- Helpers de ejemplo para configuración y acceso a la API de Doc.ECM.

---

## Objetivo del proyecto
Este repositorio muestra un patrón de Servicio de Windows capaz de:
- Conectarse a Doc.ECM con gestión de tokens.
- Programar procesamiento recurrente (FluentScheduler).
- Ejecutar jobs de negocio: exportar documentos, sincronizar tablas internas.
- Comunicar con una API externa segura (autenticación + reintentos con Polly).
- Manejar el arranque y apagado de forma controlada.

El código es un esqueleto pensado para ser adaptado (por ejemplo, conexión a SAP, integración de proveedor de firma, procesos de negocio específicos).

---

## Casos de uso típicos
- Exportar periódicamente facturas desde Doc.ECM a un ERP externo.
- Sincronizar tablas internas de Doc.ECM con datos provenientes de una API externa.
- Disparar workflows de negocio sobre documentos (cambios de estado, comentarios, etc.).
- Integraciones avanzadas (firma digital, transformaciones de documentos).

---

## Requisitos previos
- Visual Studio (por ejemplo 2019/2022) compatible con .NET Framework 4.8.1
- .NET Framework Developer Pack 4.8.1 instalado
- Acceso de red a la API de Doc.ECM y a la API externa (si se usa)
- Permisos de administrador para instalar un Servicio de Windows

El fichero `WindowsServiceExample/App.config` indica el runtime objetivo: .NETFramework,Version=v4.8.1.

---

## Estructura del repositorio (esencial)
- WindowsServiceExample/  
  - Program.cs — punto de entrada; en DEBUG ejecuta un método de procesamiento para desarrollo.
  - WindowsService.cs — ciclo de vida del servicio (OnStart, OnStop, InitService).
  - Jobs/ — JobRegistry y clases de jobs (JobExportInvoices, JobSyncTables).
  - Traitments/ — lógica de negocio concreta (TraitmentExportInvoice, TraitmentSyncInternalTableData).
  - Services/MyExternalApiService.cs — wrapper hacia una API externa (token, reintentos).
  - ConfigHelper/ — lectura/escritura de `config.json`.
  - ServiceLogger/ — helper de logging (LogHelper).
- Doc.ECM.ApiHelper.Static/ — helper estático DocECMApiHelper y DTOs (operaciones hacia Doc.ECM).

---

## Configuración

El servicio lee un archivo `config.json` ubicado en el directorio del ejecutable (se crea automáticamente si falta). La clase `YourCompanyConfig` contiene, entre otros:
- `DocECMParameters` (DocECMAPIParametersDTO) — parámetros de acceso a Doc.ECM.
- `ProcessParameters` — p. ej. JobScheduleInMinutes, LastUpdatedAt.
- `YourAPIConfig` — parámetros para la API externa (MyAPIApiUrl, MyAPIUsername, MyAPIPassword).

### Ejemplo config.json
Copia y pega este JSON en el archivo `config.json` y completa los valores:

```json
{
  "DocECMParameters": {
    "ApiUrl": "https://your-docecm.example.com",
    "Username": "yourDocECMUser",
    "Password": "yourPassword",
    "WebSiteUrl": "https://your-website.example.com"
  },
  "ProcessParameters": {
    "JobScheduleInMinutes": 30,
    "LastUpdatedAt": "1940-01-01T00:00:00.0000000+00:00"
  },
  "YourAPIConfig": {
    "MyAPIUsername": "apiUser",
    "MyAPIPassword": "apiPassword",
    "MyAPIApiUrl": "https://external.api.example.com"
  }
}
```

### Campos importantes explicados
- DocECMParameters.ApiUrl : URL base de la API de Doc.ECM (p. ej. https://api.docecm.example.com)
- DocECMParameters.Username / Password : credenciales para obtener el token vía /token
- DocECMParameters.WebSiteUrl : URL del sitio Doc.ECM (usado por la librería si es necesario)
- ProcessParameters.JobScheduleInMinutes : intervalo (minutos) para la programación de jobs
- YourAPIConfig.* : URL y credenciales para tu API externa

---

## Instalación

### Compilar en Release
- Selecciona la configuración Release en Visual Studio.
- Plataforma: x64 (recomendado para producción según el README original).
- Build > Build Solution

Tras compilar, obtendrás un ejecutable en `bin\Release
et48` (o `bin\Release` dependiendo de la configuración).

### Instalar el Servicio de Windows
Después de compilar, para instalar el servicio:

- Con sc.exe (Command Prompt como administrador):
```cmd
sc create YourCompanyService binPath= "C:\path\to\WindowsServiceExample.exe" start= auto DisplayName= "YourCompanyService"
```

- O con PowerShell (ejecutar como admin):
```powershell
New-Service -Name "YourCompanyService" -BinaryPathName "C:\path\to\WindowsServiceExample.exe" -DisplayName "YourCompanyService" -StartupType Automatic
```

Nota: Antes de instalar, asegúrate de que `config.json` esté presente junto al ejecutable y correctamente configurado.

### Iniciar / Detener / Eliminar el servicio
- Iniciar:  
```cmd
net start YourCompanyService
```
- Detener:  
```cmd
net stop YourCompanyService
```
- Eliminar:  
```cmd
sc delete YourCompanyService
```
(o `Remove-Service` en PowerShell)

---

## Uso

### Ejecutar en modo Debug (desarrollo)
En DEBUG (Program.cs), el servicio ejecuta directamente un método de procesamiento para desarrollo local:
- Abre Visual Studio en modo Debug.
- Coloca un breakpoint en `TraitmentSyncInternalTableData.SyncTables()` o `TraitmentExportInvoice.ProcessDocumentsToExport()`.
- Ejecuta la aplicación (F5). El código llama al método elegido y luego se queda bloqueado (Thread.Sleep Infinite) para inspección.

Nota: Este modo evita instalar el servicio para desarrollo.

---

## Arquitectura y componentes

### DocECMApiHelper
- Archivo principal: `Doc.ECM.ApiHelper.Static/DocECMApiHelper.cs`.
- Rol: centralizar llamdas a la API de Doc.ECM (autenticación, búsqueda, leer/guardar objetos, tablas internas, comentarios, imputaciones, etc.).
- Gestión de token: GetToken() usa el endpoint `${ApiURL}/token` (grant_type=password). El token se almacena en `ApiToken` y se refresca cuando es necesario.
- Recomendación: Siempre llamar a `DocECMApiHelper.SetParameters(...)` al inicio para inicializar la URL y credenciales.

### Jobs y Procesos
- `JobRegistry` (FluentScheduler) registra jobs y sus intervalos.
- Jobs de ejemplo:
  - `JobExportInvoices` : dispara `TraitmentExportInvoice.ProcessDocumentsToExport()`.
  - `JobSyncTables` : dispara `TraitmentSyncInternalTableData.SyncTables()`.
- Procesos: contienen la lógica de negocio. Cargan la configuración mediante `YourCompanyConfigHelper.LoadConfig()` y usan `DocECMApiHelper` para interactuar con Doc.ECM.

Importante: `JobRegistry` programa el siguiente job esperando un minuto antes de iniciar para evitar solicitar múltiples tokens Doc.ECM simultáneamente.

### MyExternalApiService
- Ejemplo de integración con una API externa.
- Patrón: Inicializa URL y credenciales, obtiene un token vía un endpoint interno, y usa RestSharp para ejecutar peticiones.
- Política de reintento: Polly se usa para detectar 401 Unauthorized, renovar el token y reintentar automáticamente.
- Método central: `ExecuteApiRequest<T>(string url, Method method, object body = null)`.

### ConfigHelper
- `YourCompanyConfigHelper.LoadConfig()` lee `config.json`. Si falta, el helper crea un archivo con valores por defecto (edítalo y ejecuta de nuevo).
- `SaveConfig()` serializa la configuración a JSON formateado.

### ServiceLogger
- `ServiceLogger/LogHelper` centraliza los logs. Jobs y servicios usan `LogHelper.Log(LogLevel, message)`.

---

## Ejemplos prácticos

### Agregar un nuevo job
1. Crea `Jobs/MyNewJob.cs`:
```csharp
using FluentScheduler;
using System.Web.Hosting;
using WindowsServiceExample.ServiceLogger;

internal class MyNewJob : IJob, IRegisteredObject
{
    private readonly LogHelper serviceLog = new LogHelper("MyNewJob");
    private readonly object _lock = new object();
    private bool _shuttingDown;

    public MyNewJob() => HostingEnvironment.RegisterObject(this);

    public void Execute()
    {
        lock (_lock)
        {
            if (_shuttingDown) return;
            serviceLog.Log(LogLevel.Info, "MyNewJob started");
            // Your logic...
            serviceLog.Log(LogLevel.Info, "MyNewJob finished");
        }
    }

    public void Stop(bool immediate)
    {
        lock (_lock) { _shuttingDown = true; }
        HostingEnvironment.UnregisterObject(this);
    }
}
```

2. Regístralo en `JobRegistry` (por ejemplo cada 15 minutos):
```csharp
Schedule<MyNewJob>().ToRunNow().AndEvery(15).Minutes();
```

3. Reconstruye y redepliega el servicio.

Notas:
- Siempre estima el tiempo de ejecución del job para evitar solapamientos.
- Usa locks o un mecanismo distribuido si pueden ejecutarse múltiples instancias.

### Ejemplo de exportación (flujo presente en TraitmentExportInvoice)
- Buscar documentos con `DocECMApiHelper.AdvancedSearch(searchPattern)`
- Obtener imputaciones con `DocECMApiHelper.GetImputations(objectId, "imputations")`
- Construir tu objeto de negocio y llamar `MyExternalApiService.ExecuteApiRequest<bool>("/invoices", Method.POST, invoice)`
- Si es exitoso, cambiar el estado del documento en Doc.ECM: `DocECMApiHelper.SaveDocument(...)`
- Añadir comentarios en errores: `DocECMApiHelper.SaveComment(...)`

---

## Buenas prácticas y limitaciones
- Token Doc.ECM: el código está diseñado para reutilizar un token hasta que expire. No solicites un nuevo token con demasiada frecuencia.
- Cuotas: el README menciona un límite (p. ej. 1440 llamadas/día) — respeta estos límites para evitar bloqueos.
- Endpoint Save object: permitido con mayor frecuencia solo para cambios reales.
- Programa jobs para evitar solapamientos: conoce la duración media de un job y añade un margen.
- Logs: asegura que la carpeta de ejecución tenga permisos de escritura para persistir logs.
- Seguridad: guarda credenciales sensibles en un vault si es posible. Como mínimo protege `config.json` con ACLs del sistema de archivos.

---

## Solución de problemas (errores comunes)
- Error de autenticación Doc.ECM:
  - Verifica `DocECMParameters.ApiUrl`, `Username`, `Password` en `config.json`.
  - Asegúrate de que el endpoint `${ApiUrl}/token` sea accesible desde la máquina.
  - Revisa los logs para el mensaje devuelto por la API.
- Jobs no se disparan:
  - Asegúrate de que el Servicio de Windows esté iniciado.
  - Verifica `JobScheduleInMinutes` en `config.json`.
  - Comprueba los logs del servicio.
- Problemas de red hacia la API externa:
  - Prueba la URL con curl/Postman desde la misma máquina.
  - Verifica proxy/firewall que pueda bloquear la petición.
- Permisos / instalación del servicio:
  - Instalar un servicio requiere permisos de administrador.
  - Si el servicio no puede escribir logs, verifica permisos de carpeta.
- Excepciones no gestionadas:
  - Los jobs registran excepciones. Se recomienda añadir más try/catch alrededor de llamadas críticas.

---

## Contribución
- Haz fork del proyecto, crea una rama feature/bugfix, prueba en DEBUG y abre un PR.
- Documenta claramente la lógica de negocio añadida.
- Respeta los patrones existentes: centraliza las llamadas a la API vía DocECMApiHelper, externaliza servicios, reutiliza ServiceLogger.

---

Si quieres, puedo:
- Generar un ejemplo personalizado de `config.json` con tus URLs.
- Escribir un script PowerShell para instalar/desinstalar el servicio.
- Explicar en detalle un método específico de DocECMApiHelper (por ejemplo SaveDocument, AdvancedSearch) o traducir comentarios adicionales del código al español.

