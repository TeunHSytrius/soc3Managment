# SOC3 — SOC-casemanagementplatform

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![React en TypeScript](https://img.shields.io/badge/Frontend-React%20%2B%20TypeScript-149ECA?logo=react&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927)
![Status](https://img.shields.io/badge/Status-Scaffolding%20gereed-orange)

**De Haagse Hogeschool · HBO-ICT Software Engineering · Semester 3 · Projectgroep 3**

Wij ontwikkelen een casemanagementplatform voor een Security Operations Centre (SOC). Het doel is om beveiligingsalerts te ontvangen en te beoordelen, onderzoeken als cases vast te leggen en klantgegevens strikt van elkaar te scheiden.

[GitHub-repository](https://github.com/TeunHSytrius/soc3Managment) · [Taiga-projectbord](https://tree.taiga.io/project/djominique-soc-groep-3/kanban)

> **Huidige stand:** De .NET 10-solution en React/TypeScript-frontend zijn aangemaakt en kunnen afzonderlijk bouwen en starten. De API bevat nog geen SOC-endpoints en de frontend is nog niet gekoppeld aan de API. Database-inrichting, authenticatie, alerts, cases en CI zijn nog niet geïmplementeerd.

---

## Inhoudsopgave

1. [Snelstart](#1-snelstart)
2. [Projectstructuur](#2-projectstructuur)
3. [Samenwerken via GitHub en Taiga](#3-samenwerken-via-github-en-taiga)
4. [Codeafspraken](#4-codeafspraken)
5. [Database en beveiliging](#5-database-en-beveiliging)
6. [Controleren en testen](#6-controleren-en-testen)
7. [Problemen oplossen](#7-problemen-oplossen)
8. [Documentatie en vervolg](#8-documentatie-en-vervolg)

---

## 1. Snelstart

### 1.1 Benodigde software

| Software       | Wat we gebruiken                                                 |
| -------------- | ---------------------------------------------------------------- |
| Git            | Repository clonen en branches beheren                            |
| .NET SDK       | 10.0.401, volgens `global.json`                                  |
| Node.js en npm | Een Node.js-versie die geschikt is voor de gebruikte Vite-versie |
| IDE            | VS Code, Rider of Visual Studio                                  |
| SQL Server     | Nodig zodra databasefunctionaliteit wordt aangesloten            |

Controleer je installatie:

```bash
git --version
dotnet --version
node --version
npm --version
```

`global.json` legt .NET SDK 10.0.401 vast. Nieuwere patches binnen dezelfde SDK-featureband zijn toegestaan.

### 1.2 Repository ophalen

```bash
git clone https://github.com/TeunHSytrius/soc3Managment.git
cd soc3Managment
git switch develop
git pull --ff-only origin develop
```

`develop` is de gezamenlijke ontwikkelbranch. `main` bevat de stabiele, goedgekeurde versie.

### 1.3 Backend starten

Open een terminal in de hoofdmap:

```bash
dotnet restore SOC3.slnx
dotnet build SOC3.slnx
dotnet run --project backend/Soc.Api --launch-profile http
```

De API gebruikt het ontwikkeladres:

`http://localhost:5042`

In de ontwikkelomgeving is het OpenAPI-document beschikbaar via:

`http://localhost:5042/openapi/v1.json`

De API heeft nog geen eigen SOC-controllers of werkende alert- en caseroutes.

**De huidige backend heeft nog geen SQL Server-verbinding nodig.** Die wordt toegevoegd wanneer de databasefunctionaliteit wordt ontwikkeld.

### 1.4 Frontend starten

Open een tweede terminal vanuit de hoofdmap:

```bash
cd frontend
npm ci
npm run dev
```

Open het adres dat Vite in de terminal toont, doorgaans:

`http://localhost:5173`

Je ziet nu de eenvoudige SOC-startpagina. Het alert- en caseoverzicht worden later gebouwd.

`npm ci` installeert de dependencies uit `package-lock.json`.

### 1.5 Dagelijks werken

Haal eerst de nieuwste wijzigingen op:

```bash
git switch develop
git pull --ff-only origin develop
```

Maak vervolgens een werkbranch voor je toegewezen Taiga-ticket.

Je kunt ontwikkelservers stoppen met `Ctrl + C`.

---

## 2. Projectstructuur

```text
soc3Managment/
├── .github/
│   └── PULL_REQUEST_TEMPLATE.md
├── backend/
│   ├── Soc.Api/
│   │   └── Controllers/
│   ├── Soc.Business/
│   │   ├── DTOs/
│   │   └── Services/
│   ├── Soc.Data/
│   │   ├── Entities/
│   │   └── Repositories/
│   └── Soc.Tests/
├── frontend/
│   ├── public/
│   └── src/
│       ├── components/
│       ├── features/
│       │   ├── alerts/
│       │   ├── auth/
│       │   └── cases/
│       ├── hooks/
│       ├── services/
│       └── types/
├── database/
│   └── migrations/
├── docs/
├── .editorconfig
├── .gitignore
├── global.json
├── SOC3.slnx
└── README.md
```

Lege mappen zijn met `.gitkeep` opgenomen in Git. Voeg code toe wanneer je aan de bijbehorende functionaliteit werkt.

### 2.1 Verantwoordelijkheden

| Project        | Verantwoordelijkheid                                  |
| -------------- | ----------------------------------------------------- |
| `Soc.Api`      | Controllers, HTTP-verzoeken en applicatieconfiguratie |
| `Soc.Business` | Bedrijfsregels, services en request-/responsemodellen |
| `Soc.Data`     | Datatoegang, databasecontexten en schemaresolutie     |
| `Soc.Tests`    | Unit- en integratietests                              |
| `frontend`     | React-componenten, schermen en API-aanroepen          |
| `database`     | SQL-scripts voor database- en klantschema-updates     |
| `docs`         | Ontwerpen en technische beslissingen                  |

### 2.2 Architectuur

```text
React + TypeScript
        |
        | HTTPS / JSON
        v
ASP.NET Core Web API
        |
        v
Services en bedrijfsregels
        |
        v
Datalaag (EF Core / SQL)
        |
        v
SQL Server
```

De frontend communiceert uitsluitend met de backend.

De backend controleert de toegangsrechten en bepaalt welke klantgegevens mogen worden opgehaald of aangepast.

De projectreferenties zijn al aangemaakt. De databaseverbinding, bedrijfsregels en beveiligingsfunctionaliteit worden tijdens de implementatie toegevoegd.

---

## 3. Samenwerken via GitHub en Taiga

We gebruiken:

- **Taiga:** User stories, taken, bugs en sprintplanning.
- **GitHub:** Code, branches, commits, Pull Requests en reviews.

### 3.1 Branchstrategie

| Branch      | Gebruik                         |
| ----------- | ------------------------------- |
| `main`      | Stabiele en goedgekeurde versie |
| `develop`   | Gezamenlijke ontwikkelbranch    |
| `feature/*` | Nieuwe functionaliteit          |
| `bugfix/*`  | Bugfixes                        |
| `chore/*`   | Technisch onderhoud             |
| `docs/*`    | Documentatie                    |

```text
feature/* ─────┐
bugfix/* ──────┤
chore/* ───────┼──> develop ──> main
docs/* ────────┘
```

We werken niet rechtstreeks op `main` of `develop`. Beide branches worden beschermd door GitHub-rulesets.

### 3.2 Branchnaamgeving

Iedere werkbranch bevat het Taiga-ticketnummer.

Formaat:

```text
<type>/tg-<ticketnummer>-<omschrijving>
```

Voorbeelden:

```text
feature/tg-59-alert-access
bugfix/tg-60-alert-filter
chore/tg-61-database-setup
docs/tg-62-update-documentation
```

Gebruik altijd het echte Taiga-ticketnummer en een korte Engelse omschrijving.

### 3.3 Nieuwe werkbranch aanmaken

```bash
git switch develop
git pull --ff-only origin develop
git switch -c feature/tg-59-alert-access
```

Gebruik de branchnaam die bij jouw eigen ticket hoort.

### 3.4 Commitconventies

We gebruiken Conventional Commits.

Formaat:

```text
<type>: <beschrijving> (TG-<nummer>)
```

| Type       | Gebruik                      |
| ---------- | ---------------------------- |
| `feat`     | Nieuwe functionaliteit       |
| `fix`      | Fout oplossen                |
| `refactor` | Bestaande code verbeteren    |
| `test`     | Tests toevoegen of aanpassen |
| `docs`     | Documentatie                 |
| `chore`    | Onderhoud en configuratie    |
| `ci`       | CI/CD-configuratie           |

Voorbeelden:

```text
feat: add alert access checks (TG-59)
fix: correct alert status filtering (TG-60)
test: add schema isolation tests (TG-62)
```

Commitberichten zijn in het Engels en beschrijven wat er daadwerkelijk is aangepast.

### 3.5 Wijzigingen pushen

```bash
git status
git add -A
git commit -m "feat: add alert access checks (TG-59)"
git push -u origin feature/tg-59-alert-access
```

Maak daarna op GitHub een Pull Request naar `develop`.

### 3.6 Pull Requests

Iedere Pull Request gebruikt de template:

`.github/PULL_REQUEST_TEMPLATE.md`

Vul minimaal in:

- Het Taiga-ticketnummer en de link.
- Wat er is aangepast.
- Hoe de wijziging is getest.
- Eventuele aandachtspunten voor de reviewer.

### 3.7 Branchbeveiliging

Voor `main` en `develop` zijn actieve GitHub-rulesets ingesteld.

De belangrijkste regels zijn:

- Iedere wijziging gaat via een Pull Request.
- Minimaal één reviewer moet goedkeuren.
- Goedkeuringen vervallen wanneer nieuwe wijzigingen worden gepusht.
- Reviewgesprekken moeten zijn afgehandeld.
- Force-pushes zijn niet toegestaan.
- Beschermde branches mogen niet worden verwijderd.

**Mergebeleid**

| Van        | Naar      | Methode               |
| ---------- | --------- | --------------------- |
| Werkbranch | `develop` | Squash and merge      |
| `develop`  | `main`    | Create a merge commit |

CI-statuschecks zijn nog niet verplicht. Deze worden toegevoegd wanneer de bijbehorende GitHub Actions-workflows werken.

### 3.8 Eigen branch bijwerken

```bash
git fetch origin
git rebase origin/develop
```

Gebruik rebase op je eigen werkbranch. Herschrijf niet de geschiedenis van gedeelde branches.

---

## 4. Codeafspraken

We volgen de naamgevingsafspraken uit het technisch ontwerp v1.3.

**Code wordt in het Engels geschreven. Schermteksten mogen Nederlands zijn.**

### 4.1 C# en ASP.NET Core

| Onderdeel        | Conventie      | Voorbeeld           |
| ---------------- | -------------- | ------------------- |
| Class            | PascalCase     | `AlertService`      |
| Interface        | I + PascalCase | `IAlertService`     |
| Method           | PascalCase     | `ClaimAlertAsync()` |
| Property         | PascalCase     | `SchemaName`        |
| Enum             | PascalCase     | `AlertStatus`       |
| Enumwaarde       | PascalCase     | `InProgress`        |
| Parameter        | camelCase      | `alertId`           |
| Lokale variabele | camelCase      | `currentUser`       |
| Async-methode    | Async-suffix   | `GetAlertAsync()`   |

Algemene afspraken:

- Gebruik duidelijke, beschrijvende namen.
- Gebruik dependency injection.
- Gebruik `async` en `await` voor asynchrone I/O.
- Plaats bedrijfsregels in `Soc.Business`.
- Plaats databasecode in `Soc.Data`.
- Houd controllers beperkt tot HTTP-verwerking.
- Voorkom dubbele code en onnodige complexiteit.

### 4.2 React en TypeScript

| Onderdeel        | Conventie      | Voorbeeld       |
| ---------------- | -------------- | --------------- |
| Component        | PascalCase     | `AlertList`     |
| Componentbestand | PascalCase.tsx | `AlertList.tsx` |
| Functie          | camelCase      | `fetchAlerts()` |
| Variabele        | camelCase      | `selectedAlert` |
| Type/interface   | PascalCase     | `AlertResponse` |
| JSON-property    | camelCase      | `receivedAt`    |

Algemene afspraken:

- Gebruik functionele React-componenten.
- Geef componenten één duidelijke verantwoordelijkheid.
- Gebruik TypeScript voor API-gegevens en componentproperties.
- Scheid API-communicatie zoveel mogelijk van presentatiecode.
- Plaats geen servergeheimen in frontendcode.

### 4.3 Database

| Onderdeel            | Voorbeeld            |
| -------------------- | -------------------- |
| Entity               | `InvestigationCase`  |
| SQL-tabel            | `InvestigationCases` |
| Primaire sleutel     | `Id`                 |
| Vreemde sleutel      | `CaseId`             |
| Werkruimte           | `ScopeId`            |
| Schema-identificatie | `SchemaName`         |
| Versiecontrole       | `RowVersion`         |

Volgens het technisch ontwerp geldt:

- Centrale gebruikers- en klantgegevens staan in het platform-schema.
- Onderzoeksgegevens staan in afzonderlijke klantschema's.
- Onderzoekstabellen gebruiken geen `TenantId` als scheidingskolom.
- `ScopeId` wordt gebruikt voor verdere afscherming binnen een klant.
- De backend bepaalt de SQL-schemanaam via gecontroleerde platformgegevens.
- SQL-rechten worden zo beperkt mogelijk ingesteld.

### 4.4 API

Alle API-routes beginnen met:

```text
/api/v1
```

Geplande routes uit het technisch ontwerp zijn onder andere:

```text
POST /api/v1/ingest/alerts
GET  /api/v1/alerts
GET  /api/v1/alerts/{id}
POST /api/v1/alerts/{id}/claim
POST /api/v1/alerts/{id}/triage

GET  /api/v1/cases
GET  /api/v1/cases/{id}
```

Deze routes zijn nog niet geïmplementeerd.

De frontend stuurt waar nodig `X-Workspace-Id` en `X-Scope-Id` mee.

De backend controleert de toegang en bepaalt zelf het juiste klantschema. De frontend mag nooit rechtstreeks een SQL-schemanaam kiezen.

### 4.5 Codeopmaak

| Onderdeel          | Controle                           |
| ------------------ | ---------------------------------- |
| C#                 | `.editorconfig` en `dotnet format` |
| Backend            | `dotnet build` en `dotnet test`    |
| React / TypeScript | ESLint en TypeScript               |
| Frontendbuild      | Vite                               |

`global.json` legt de .NET SDK-versie vast.

`.editorconfig` bevat afspraken voor codeopmaak en bepaalde C#-conventies.

---

## 5. Database en beveiliging

**De onderstaande onderdelen staan in het technisch ontwerp, maar moeten nog worden geïmplementeerd.**

### 5.1 Klantscheiding

We gebruiken één SQL Server-database met afzonderlijke schema's per klant.

```text
SOC Database
│
├── platform
│   ├── Tenants
│   ├── Scopes
│   ├── AppUsers
│   └── RoleAssignments
│
├── klant_a
│   ├── Alerts
│   ├── InvestigationCases
│   └── CaseEvents
│
└── klant_b
    ├── Alerts
    ├── InvestigationCases
    └── CaseEvents
```

Een gebruiker mag uitsluitend gegevens benaderen waarvoor hij of zij toegang heeft.

De backend controleert per verzoek:

- De rol van de gebruiker.
- De toegestane klant.
- De toegestane scope.
- De gevraagde handeling.

### 5.2 Database-inrichting

Voor de database gelden de volgende afspraken:

- Platformgegevens gebruiken EF Core-migrations.
- Klantschema's krijgen gecontroleerde SQL-deploymentscripts.
- Schemawijzigingen worden vastgelegd in versiebeheer.
- Klantschema's worden pas actief nadat tabellen en rechten zijn gecontroleerd.
- De API krijgt geen onnodige SQL-beheerrechten.

De schema-per-klant-aanpak met EF Core moet eerst worden gevalideerd.

Daarvoor gebruiken we minimaal twee fictieve klanten en testen we onder andere schemaresolutie, modelcaching en toegangsrechten.

### 5.3 Beveiligingsafspraken

- Autorisatie vindt altijd plaats in de backend.
- Wachtwoorden en API-sleutels worden niet naar Git gepusht.
- De frontend bevat geen servergeheimen.
- SQL-rechten worden beperkt volgens het least-privilege-principe.
- Klantgegevens mogen niet tussen schema's uitlekken.
- Gevoelige handelingen worden geaudit zodra auditing is geïmplementeerd.

De huidige `appsettings.json` bevat alleen basisinstellingen voor logging en `AllowedHosts`.

De database- en authenticatieconfiguratie volgen tijdens de ontwikkeling.

---

## 6. Controleren en testen

### 6.1 Backend

Voer vanuit de hoofdmap uit:

```bash
dotnet restore SOC3.slnx
dotnet build SOC3.slnx
dotnet format SOC3.slnx --verify-no-changes
dotnet test SOC3.slnx
```

`Soc.Tests` bestaat, maar bevat momenteel nog geen testmethoden.

Daardoor kan `dotnet test` melden dat er nul tests gevonden zijn. Dit betekent niet dat de functionaliteit al getest is.

### 6.2 Frontend

Voer vanuit de hoofdmap uit:

```bash
cd frontend
npm ci
npm run lint
npm run build
npm audit
```

De frontendbuild voert TypeScript-controle en een Vite-build uit.

`npm audit` kan kwetsbaarheden melden in ontwikkelafhankelijkheden. Onderzoek deze meldingen voordat je dependencies bijwerkt.

Gebruik niet zonder onderzoek `npm audit fix --force`.

### 6.3 Testafspraken

Wanneer functionaliteit wordt toegevoegd, schrijven we passende tests.

Belangrijke testonderwerpen zijn:

- Alerts ontvangen en valideren.
- Dubbele alerts voorkomen.
- Tenant- en scope-isolatie.
- Gelijktijdig claimen van alerts.
- Database-transacties.
- Authenticatie en 2FA.
- Auditregistratie.
- Schemaresolutie en SQL-rechten.

### 6.4 GitHub Actions

GitHub Actions wordt gebruikt om controles automatisch uit te voeren zodra de workflows zijn ingericht.

De beoogde controles zijn:

- Backendbuild.
- Backendtests.
- Frontendlinting.
- Frontendbuild.
- Eventuele aanvullende beveiligings- en integratietests.

Verplichte statuschecks worden pas ingesteld als de workflows daadwerkelijk werken.

---

## 7. Problemen oplossen

| Probleem                      | Wat je controleert                                |
| ----------------------------- | ------------------------------------------------- |
| `dotnet` start niet           | Controleer de geïnstalleerde SDK en `global.json` |
| `npm` ontbreekt               | Controleer de Node.js-installatie                 |
| `npm ci` mislukt              | Controleer `package.json` en `package-lock.json`  |
| Backend start niet            | Controleer de build en de foutmelding             |
| Geen SOC-routes beschikbaar   | De eigen controllers zijn nog niet gebouwd        |
| Frontend bereikt de API niet  | De API-koppeling is nog niet ingericht            |
| `dotnet test` vindt 0 tests   | Er zijn nog geen testmethoden toegevoegd          |
| Push naar `develop` geweigerd | Gebruik een werkbranch en Pull Request            |
| Mergeconflict                 | Werk je branch bij vanuit `develop`               |

### Probleem melden

Vermeld bij een probleem:

```text
Besturingssysteem:
Softwareversies:
Branch:
Uitgevoerd commando:
Foutmelding:
Wat heb je al geprobeerd?
```

Deel nooit wachtwoorden, API-sleutels of andere gevoelige gegevens.

Zorg dat een probleem reproduceerbaar is, zodat andere groepsleden het kunnen onderzoeken.

---

## 8. Documentatie en vervolg

### 8.1 Ontwerpdocumenten

De belangrijkste ontwerpdocumenten zijn:

- **Functioneel Ontwerp (FO):** Beschrijft wat het platform moet doen.
- **Technisch Ontwerp v1.3 (TO):** Beschrijft de architectuur, database, API, beveiliging en technische afspraken.

Deze documenten vormen de basis voor de implementatie en bijbehorende tests.

De documenten zijn nog niet opgenomen in `docs/`. Voeg de actuele, voor het team vrijgegeven versies toe.

### 8.2 Ontwikkelvolgorde

De voorgestelde technische volgorde is:

1. Schema-per-klant-proof-of-concept en databaseafspraken valideren.
2. Authenticatie en toegangscontrole bouwen.
3. De eerste alert-API en bijbehorende tests ontwikkelen.
4. De React-schermen verbinden met de API.
5. Cases, onderzoekstijdlijn en overige user stories uitwerken.
6. GitHub Actions toevoegen en statuschecks verplicht maken wanneer ze betrouwbaar werken.

De daadwerkelijke planning en taakverdeling staan in Taiga.

### 8.3 Documentatie onderhouden

Deze README moet overeenkomen met de daadwerkelijke projectstructuur.

Werk hem bij wanneer:

- Installatiecommando's veranderen.
- Dependencies worden bijgewerkt.
- Configuratie wordt toegevoegd.
- De database-inrichting verandert.
- Nieuwe ontwikkelafspraken worden gemaakt.

Beschrijf wat daadwerkelijk werkt en maak onderscheid met functionaliteit die nog gepland staat.

---

**SOC3 — Projectgroep 3**
_HBO-ICT Software Engineering · De Haagse Hogeschool · 2026_
