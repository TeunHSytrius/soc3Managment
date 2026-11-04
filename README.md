<div align="center">

# SOC3

### SOC Casemanagementplatform

**De Haagse Hogeschool · HBO-ICT Software Engineering · Semester 3**

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-TypeScript-149ECA?logo=react&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927)
![GitHub](https://img.shields.io/badge/GitHub-Team_Workflow-181717?logo=github&logoColor=white)
![Status](https://img.shields.io/badge/Status-In_ontwikkeling-orange)

**[Repository](https://github.com/TeunHSytrius/soc3Managment)** · **[Taiga](https://tree.taiga.io/project/djominique-soc-groep-3/kanban)**

</div>

---

## Over het project

SOC3 is een webapplicatie voor het beheren van security-alerts en onderzoeken binnen een Security Operations Centre (SOC).

Het platform ondersteunt analisten bij het ontvangen en beoordelen van alerts, het beheren van cases, het vastleggen van onderzoeksinformatie en het samenwerken aan incidenten.

Deze README is de centrale handleiding voor alle ontwikkelaars. Hierin staan de installatie-instructies, projectstructuur, codeconventies en Git-afspraken.

> **Huidige status**
>
> De repository wordt voorbereid op de ontwikkeling. De start- en controlescripts in deze README zijn de afgesproken doelstructuur en moeten nog worden gerealiseerd. Tot die tijd is de applicatie niet lokaal op te starten.

## Inhoudsopgave

| Aan de slag                                    | Ontwikkelafspraken                                             |
| ---------------------------------------------- | -------------------------------------------------------------- |
| [01 · Snelstart](#01--snelstart)               | [04 · Git-workflow](#04--git-workflow)                         |
| [02 · Installatie](#02--installatie)           | [05 · Codeconventies](#05--codeconventies)                     |
| [03 · Projectstructuur](#03--projectstructuur) | [06 · Kwaliteit en beveiliging](#06--kwaliteit-en-beveiliging) |

**Overig:** [07 · Problemen oplossen](#07--problemen-oplossen) · [08 · Projectafspraken](#08--projectafspraken)

---

# 01 · Snelstart

Het uitgangspunt is dat ieder groepslid de applicatie op een eigen computer kan installeren, starten en testen zonder handmatig projectbestanden te wijzigen.

### Benodigdheden

| Software   | Versie                                   |
| ---------- | ---------------------------------------- |
| Git        | Recente stabiele versie                  |
| .NET SDK   | 10.x                                     |
| Node.js    | 24 LTS                                   |
| npm        | Via Node.js                              |
| SQL Server | Ondersteunde lokale of externe instantie |
| IDE        | VS Code, Visual Studio of Rider          |

Voor een lokale SQL Server-container is daarnaast Docker met Compose nodig op een ondersteund systeem.

### Windows — PowerShell

```powershell
git clone https://github.com/TeunHSytrius/soc3Managment.git
cd soc3Managment
git switch develop

# Eenmalige configuratie
.\scripts\setup.ps1

# Applicatie starten
.\scripts\dev.ps1

# Projectcontroles uitvoeren
.\scripts\check.ps1
```

### macOS / Linux — Terminal

```bash
git clone https://github.com/TeunHSytrius/soc3Managment.git
cd soc3Managment
git switch develop

# Eenmalige configuratie
bash scripts/setup.sh

# Applicatie starten
bash scripts/dev.sh

# Projectcontroles uitvoeren
bash scripts/check.sh
```

> **Let op:** `setup`, `dev` en `check` zijn nog te ontwikkelen scripts. Deze commando's zijn pas uitvoerbaar wanneer de scripts aan de repository zijn toegevoegd en op de ondersteunde ontwikkelomgevingen zijn getest.

### Wat moeten deze scripts straks doen?

| Script  | Verantwoordelijkheid                                                                                             |
| ------- | ---------------------------------------------------------------------------------------------------------------- |
| `setup` | Softwareversies controleren, dependencies herstellen, lokale configuratie begeleiden en de database voorbereiden |
| `dev`   | De benodigde services, backend en frontend starten                                                               |
| `check` | Codeopmaak, builds en automatische tests controleren                                                             |

Bij fouten moeten de scripts een begrijpelijke melding geven met de ontbrekende instelling of de benodigde vervolgstap.

**De applicatie moet op iedere ondersteunde ontwikkelomgeving dezelfde resultaten opleveren.**

---

# 02 · Installatie

## Software controleren

Controleer de geïnstalleerde tools:

```bash
git --version
dotnet --version
node --version
npm --version
```

Wanneer je Docker gebruikt:

```bash
docker --version
docker compose version
```

De afgesproken versies worden waar mogelijk vastgelegd in de repository.

| Bestand                     | Doel                                |
| --------------------------- | ----------------------------------- |
| `global.json`               | .NET SDK-versie vastleggen          |
| `package-lock.json`         | Frontenddependencies vastleggen     |
| `.env.example`              | Benodigde configuratie documenteren |
| `.editorconfig`             | Algemene codeopmaak en conventies   |
| `compose.yaml`              | Databaseomgeving definiëren         |
| `.config/dotnet-tools.json` | Lokale .NET-tools vastleggen        |

## Lokale configuratie

Iedere ontwikkelaar gebruikt een eigen configuratie.

Persoonlijke instellingen mogen niet afhankelijk zijn van de computer van een ander groepslid.

Gevoelige backendinstellingen worden lokaal beheerd, bijvoorbeeld met .NET User Secrets.

Wanneer `.env.example` beschikbaar is, maak je een lokale kopie.

**Windows:**

```powershell
Copy-Item .env.example .env
```

**macOS / Linux:**

```bash
cp .env.example .env
```

Het bestand `.env` wordt uitgesloten van Git.

Bewaar geen wachtwoorden, authenticatietokens of API-sleutels in de repository.

## Database

De applicatie gebruikt Microsoft SQL Server met Entity Framework Core 10.

We gebruiken migrations om de databasestructuur reproduceerbaar aan te maken en bij te werken.

De databaseomgeving moet aan de volgende voorwaarden voldoen:

- Iedere ontwikkelaar heeft toegang tot een eigen ontwikkel- of testdatabase.
- De database kan vanuit een lege omgeving worden opgebouwd.
- Databasewijzigingen worden vastgelegd in migrations.
- Connection strings worden via lokale configuratie ingesteld.
- Er zijn geen persoonlijke bestandslocaties of computerafhankelijke instellingen in de applicatiecode opgenomen.

### Ondersteuning per besturingssysteem

| Platform | Databaseomgeving                                           |
| -------- | ---------------------------------------------------------- |
| Windows  | Lokale SQL Server of ondersteunde Docker-configuratie      |
| Linux    | Ondersteunde SQL Server-installatie of Docker-configuratie |
| macOS    | Bereikbare SQL Server-instantie op een ondersteund systeem |

De officiële SQL Server Linux-container ondersteunt Apple Silicon niet als native ARM64-platform. Daarom moet voor zulke computers een geschikte externe SQL Server-omgeving beschikbaar zijn.

De ontwikkelomgeving wordt pas als gereed beschouwd wanneer de installatie op de daadwerkelijk gebruikte computers is getest.

## Handmatige commando's

Wanneer de basisprojecten bestaan, kunnen deze commando's worden gebruikt voor afzonderlijke controles.

**Backenddependencies herstellen:**

```bash
dotnet restore SOC3.slnx
```

**Backend starten:**

```bash
dotnet run --project backend/Soc.Api
```

**Frontend starten:**

```bash
cd frontend
npm ci
npm run dev
```

**Database-migrations uitvoeren:**

```bash
dotnet tool restore
dotnet ef database update --project backend/Soc.Data --startup-project backend/Soc.Api
```

De database-migrations vereisen een correct geconfigureerde databaseverbinding.

Deze handmatige commando's zijn bedoeld voor ontwikkeling en probleemoplossing. De definitieve snelstart moet de gebruikelijke handelingen zoveel mogelijk automatiseren.

---

# 03 · Projectstructuur

De projectstructuur is gebaseerd op het technisch ontwerp, versie 1.2.

```text
soc3Managment/
│
├── .github/
│   └── workflows/
│
├── backend/
│   ├── Soc.Api/
│   ├── Soc.Business/
│   ├── Soc.Data/
│   └── Soc.Tests/
│
├── frontend/
│
├── scripts/
│   ├── setup.ps1
│   ├── setup.sh
│   ├── dev.ps1
│   ├── dev.sh
│   ├── check.ps1
│   └── check.sh
│
├── docs/
├── .config/
│   └── dotnet-tools.json
│
├── .editorconfig
├── .env.example
├── .gitignore
├── compose.yaml
├── global.json
├── SOC3.slnx
└── README.md
```

Dit is de beoogde structuur. De bestanden worden tijdens de projectopzet daadwerkelijk aangemaakt.

## Architectuur

```text
React + TypeScript
        │
        │ HTTPS / JSON
        ▼
ASP.NET Core Web API
        │
        │ Entity Framework Core
        ▼
    SQL Server
```

De frontend communiceert uitsluitend via de backend met de database.

### Backendlagen

| Project        | Verantwoordelijkheid                                           |
| -------------- | -------------------------------------------------------------- |
| `Soc.Api`      | Controllers, HTTP-verzoeken, authenticatie en API-configuratie |
| `Soc.Business` | Services, bedrijfsregels, request- en responsemodellen         |
| `Soc.Data`     | Database-entiteiten, AppDbContext, configuratie en migrations  |
| `Soc.Tests`    | Unit-tests en integratietests                                  |

De projectreferenties volgen het technisch ontwerp:

```text
Soc.Api ──> Soc.Business ──> Soc.Data
    │
    └────────────────────────> Soc.Data
          (databaseconfiguratie)
```

**Belangrijk:**

- Controllers bevatten geen onnodige bedrijfslogica.
- Services voeren de bedrijfsregels uit.
- Databasecode wordt beheerd in de datalaag.
- Afhankelijkheden worden via dependency injection beschikbaar gemaakt.
- API-responses gebruiken aparte responsemodellen.

---

# 04 · Git-workflow

We gebruiken GitHub voor versiebeheer en Taiga voor de projectplanning.

De Scrum Master wijst tickets toe via Taiga. Elke werkbranch moet gekoppeld zijn aan een toegewezen ticket.

## Branchstrategie

| Branch      | Doel                             |
| ----------- | -------------------------------- |
| `main`      | Stabiele en goedgekeurde versies |
| `develop`   | Gezamenlijke ontwikkelbranch     |
| `feature/*` | Nieuwe functionaliteit           |
| `bugfix/*`  | Fouten oplossen                  |
| `chore/*`   | Projectconfiguratie en onderhoud |
| `docs/*`    | Documentatie                     |

```text
feature/* ─────┐
bugfix/* ──────┤
chore/* ───────┼──> develop ──> main
docs/* ────────┘
```

Rechtstreeks ontwikkelen op `main` en `develop` is niet toegestaan.

## Branchnaamgeving

**Het Taiga-ticketnummer is verplicht.**

Formaat:

```text
<type>/tg-<ticketnummer>-<omschrijving>
```

Voorbeelden:

```text
feature/tg-42-alert-ingestion
feature/tg-43-case-management
bugfix/tg-51-alert-status
chore/tg-52-database-setup
docs/tg-53-update-readme
```

Regels:

- Gebruik het echte Taiga-ticketnummer.
- Gebruik kleine letters en Engelse omschrijvingen.
- Gebruik kebab-case.
- Houd branchnamen kort en duidelijk.
- Maak werkbranches vanaf `develop`.

## Een nieuwe branch maken

Werk eerst `develop` bij:

```bash
git switch develop
git pull --ff-only origin develop
```

Maak daarna je branch:

```bash
git switch -c feature/tg-42-alert-ingestion
```

## Commits

We gebruiken Conventional Commits met een ticketnummer.

Formaat:

```text
<type>: <beschrijving> (TG-<nummer>)
```

| Type       | Betekenis                               |
| ---------- | --------------------------------------- |
| `feat`     | Nieuwe functionaliteit                  |
| `fix`      | Bugfix                                  |
| `refactor` | Code verbeteren zonder gedragswijziging |
| `test`     | Tests toevoegen of aanpassen            |
| `docs`     | Documentatie                            |
| `chore`    | Configuratie en onderhoud               |
| `ci`       | CI-workflows                            |

Voorbeelden:

```text
feat: add alert ingestion endpoint (TG-42)
fix: prevent duplicate alerts (TG-51)
test: add tenant isolation tests (TG-62)
chore: configure database (TG-52)
```

Commitberichten zijn in het Engels en beschrijven concreet de wijziging.

Vermijd berichten zoals `update`, `changes` of `fix stuff`.

## Pull Requests

Iedere wijziging naar een beschermde branch gaat via een Pull Request.

**Voorbeeldtitel:**

```text
[TG-42] Implement alert ingestion endpoint
```

Een Pull Request bevat minimaal:

1. Een verwijzing naar het Taiga-ticket.
2. Een korte beschrijving van de wijzigingen.
3. Een toelichting op de uitgevoerde tests.
4. Eventuele aandachtspunten voor de reviewer.

### Reviewafspraken

Voor `main` en `develop` zijn GitHub-rulesets ingesteld.

- Minimaal één groepsgenoot keurt de wijziging goed.
- Openstaande reviewgesprekken worden opgelost.
- Nieuwe commits na een goedkeuring vereisen opnieuw een review.
- Force pushes naar beschermde branches zijn niet toegestaan.
- Beschermde branches mogen niet worden verwijderd.

### Mergebeleid

| Van        | Naar      | Methode               |
| ---------- | --------- | --------------------- |
| Werkbranch | `develop` | Squash and merge      |
| `develop`  | `main`    | Create a merge commit |

## Rebase

Je kunt je eigen werkbranch bijwerken met de nieuwste wijzigingen uit `develop`:

```bash
git fetch origin
git rebase origin/develop
```

Gebruik rebase alleen bewust op eigen werkbranches. Herschrijf niet de geschiedenis van gedeelde branches.

Bij conflicten moeten de wijzigingen worden gecontroleerd voordat ze opnieuw worden gepusht.

---

# 05 · Codeconventies

De codeconventies volgen hoofdstuk 3 van het technisch ontwerp.

**Namen in de code zijn Engels. Gebruikersteksten en toelichtingen kunnen Nederlands zijn.**

## C# en ASP.NET Core

| Onderdeel         | Conventie      | Voorbeeld           |
| ----------------- | -------------- | ------------------- |
| Class             | PascalCase     | `AlertService`      |
| Interface         | I + PascalCase | `IAlertService`     |
| Method            | PascalCase     | `ClaimAlertAsync()` |
| Property          | PascalCase     | `TenantId`          |
| Enum              | PascalCase     | `AlertStatus`       |
| Enumwaarde        | PascalCase     | `InProgress`        |
| Parameter         | camelCase      | `alertId`           |
| Lokale variabele  | camelCase      | `currentUser`       |
| Asynchrone method | Async-suffix   | `GetAlertAsync()`   |

**Voorbeeld:**

```csharp
public enum AlertStatus
{
    New,
    InProgress,
    LinkedToCase,
    Closed
}
```

### Algemene C#-afspraken

- Gebruik duidelijke en beschrijvende namen.
- Gebruik `async` en `await` voor asynchrone I/O.
- Laat asynchrone methods eindigen op `Async`.
- Gebruik dependency injection voor services.
- Houd controllers beperkt tot HTTP-verwerking.
- Plaats bedrijfsregels in `Soc.Business`.
- Plaats EF Core-code in `Soc.Data`.
- Vermijd duplicatie en onnodige complexiteit.

## React en TypeScript

| Onderdeel        | Conventie      | Voorbeeld       |
| ---------------- | -------------- | --------------- |
| Component        | PascalCase     | `AlertList`     |
| Componentbestand | PascalCase.tsx | `AlertList.tsx` |
| Functie          | camelCase      | `fetchAlerts()` |
| Variabele        | camelCase      | `selectedAlert` |
| Type/interface   | PascalCase     | `AlertResponse` |
| JSON-property    | camelCase      | `receivedAt`    |

### Frontendafspraken

- Gebruik functionele React-componenten.
- Geef iedere component een duidelijke verantwoordelijkheid.
- Gebruik TypeScript-types voor API-gegevens.
- Houd API-communicatie zoveel mogelijk gescheiden van de gebruikersinterface.
- Plaats geen geheime sleutels of wachtwoorden in frontendcode.
- Autorisatie wordt altijd door de backend afgedwongen.

## Entity Framework Core

| Onderdeel        | Naamgeving           |
| ---------------- | -------------------- |
| Entity           | `InvestigationCase`  |
| SQL-tabel        | `InvestigationCases` |
| Primaire sleutel | `Id`                 |
| Verwijzing       | `CaseId`             |
| Klantcontext     | `TenantId`           |
| Werkruimte       | `ScopeId`            |
| Versiecontrole   | `RowVersion`         |

Volgens het huidige technisch ontwerp gebruiken we gedeelde tabellen met gecontroleerde tenant- en scope-toegang.

We gebruiken:

- EF Core-migrations voor databasewijzigingen.
- Queryfilters voor standaardgegevensafscherming.
- Aanvullende controles bij het opslaan.
- Databaseconstraints voor de integriteit van relaties.
- UTC voor opgeslagen tijdstippen.

Een verandering in databasearchitectuur wordt eerst gezamenlijk besloten en vastgelegd in het technisch ontwerp.

## API-conventies

Alle API-routes beginnen met:

```text
/api/v1
```

Voorbeelden:

```text
GET  /api/v1/alerts
GET  /api/v1/alerts/{id}
POST /api/v1/alerts/{id}/claim
POST /api/v1/alerts/{id}/triage

GET  /api/v1/cases
GET  /api/v1/cases/{id}
```

Request- en responsemodellen hebben duidelijke namen:

```text
ClaimAlertRequest
AlertResponse
AddCaseEventRequest
CaseEventResponse
```

We geven geen volledige database-entiteiten rechtstreeks terug via API-responses.

JSON-properties gebruiken camelCase.

## Automatische controle

We leggen codeopmaak en kwaliteitsregels waar mogelijk vast in configuratiebestanden.

| Technologie    | Controle                           |
| -------------- | ---------------------------------- |
| C#             | `.editorconfig` en `dotnet format` |
| .NET           | `dotnet build` en `dotnet test`    |
| TypeScript     | TypeScript-buildcontrole           |
| React          | ESLint                             |
| Frontendopmaak | Prettier                           |

Deze controles worden onderdeel van het geplande `check`-script en later van GitHub Actions.

Codeconventies zijn daardoor niet alleen leesafspraken, maar worden waar mogelijk automatisch gecontroleerd.

---

# 06 · Kwaliteit en beveiliging

## Testen

Iedere wijziging moet lokaal gecontroleerd kunnen worden.

De uiteindelijke standaardcontrole wordt:

**Windows:**

```powershell
.\scripts\check.ps1
```

**macOS / Linux:**

```bash
bash scripts/check.sh
```

De controlescripts worden toegevoegd tijdens de projectopzet.

### Handmatige controles

**Backend bouwen:**

```bash
dotnet build SOC3.slnx
```

**Backendtests uitvoeren:**

```bash
dotnet test SOC3.slnx
```

**Frontend controleren:**

```bash
cd frontend
npm run build
```

De scripts moeten deze en de overige afgesproken kwaliteitscontroles op een consistente manier uitvoeren.

## Belangrijke testgebieden

Volgens het technisch ontwerp testen we onder andere:

- Ontvangen en valideren van alerts.
- Voorkomen van dubbele alerts.
- Tenant- en scope-isolatie.
- Gelijktijdig claimen van alerts.
- Correcte database-transacties.
- Authenticatie en tweefactorauthenticatie.
- Rolgebaseerde toegangscontrole.
- Auditregistratie.
- Statusovergangen van cases.

Voor database-integriteit en tenant-scheiding zijn integratietests met een echte SQL Server-database nodig.

## Beveiligingsregels

Omdat de applicatie gegevens van meerdere klanten verwerkt, gelden de volgende basisregels:

- Autorisatie wordt afgedwongen in de backend.
- Gebruikers krijgen alleen toegang tot toegestane tenants en scopes.
- Databasekoppelingen mogen geen klantgrenzen doorbreken.
- Wachtwoorden en API-sleutels worden niet in Git opgeslagen.
- Gevoelige gegevens worden niet onnodig gelogd.
- Belangrijke acties worden vastgelegd in het auditlog.
- API-responses bevatten alleen toegestane gegevens.

Raadpleeg hoofdstuk 8 van het technisch ontwerp voor de volledige beveiligingsuitwerking.

## Continuous Integration

GitHub Actions wordt ingericht wanneer de basisprojecten en automatische tests beschikbaar zijn.

De CI controleert dan onder andere:

- Backendbuild.
- Backendtests.
- Frontendbuild.
- Codeconventies.
- Aanvullende kwaliteitscontroles.

Zodra deze controles betrouwbaar werken, worden de relevante CI-statuschecks verplicht gemaakt voor Pull Requests.

---

# 07 · Problemen oplossen

Een ontwikkelaar moet veelvoorkomende problemen zelfstandig kunnen herkennen en oplossen.

| Probleem                   | Eerste controle                                                 |
| -------------------------- | --------------------------------------------------------------- |
| `.NET` niet gevonden       | Controleer `dotnet --version`                                   |
| `npm` niet gevonden        | Controleer `node --version` en `npm --version`                  |
| Backend start niet         | Controleer buildfouten en lokale configuratie                   |
| Databaseverbinding mislukt | Controleer SQL Server, netwerkverbinding en connection string   |
| Frontend bereikt API niet  | Controleer de backend en de Vite-proxy                          |
| Migrations mislukken       | Controleer databaseverbinding, rechten en migrationstatus       |
| Git-mergeconflict          | Controleer de gewijzigde bestanden en branchgeschiedenis        |
| Tests falen                | Lees de volledige testuitvoer en controleer de afhankelijkheden |

### Probleem melden

Wanneer een probleem niet zelfstandig kan worden opgelost, vermeld dan:

```text
Besturingssysteem:
Softwareversies:
Branch:
Uitgevoerd commando:
Volledige foutmelding:
Reeds geprobeerd:
```

Verwijder altijd wachtwoorden, tokens en andere geheimen uit gedeelde logs en screenshots.

Een probleem moet reproduceerbaar zijn. Vermeld daarom niet alleen dat iets niet werkt, maar ook welke stap mislukt.

---

# 08 · Projectafspraken

## Definition of Done

Een wijziging is pas afgerond wanneer:

- [ ] De afgesproken functionaliteit werkt.
- [ ] De code de projectconventies volgt.
- [ ] De relevante tests zijn uitgevoerd.
- [ ] De applicatie correct bouwt.
- [ ] Eventuele database-migrations zijn toegevoegd.
- [ ] De documentatie waar nodig is bijgewerkt.
- [ ] Een groepsgenoot de Pull Request heeft goedgekeurd.
- [ ] Openstaande reviewopmerkingen zijn afgehandeld.
- [ ] De wijziging is samengevoegd met `develop`.

Wijzigingen aan de installatie of ontwikkelomgeving worden ook op een tweede geschikte computer gecontroleerd.

## README onderhouden

Deze README moet altijd overeenkomen met de werkelijke repository.

Wijzigingen aan de volgende onderdelen vereisen waar nodig een aanpassing van de documentatie:

- Softwareversies.
- Installatiestappen.
- Configuratiebestanden.
- Startcommando's.
- Projectstructuur.
- Database-instructies.
- Codeconventies.
- Git-workflow.

## Uitgangspunten

> **Eén repository. Eén afgesproken projectstructuur. Eén werkwijze.**
>
> Ieder groepslid moet de applicatie zelfstandig kunnen installeren, starten, ontwikkelen en testen op een ondersteunde computer.
>
> De README beschrijft hoe dat werkt. De projectconfiguratie en automatische controles moeten ervoor zorgen dat de afspraken daadwerkelijk worden nageleefd.

---

<div align="center">

**SOC3 · Projectgroep 3**

HBO-ICT Software Engineering
De Haagse Hogeschool · 2026

</div>
