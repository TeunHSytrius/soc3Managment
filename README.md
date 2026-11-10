# SOC3 — SOC-casemanagementplatform

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![React en TypeScript](https://img.shields.io/badge/Frontend-React%20%2B%20TypeScript-149ECA?logo=react&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927)
![Status](https://img.shields.io/badge/Status-Scaffolding%20gereed-orange)

**De Haagse Hogeschool · HBO-ICT Software Engineering · Semester 3 · Projectgroep 3**

Wij ontwikkelen een casemanagementplatform voor een Security Operations Centre (SOC). Het doel is om beveiligingsalerts te ontvangen en te beoordelen, onderzoeken als cases vast te leggen en klantgegevens strikt van elkaar te scheiden.

[Repository](https://github.com/TeunHSytrius/soc3Managment) · [Taiga-projectbord](https://tree.taiga.io/project/djominique-soc-groep-3/kanban)

> **Huidige stand:** de .NET 10-solution en React/TypeScript-frontend zijn aangemaakt en kunnen afzonderlijk bouwen en starten. De API bevat nog geen SOC-endpoints; de frontend is nog niet gekoppeld aan de API. Database-inrichting, authenticatie, alerts, cases en CI zijn nog niet geïmplementeerd.

## Inhoud

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

### Benodigd

| Software | Wat we gebruiken |
|---|---|
| Git | Repository clonen en branches beheren |
| .NET SDK | **10.0.401**, volgens `global.json` (nieuwere patch binnen dezelfde featureband toegestaan) |
| Node.js en npm | Een Node.js-versie die geschikt is voor de gebruikte Vite-versie |
| Editor | VS Code, Rider of Visual Studio |
| SQL Server | Pas nodig zodra de databasefunctionaliteit wordt aangesloten |

Controleer je installatie:

```bash
git --version
dotnet --version
node --version
npm --version
```

Als de gevraagde .NET SDK ontbreekt, kan `dotnet` een melding geven over `global.json`. Installeer dan de passende SDK; wijzig niet alleen voor je eigen computer de gezamenlijke versie.

### Repository ophalen

```bash
git clone https://github.com/TeunHSytrius/soc3Managment.git
cd soc3Managment
git switch develop
git pull --ff-only origin develop
```

De broncode waar we samen aan ontwikkelen staat op `develop`. Begin nieuwe werkzaamheden vanuit deze branch, maar maak voor je eigen ticket een werkbranch.

### Backend starten

Open een terminal in de hoofdmap:

```bash
dotnet restore SOC3.slnx
dotnet build SOC3.slnx
dotnet run --project backend/Soc.Api --launch-profile http
```

Het ingestelde HTTP-ontwikkeladres is **http://localhost:5042**. In de ontwikkelomgeving biedt de template een OpenAPI-document op **http://localhost:5042/openapi/v1.json**. Er zijn nog geen eigen SOC-controllers met werkende routes.

De huidige API heeft **nog geen SQL Server-verbinding nodig**: die koppeling moet nog gebouwd worden.

### Frontend starten

Open een **tweede terminal**, ook vanuit de hoofdmap:

```bash
cd frontend
npm ci
npm run dev
```

Open het adres dat Vite toont, doorgaans **http://localhost:5173**. Je ziet nu een eenvoudige SOC-startpagina, geen werkend alert- of caseoverzicht.

`npm ci` gebruikt de versies uit `package-lock.json`. Commit dat lockbestand altijd wanneer dependencies veranderen; commit `node_modules/` niet.

### Stoppen en opnieuw starten

Stop een ontwikkelserver met **Ctrl+C**. Bij een volgende werkdag volstaan doorgaans `dotnet run --project backend/Soc.Api --launch-profile http` en, in een tweede terminal vanuit `frontend/`, `npm run dev`. Installeer packages opnieuw als het lockbestand is gewijzigd.

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

Lege mappen zijn met `.gitkeep` in Git opgenomen. Voeg echte bestanden pas toe wanneer je aan de bijbehorende taak werkt.

| Project | Verantwoordelijkheid |
|---|---|
| `Soc.Api` | ASP.NET Core-controllers, HTTP-verzoeken en applicatieconfiguratie |
| `Soc.Business` | Bedrijfsregels, services en request-/responsemodellen |
| `Soc.Data` | Datatoegang, databasecontexten en schemaresolutie |
| `Soc.Tests` | Unit- en integratietests |
| `frontend` | React-componenten, schermen en API-aanroepen |
| `database` | Later: versiebeheerbare SQL-scripts voor klantschema's |
| `docs` | Ontwerpen en technische beslissingen |

De API verwijst al naar `Soc.Business` en `Soc.Data`; `Soc.Data` verwijst naar `Soc.Business`. Deze referenties vormen de basis, niet een voltooide implementatie.

**Architectuur volgens het technisch ontwerp:** React/TypeScript → ASP.NET Core API → services → datatoegang → SQL Server. De verbindingen en beveiligingsregels worden tijdens de realisatie toegevoegd.

---

## 3. Samenwerken via GitHub en Taiga

**Taiga is voor de planning**: user stories, taken, bugs en sprintvoortgang. **GitHub is voor de code**: branches, commits, Pull Requests en reviews.

### Branches

| Branch | Gebruik |
|---|---|
| `main` | Goedgekeurde, stabiele stand |
| `develop` | Gezamenlijke ontwikkelversie |
| `feature/tg-<nummer>-omschrijving` | Nieuwe functionaliteit |
| `bugfix/tg-<nummer>-omschrijving` | Bug oplossen |
| `chore/tg-<nummer>-omschrijving` | Technisch onderhoud |
| `docs/tg-<nummer>-omschrijving` | Documentatie |

Gebruik het **echte Taiga-ticketnummer** en een korte Engelse omschrijving. Maak zo nodig eerst een Taiga-taak aan; verzin geen nummer.

### Werken aan een ticket

```bash
git switch develop
git pull --ff-only origin develop
git switch -c feature/tg-59-alert-access
```

De branchnaam hierboven is een **voorbeeld**; gebruik de juiste branchnaam voor je toegewezen ticket.

Na je wijziging:

```bash
git status
git add -A
git commit -m "feat: add alert access checks (TG-59)"
git push -u origin feature/tg-59-alert-access
```

Open daarna een **Pull Request naar `develop`**, niet naar `main`. Gebruik de template in `.github/PULL_REQUEST_TEMPLATE.md` en vul in wat je hebt gewijzigd, hoe je het hebt getest en welk Taiga-ticket erbij hoort.

### GitHub-rulesets

De bestaande rulesets op `main` en `develop` zijn actief. Ze vereisen:

- een Pull Request met **minimaal één goedkeuring**;
- opnieuw beoordelen na nieuwe commits;
- het oplossen van reviewgesprekken;
- geen force-push en geen verwijdering van de beschermde branches.

Voor `develop` is **Squash and merge** toegestaan; voor `main` **Create a merge commit**. Verplaats goedgekeurde code via `develop` naar `main`.

**Er zijn nog geen verplichte CI-statuschecks ingesteld.** Die worden pas ingeschakeld zodra er werkende GitHub Actions-workflows zijn.

### Eigen branch bijwerken

```bash
git fetch origin
git rebase origin/develop
```

Doe dit op je **eigen werkbranch**, niet op `main` of `develop`. Los eventuele conflicten op voordat je de Pull Request afrondt.

---

## 4. Codeafspraken

We volgen de naamgeving uit het technisch ontwerp v1.3. **Code-identifiers zijn Engels; schermteksten mogen Nederlands zijn.**

| Onderdeel | Afspraak | Voorbeeld |
|---|---|---|
| C# class, property, methode, enum | PascalCase | `AlertService`, `SchemaName` |
| C# interface | `I` + PascalCase | `IAlertService` |
| C# async-methode | Eindigt op `Async` | `ClaimAlertAsync` |
| Variabele / parameter | camelCase | `alertId` |
| React-component / bestand | PascalCase | `AlertList.tsx` |
| JSON-property | camelCase | `receivedAt` |
| Database-entiteit | Enkelvoud | `InvestigationCase` |
| SQL-tabel | Meervoud | `InvestigationCases` |

De map `Soc.Api` bevat HTTP-code. Plaats bedrijfsregels in `Soc.Business` en datatoegang in `Soc.Data`. De frontend communiceert via de API en nooit rechtstreeks met SQL Server.

`global.json` legt de .NET SDK vast; `.editorconfig` is bedoeld voor gedeelde editor- en C#-opmaakafspraken. React en TypeScript worden gecontroleerd met ESLint. Sla geen persoonlijke paden of geheime waarden op in Git.

---

## 5. Database en beveiliging

**Nog te implementeren — dit is het ontwerp, niet de huidige situatie.**

Volgens het technisch ontwerp v1.3 gebruiken we één SQL Server-database met:

- een centraal `platform`-schema voor gebruikers, klanten, scopes en rechten;
- **een apart SQL-schema per klant** voor alerts, cases en andere onderzoeksgegevens;
- `ScopeId` om toegang binnen één klant verder te beperken.

Onderzoekstabellen gebruiken **geen `TenantId` als scheidingskolom**. De backend controleert per verzoek de rol, klant en scope; de SQL-schemanaam wordt uitsluitend via vertrouwde servermetadata bepaald.

**Let op:** schema-per-klant met EF Core is een technisch aandachtspunt. Eerst moet een proof of concept met minimaal twee fictieve klanten aantonen dat schemaresolutie, modelcaching, SQL-rechten en gegevensisolatie correct werken. Voor het platform zijn EF Core-migrations voorzien; voor klantschema's gecontroleerde SQL-scripts. Deze onderdelen zijn nu nog niet aanwezig.

Andere afgesproken beveiligingsprincipes:

- geen wachtwoorden, API-sleutels of klantgegevens in commits of logs;
- autorisatie altijd in de backend, nooit uitsluitend in React;
- alleen minimale SQL-rechten per klant;
- auditregistratie en tests op ongeoorloofde toegang zodra de functies bestaan.

`appsettings.json` bevat nu alleen standaardlogging en `AllowedHosts`. Verbindingsinstellingen en geheime configuratie worden pas toegevoegd wanneer de database- en authenticatiefunctionaliteit wordt gebouwd.

---

## 6. Controleren en testen

**Backend — vanuit de hoofdmap:**

```bash
dotnet restore SOC3.slnx
dotnet build SOC3.slnx
dotnet format SOC3.slnx --verify-no-changes
dotnet test SOC3.slnx
```

`Soc.Tests` bestaat, maar bevat momenteel **geen testmethoden**. `dotnet test` kan daarom melden dat er 0 tests gevonden zijn; dat is geen bewijs dat de functionaliteit al getest is.

**Frontend — vanuit de hoofdmap:**

```bash
cd frontend
npm ci
npm run lint
npm run build
npm audit
```

`npm run build` voert TypeScript-controle en een Vite-build uit. `npm audit` kan kwetsbaarheden in ontwikkelafhankelijkheden melden. Onderzoek die meldingen; voer niet blind `npm audit fix --force` uit.

**Voor een Pull Request:** voer de relevante controles uit en vink de checklist in de PR-template eerlijk af. Voeg bij nieuwe functionaliteit passende tests toe.

---

## 7. Problemen oplossen

| Probleem | Wat je controleert |
|---|---|
| `dotnet` start niet of SDK ontbreekt | `dotnet --list-sdks` en `global.json` |
| `npm` ontbreekt | Installeer een geschikte Node.js-versie |
| `npm ci` mislukt | Kloppen `package.json` en `package-lock.json`? |
| Frontend start niet | Voer `npm ci` uit in `frontend/`; kijk naar de Vite-fout |
| Geen SOC-routes in de API | Verwacht: eigen controllers zijn nog niet gebouwd |
| Frontend bereikt de API niet | Verwacht: API-koppeling is nog niet ingericht |
| `dotnet test` meldt 0 tests | Er zijn nog geen tests toegevoegd |
| Push naar `develop` geweigerd | Gebruik een werkbranch en Pull Request; de ruleset is actief |
| Mergeconflict | Werk je branch bij vanaf `develop` en los conflicten lokaal op |

Bij een probleem: vermeld **besturingssysteem, branch, commando, exacte foutmelding en wat je al hebt geprobeerd**. Verwijder geheimen uit screenshots en logs.

---

## 8. Documentatie en vervolg

De functionele en technische uitwerking staan in het **Functioneel Ontwerp (FO)** en **Technisch Ontwerp v1.3 (TO)**. Deze bestanden zijn **nog niet in `docs/` opgenomen**. Voeg uitsluitend de actuele, voor het team vrijgegeven versies toe.

Een logische volgende technische volgorde:

1. De schema-per-klant-proof-of-concept en databaseafspraken valideren.
2. Authentificatie en toegangscontrole bouwen.
3. De eerste alert-API en bijbehorende tests maken.
4. React-schermen met de API verbinden.
5. Cases, onderzoekstijdlijn en overige user stories uitwerken.
6. GitHub Actions toevoegen; daarna pas geslaagde checks verplicht stellen.

Werk deze README bij wanneer de projectstructuur, startcommando's of afspraken veranderen. Beschrijf steeds **wat daadwerkelijk werkt** en maak onderscheid met wat nog gepland is.

---

**SOC3 — Projectgroep 3 · De Haagse Hogeschool · 2026**
