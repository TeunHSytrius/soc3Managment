
# SOC3 — SOC Casemanagementplatform

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-TypeScript-149ECA?logo=react&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927)
![Status](https://img.shields.io/badge/Status-In_ontwikkeling-orange)

**De Haagse Hogeschool · HBO-ICT Software Engineering · Semester 3**

SOC3 is een casemanagementplatform voor een Security Operations Centre (SOC). Het platform ondersteunt het ontvangen en beoordelen van beveiligingsmeldingen, het uitvoeren van onderzoeken en het vastleggen van onderzoeksinformatie.

Deze README is de centrale handleiding voor de ontwikkelaars van projectgroep 3. Hierin staat hoe je het project lokaal start, welke codeafspraken gelden en hoe we samenwerken via GitHub.

**[GitHub-repository](https://github.com/TeunHSytrius/soc3Managment) · [Taiga-projectbord](https://tree.taiga.io/project/djominique-soc-groep-3/kanban)**

> **Status:** De projectbasis wordt nog ingericht. De onderstaande startcommando's zijn bedoeld voor de afgesproken projectstructuur en werken zodra die is toegevoegd. De database-initialisatie moet eerst worden uitgewerkt en getest. Tot die tijd is de volledige applicatie nog niet lokaal te starten.

---

## Inhoudsopgave

| Aan de slag | Ontwikkelafspraken |
|---|---|
| [1. Snelstart](#1-snelstart) | [4. Git-workflow](#4-git-workflow) |
| [2. Installatie en configuratie](#2-installatie-en-configuratie) | [5. Codeconventies](#5-codeconventies) |
| [3. Projectstructuur](#3-projectstructuur) | [6. Testen en beveiliging](#6-testen-en-beveiliging) |

[7. Problemen oplossen](#7-problemen-oplossen) · [8. Projectafspraken](#8-projectafspraken)

---

## 1. Snelstart

Deze instructies zijn bedoeld voor ontwikkelaars die de benodigde software al hebben geïnstalleerd.

### 1.1 Repository ophalen

```bash
git clone https://github.com/TeunHSytrius/soc3Managment.git
cd soc3Managment
git switch develop
```

We gebruiken `develop` als gezamenlijke ontwikkelbranch. `main` bevat de stabiele, goedgekeurde versie.

### 1.2 Backend installeren en starten

Vanuit de hoofdmap:

```bash
dotnet restore SOC3.slnx
dotnet run --project backend/Soc.Api
```

De backend heeft een werkende SQL Server-verbinding en een ingerichte database nodig.

Raadpleeg [Database configureren](#23-database-configureren) voordat je de applicatie voor het eerst start.

### 1.3 Frontend installeren en starten

Open een tweede terminal:

```bash
cd frontend
npm ci
npm run dev
```

Open het adres dat in de terminal wordt weergegeven, normaal gesproken `http://localhost:5173`.

De frontend communiceert via de API met de backend.

### 1.4 Dagelijks werken

Zorg eerst dat je de nieuwste versie van `develop` hebt:

```bash
git switch develop
git pull --ff-only origin develop
```

Start daarna de backend en frontend met de bovenstaande commando's.

Wanneer dependencies of de databasestructuur zijn gewijzigd, moeten deze eerst worden bijgewerkt.

---

## 2. Installatie en configuratie

### 2.1 Benodigde software

| Software | Versie |
|---|---|
| Git | Recente stabiele versie |
| .NET SDK | 10.x |
| Node.js | Afgesproken LTS-versie |
| npm | Meegeleverd met Node.js |
| SQL Server | Ondersteunde lokale of externe instantie |
| IDE | VS Code, Visual Studio of Rider |

Controleer de installatie:

```bash
git --version
dotnet --version
node --version
npm --version
```

Iedere ontwikkelaar gebruikt dezelfde afgesproken versies. Die worden waar mogelijk vastgelegd in de projectconfiguratie.

De .NET-backend en React-frontend kunnen op Windows, macOS en Linux worden ontwikkeld.

Voor SQL Server gebruiken we een ondersteunde lokale instantie of een bereikbare ontwikkelserver. De officiële SQL Server Linux-container heeft geen native ondersteuning voor Apple Silicon.

### 2.2 Lokale configuratie

Persoonlijke instellingen en geheimen worden niet in de repository opgeslagen.

We gebruiken hiervoor onder andere:

| Bestand of voorziening | Doel |
|---|---|
| `global.json` | Afgesproken .NET SDK-versie |
| `package-lock.json` | Vastgelegde frontenddependencies |
| `.editorconfig` | Codeopmaak en conventies |
| `.gitignore` | Uitsluiten van lokale en gegenereerde bestanden |
| `appsettings.json` | Niet-gevoelige backendinstellingen |
| .NET User Secrets | Gevoelige lokale backendinstellingen |

De definitieve configuratiebestanden worden tijdens de projectopzet toegevoegd.

### 2.3 Database configureren

Volgens het technisch ontwerp v1.3 gebruiken we **één SQL Server-database met een afzonderlijk schema per klant**.

De database bevat:

- Een centraal `platform`-schema voor accounts, klanten, scopes, rechten en geregistreerde bronnen.
- Een afzonderlijk klantschema voor alerts, cases, observables, tijdlijngegevens en klantgebonden auditregels.
- Een verdere scheiding binnen klanten door middel van `ScopeId`.

Voorbeeld:

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

De daadwerkelijke database-inrichting volgt de technische proef uit hoofdstuk 12 van het TO.

**Database-updates**

- Het platform-schema gebruikt EF Core-migrations.
- Klantschema's krijgen versiebeheerbare SQL-deploymentscripts.
- Nieuwe klantschema's worden pas gebruikt nadat de tabellen en SQL-rechten gecontroleerd zijn.
- De gewone EF Core-migrations worden niet automatisch voor alle klantschema's gebruikt.

**Nog in te richten:** de exacte database-initialisatie, de lokale verbindingsinstellingen en het uitvoeren van SQL-deploymentscripts.

Deze instructies worden aangevuld zodra de technische proef voor schema-per-klant is geslaagd. De ontwikkelomgeving moet daarna vanuit een lege database met minimaal twee fictieve klanten kunnen worden opgebouwd.

### 2.4 Backend configureren

De backend gebruikt ASP.NET Core en SQL Server.

Lokale, gevoelige instellingen worden buiten Git opgeslagen, bijvoorbeeld met .NET User Secrets.

De definitieve connection-stringnaam en de benodigde configuratiestappen worden vastgelegd bij het aanmaken van `Soc.Api`.

Gebruik nooit echte wachtwoorden of API-sleutels in `appsettings.json` of andere bestanden die naar Git worden gepusht.

---

## 3. Projectstructuur

De beoogde structuur volgt het technisch ontwerp v1.3.

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
├── docs/
│
├── .editorconfig
├── .gitignore
├── global.json
├── SOC3.slnx
└── README.md
```

De structuur wordt tijdens het opzetten van de applicatie aangemaakt.

### 3.1 Architectuur

```text
React + TypeScript
        │
        │ HTTPS / JSON
        ▼
ASP.NET Core Web API
        │
        ▼
Services en bedrijfsregels
        │
        ▼
Datalaag (EF Core / SQL)
        │
        ▼
SQL Server
```

De frontend communiceert alleen met de backend. De backend controleert de rechten en bepaalt welke databasegegevens toegankelijk zijn.

### 3.2 Verantwoordelijkheden

| Project | Verantwoordelijkheid |
|---|---|
| `Soc.Api` | Controllers, HTTP-verzoeken, authenticatie en configuratie |
| `Soc.Business` | Services, bedrijfsregels en request-/responsemodellen |
| `Soc.Data` | PlatformDbContext, TenantDbContext, schemaresolutie en datatoegang |
| `Soc.Tests` | Unit-tests en integratietests |
| `frontend` | React-componenten en gebruikersinterface |

**Afspraken**

- Controllers verwerken HTTP-verzoeken.
- Bedrijfsregels horen in services.
- Databasecode hoort in `Soc.Data`.
- Afhankelijkheden worden via dependency injection beschikbaar gemaakt.
- Een controller mag nooit zelf een SQL-schemanaam uit gebruikersinvoer bepalen.

Meer informatie staat in hoofdstuk 3 en 6 van het technisch ontwerp.

---

## 4. Git-workflow

We gebruiken GitHub voor versiebeheer en Taiga voor de projectplanning.

De Scrum Master wijst de tickets toe aan de groepsleden. Iedere werkbranch wordt gekoppeld aan het bijbehorende Taiga-ticket.

### 4.1 Branchstrategie

| Branch | Gebruik |
|---|---|
| `main` | Stabiele en goedgekeurde versie |
| `develop` | Gezamenlijke ontwikkelbranch |
| `feature/*` | Nieuwe functionaliteit |
| `bugfix/*` | Fouten oplossen |
| `chore/*` | Configuratie en onderhoud |
| `docs/*` | Documentatie |

```text
feature/* ─────┐
bugfix/* ──────┤
chore/* ───────┼──> develop ──> main
docs/* ────────┘
```

We werken niet rechtstreeks op `main` of `develop`.

### 4.2 Branchnaamgeving

**Iedere werkbranch bevat verplicht het Taiga-ticketnummer.**

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

Gebruik het echte ticketnummer, Engelse beschrijvingen, kleine letters en koppeltekens.

### 4.3 Een branch aanmaken

```bash
git switch develop
git pull --ff-only origin develop
git switch -c feature/tg-42-alert-ingestion
```

Vanaf dat moment werk je op je eigen branch.

### 4.4 Commitconventies

We gebruiken Conventional Commits met het Taiga-ticketnummer.

Formaat:

```text
<type>: <beschrijving> (TG-<nummer>)
```

| Type | Betekenis |
|---|---|
| `feat` | Nieuwe functionaliteit |
| `fix` | Bugfix |
| `refactor` | Code verbeteren |
| `test` | Tests |
| `docs` | Documentatie |
| `chore` | Configuratie en onderhoud |
| `ci` | CI-configuratie |

Voorbeelden:

```text
feat: add alert ingestion endpoint (TG-42)
fix: prevent duplicate alerts (TG-51)
test: add schema isolation tests (TG-62)
```

Commitberichten zijn in het Engels en beschrijven concreet wat er is gewijzigd.

### 4.5 Pull Requests

Push je branch:

```bash
git push -u origin feature/tg-42-alert-ingestion
```

Maak vervolgens een Pull Request naar `develop`.

**Voorbeeldtitel:**

```text
[TG-42] Implement alert ingestion endpoint
```

Vermeld in de Pull Request:

- Het bijbehorende Taiga-ticket.
- Wat er is aangepast.
- Hoe de wijziging is getest.
- Eventuele aandachtspunten voor de reviewer.

### 4.6 Code reviews

Voor `main` en `develop` zijn GitHub-rulesets ingesteld.

- Iedere wijziging gaat via een Pull Request.
- Minimaal één groepsgenoot moet goedkeuren.
- Openstaande reviewgesprekken worden afgehandeld.
- Nieuwe commits na goedkeuring vereisen opnieuw een review.
- Force pushes naar beschermde branches zijn niet toegestaan.

**Mergebeleid**

| Van | Naar | Methode |
|---|---|---|
| Werkbranch | `develop` | Squash and merge |
| `develop` | `main` | Create a merge commit |

### 4.7 Rebase

Je kunt je eigen werkbranch bijwerken met de nieuwste versie van `develop`:

```bash
git fetch origin
git rebase origin/develop
```

Gebruik rebase alleen bewust op eigen werkbranches. Herschrijf de commitgeschiedenis van gedeelde branches niet.

---

## 5. Codeconventies

We volgen hoofdstuk 3 van het technisch ontwerp v1.3.

**Alle namen in de code zijn Engels. Gebruikersteksten mogen Nederlands zijn.**

### 5.1 C# en ASP.NET Core

| Onderdeel | Conventie | Voorbeeld |
|---|---|---|
| Class | PascalCase | `AlertService` |
| Interface | I + PascalCase | `IAlertService` |
| Method | PascalCase | `ClaimAlertAsync()` |
| Property | PascalCase | `SchemaName` |
| Enum | PascalCase | `AlertStatus` |
| Enumwaarde | PascalCase | `InProgress` |
| Parameter | camelCase | `alertId` |
| Lokale variabele | camelCase | `currentUser` |
| Asynchrone method | Async-suffix | `GetAlertAsync()` |

Voorbeeld:

```csharp
public enum AlertStatus
{
    New,
    InProgress,
    LinkedToCase,
    Closed
}
```

**Algemene afspraken**

- Gebruik duidelijke en beschrijvende namen.
- Gebruik dependency injection.
- Gebruik `async` en `await` voor asynchrone I/O.
- Plaats bedrijfsregels in `Soc.Business`.
- Plaats databasecode in `Soc.Data`.
- Houd controllers beperkt tot HTTP-verwerking.
- Vermijd dubbele code en onnodige complexiteit.

### 5.2 React en TypeScript

| Onderdeel | Conventie | Voorbeeld |
|---|---|---|
| Component | PascalCase | `AlertList` |
| Componentbestand | PascalCase.tsx | `AlertList.tsx` |
| Functie | camelCase | `fetchAlerts()` |
| Variabele | camelCase | `selectedAlert` |
| Type/interface | PascalCase | `AlertResponse` |
| JSON-property | camelCase | `receivedAt` |

**Afspraken**

- Gebruik functionele React-componenten.
- Geef componenten één duidelijke verantwoordelijkheid.
- Gebruik TypeScript-types voor API-gegevens.
- Houd API-communicatie waar mogelijk gescheiden van presentatiecode.
- Plaats nooit servergeheimen in frontendcode.

### 5.3 Databaseconventies

| Onderdeel | Voorbeeld |
|---|---|
| Entity | `InvestigationCase` |
| SQL-tabel | `InvestigationCases` |
| Primaire sleutel | `Id` |
| Vreemde sleutel | `CaseId` |
| Werkruimte | `ScopeId` |
| Schema-identificatie | `SchemaName` |
| Versiecontrole | `RowVersion` |

Volgens het technisch ontwerp geldt:

- Centrale gebruikers- en klantgegevens staan in het platform-schema.
- Onderzoeksgegevens staan in het afzonderlijke schema van de klant.
- Onderzoekstabellen gebruiken geen `TenantId` als scheidingskolom.
- `ScopeId` wordt gebruikt voor afscherming binnen één klant.
- De backend bepaalt de schemannaam via gecontroleerde platformgegevens.
- SQL-rechten worden zo beperkt mogelijk ingesteld.
- Schemawijzigingen worden vastgelegd in versiebeheer.

### 5.4 API-conventies

Alle API-routes beginnen met:

```text
/api/v1
```

Voorbeelden:

```text
POST /api/v1/ingest/alerts
GET  /api/v1/alerts
GET  /api/v1/alerts/{id}
POST /api/v1/alerts/{id}/claim
POST /api/v1/alerts/{id}/triage

GET  /api/v1/cases
GET  /api/v1/cases/{id}
```

We gebruiken duidelijke request- en responsemodellen, zoals `ClaimAlertRequest` en `AlertResponse`.

De frontend stuurt waar nodig `X-Workspace-Id` en `X-Scope-Id` mee. De backend controleert de toegang en zoekt zelf het juiste klantschema op.

De frontend stuurt nooit rechtstreeks een SQL-schemanaam mee.

### 5.5 Codeopmaak

Waar mogelijk controleren we conventies automatisch:

| Onderdeel | Controle |
|---|---|
| C# | `.editorconfig`, `dotnet format` |
| Backend | `dotnet build`, `dotnet test` |
| TypeScript | TypeScript-buildcontrole |
| React | ESLint |
| Frontendopmaak | Prettier, zodra ingericht |

Deze controles worden tijdens de projectopzet geconfigureerd.

---

## 6. Testen en beveiliging

### 6.1 Tests uitvoeren

Controleer de backend vanuit de hoofdmap:

```bash
dotnet build SOC3.slnx
dotnet test SOC3.slnx
```

Controleer de frontend vanuit `frontend/`:

```bash
npm run build
```

De commando's zijn beschikbaar wanneer de bijbehorende projecten zijn aangemaakt.

### 6.2 Belangrijke testscenario's

Volgens het technisch ontwerp testen we onder andere:

- Geldige en ongeldige alerts.
- Dubbele alerts.
- Tenant- en scope-isolatie.
- Gelijktijdig claimen van alerts.
- Database-transacties.
- Authenticatie en 2FA.
- Auditregistratie.
- Onjuiste schemaresolutie.
- SQL-rechten tussen klantschema's.
- EF Core-modelcaching bij meerdere klanten.

Voor de beveiligingstests gebruiken we een echte SQL Server-testdatabase met minimaal twee afzonderlijke klantschema's.

### 6.3 Beveiligingsafspraken

- Autorisatie wordt altijd in de backend gecontroleerd.
- Iedere klant krijgt een afzonderlijk databaseschema.
- Een gebruiker mag alleen toegestane scopes benaderen.
- De frontend bepaalt nooit zelf de databasebeveiliging.
- Wachtwoorden en API-sleutels worden niet in Git opgeslagen.
- Belangrijke handelingen worden geaudit.
- Klantschema's en SQL-rechten worden gecontroleerd getest.

Meer informatie staat in hoofdstuk 8 van het technisch ontwerp.

### 6.4 Continuous Integration

GitHub Actions wordt toegevoegd zodra de basisprojecten bestaan.

De CI controleert vervolgens:

- Backendbuild.
- Backendtests.
- Frontendbuild.
- Waar ingericht: codeconventies en aanvullende tests.

Verplichte CI-statuschecks worden pas ingeschakeld wanneer de workflows betrouwbaar werken.

---

## 7. Problemen oplossen

| Probleem | Controleer |
|---|---|
| `dotnet` wordt niet herkend | Is .NET SDK 10 geïnstalleerd? |
| `npm` wordt niet herkend | Is Node.js geïnstalleerd? |
| Backend start niet | Buildfouten en configuratie |
| Databaseverbinding mislukt | SQL Server, rechten en connection string |
| Klantschema ontbreekt | Is het schema correct ingericht? |
| Database-update mislukt | Platform-migrations of schema-deploymentscripts |
| Frontend bereikt de API niet | Backendadres en proxyconfiguratie |
| Git-conflict | Eigen branch en wijzigingen uit `develop` |
| Tests mislukken | Testuitvoer en benodigde testomgeving |

### Probleem melden

Kun je een probleem niet oplossen? Deel dan:

```text
Besturingssysteem:
Softwareversies:
Branch:
Uitgevoerd commando:
Foutmelding:
Wat heb je al geprobeerd?
```

Deel geen wachtwoorden, API-sleutels of andere geheimen.

Een foutmelding moet duidelijk en reproduceerbaar zijn, zodat een ander groepslid het probleem kan onderzoeken.

---

## 8. Projectafspraken

### 8.1 Definition of Done

Een wijziging is pas afgerond wanneer:

- [ ] De afgesproken functionaliteit werkt.
- [ ] De codeconventies zijn gevolgd.
- [ ] De relevante tests zijn uitgevoerd.
- [ ] De applicatie succesvol bouwt.
- [ ] Databasewijzigingen zijn vastgelegd.
- [ ] De documentatie waar nodig is bijgewerkt.
- [ ] De Pull Request is goedgekeurd.
- [ ] Reviewopmerkingen zijn afgehandeld.
- [ ] De wijzigingen zijn gemerged naar `develop`.

### 8.2 Documentatie onderhouden

Deze README moet overeenkomen met de daadwerkelijke projectstructuur.

Werk de README bij wanneer installatiecommando's, softwareversies, configuratie, database-inrichting of ontwikkelafspraken veranderen.

Een wijziging aan de ontwikkelomgeving wordt gecontroleerd door de installatie op een andere geschikte computer opnieuw uit te voeren.

### 8.3 Ontwerpdocumenten

Onze belangrijkste ontwerpdocumenten zijn:

- **Functioneel Ontwerp v1.1:** beschrijft wat de applicatie moet kunnen en hoe gebruikers ermee werken.
- **Technisch Ontwerp v1.3:** beschrijft hoe de applicatie wordt gebouwd, inclusief de architectuur, database, API en beveiliging.

Deze documenten vormen de basis voor de implementatie en tests.

### 8.4 Gezamenlijke werkwijze

> **Eén repository, één projectstructuur en één werkwijze.**
>
> Ieder groepslid moet de applicatie zelfstandig kunnen installeren, starten, ontwikkelen en testen op een ondersteunde computer.
>
> We volgen de vastgelegde codeconventies en Git-afspraken. Problemen met de ontwikkelomgeving moeten reproduceerbaar zijn en worden gezamenlijk opgelost.

---

**SOC3 — Projectgroep 3**  
HBO-ICT Software Engineering · De Haagse Hogeschool · 2026
