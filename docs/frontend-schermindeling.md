# Frontend-schermindeling SOC3

Dit document is de gedeelde basis voor de schermen en herbruikbare UI-onderdelen van de SOC3-frontend. De indeling volgt de frontendstories en de bijbehorende functionele stories op het [Taiga-projectbord](https://tree.taiga.io/project/djominique-soc-groep-3/kanban).

De frontend heeft nu een presentatieve basislayout, navigatiemenu en herbruikbare laad-, lege-, fout- en toegang-geweigerd-staten. De voorbeeldgebruiker en klantcontext zijn testgegevens. Dit document beschrijft de beoogde structuur; routering, echte autorisatie en API-integratie zijn nog niet gebouwd.

## Uitgangspunten

- De server bepaalt de rol, rechten en toegankelijke klantgegevens. De frontend gebruikt deze informatie voor navigatie en weergave, maar is geen autorisatiegrens.
- Toon alleen klanten waartoe een SOC-medewerker toegang heeft. Een klantgebruiker heeft één vaste klantcontext en kan die niet wisselen.
- Verberg interne notities en niet-klantzichtbare informatie volledig in de klantweergave.
- Gebruik dezelfde kop, navigatie, formulierfeedback, meldingsstijlen en tabelpatronen in alle schermen.
- Ondersteun toetsenbordbediening en desktop en tablet. Op smalle schermen blijft de inhoud bruikbaar; tabellen mogen horizontaal scrollen of een compacte rijweergave gebruiken.
- Toon altijd expliciete laad-, lege-, fout- en toegangsstatussen. Toon een wijziging pas als succesvol nadat de API die heeft bevestigd.
- Status is nooit alleen met kleur aangeduid; gebruik ook tekst of een ander herkenbaar label.

## Schermen en navigatie per rol

De onderstaande rollen komen uit US-22. De precieze rechten en verschillen worden door de backend bepaald. Navigatie-items zijn daarom permissiegestuurd, niet hard gekoppeld aan een rolnaam in de frontend.

De huidige React-frontend toont het SOC3-startscherm op `/` en klantenbeheer als afzonderlijk scherm op `/admin/customers`.

| Rol | Hoofdnavigatie | Belangrijkste schermen en acties |
| --- | --- | --- |
| Analist | Alerts, Cases, Account | Alertwachtrij met filters en FIFO-sortering; alertdetails met brongegevens en statusgeschiedenis; alert oppakken, triageren en afsluiten; case aanmaken vanuit een alert; cases onderzoeken, zoeken en bijwerken. |
| Senior analist | Alerts, Cases, Account | Dezelfde operationele schermen als de analist. De stories beschrijven geen afzonderlijke senioracties; toon extra acties alleen wanneer de backend ze toestaat. |
| Accountmanager | Cases van mijn klanten, Account | Overzicht van lopende cases van gekoppelde klanten. Toon geen cases buiten de toegekende klantrelaties. De precieze inhoud van dit overzicht moet nog met de opdrachtgever worden gevalideerd (US-30). |
| Klant | Cases, Account | Alleen cases van de eigen klant die door het SOC zichtbaar zijn gemaakt; klantreactie toevoegen bij een zichtbare case. Interne notities en andere interne onderzoeksinformatie zijn niet zichtbaar. |
| Beheerder | Klanten, Gebruikers en rollen, API-sleutels, Auditlog, Account | Klanten aanmaken/deactiveren; gebruikers, rollen en klanttoegang beheren; API-sleutels aanmaken/intrekken; auditlog zoeken en filteren. Deze schermen zijn uitsluitend bereikbaar met de juiste backendrechten. |

### Open punten over rollen

- US-22 noemt analist, senior analist, accountmanager, klant en beheerder. De story vermeldt dat deze rollen nog met de opdrachtgever gevalideerd moeten worden.
- US-09 beschrijft SLA-rapportage voor een SOC-manager, maar SOC-manager staat niet in de rollenlijst van US-22. Bepaal met de opdrachtgever of dit een aparte rol is of onder een bestaande rol valt voordat hiervoor navigatie of rechten worden vastgelegd.
- US-08 vraagt om een waarschuwing bij dreigende SLA-overschrijding, maar legt niet vast voor welke rollen die zichtbaar moet zijn. Laat de backend/permissies bepalen wie deze melding ontvangt.

## Schermoverzicht en voorgestelde routes

Routes zijn naamgevingsvoorstellen voor implementatie; de frontend heeft nog geen routerbibliotheek of routeafscherming.

| Scherm | Routevoorstel | Toegang en inhoud |
| --- | --- | --- |
| Inloggen | `/login` | E-mail of gebruikersnaam en wachtwoord; veilige, algemene foutmelding; laadstatus en tijdelijke blokkering. |
| 2FA instellen/verifiëren | `/login/2fa`, `/account/security/2fa` | QR-code, verificatiecode, herstelcodes en verplichte rolsetup. Geheimen en codecontrole blijven aan de backend. |
| Alerts | `/alerts` | Toegankelijke alerts, standaard FIFO, filters op klant en status. Toon klant, titel, bron, ontvangsttijd, status en resterende SLA-tijd. |
| Alertdetails | `/alerts/:alertId` | Gestructureerde en originele alert, statusgeschiedenis en toegestane acties. Leg de triage-uitkomst vast voordat de alert wordt afgesloten. |
| Case vanuit alert maken | `/cases/new?alertId=:alertId` | Titel, ernst en overige vereiste invoer; klant komt vast uit de alert en is niet wijzigbaar. |
| Cases | `/cases` | Cases binnen de actieve toegangscontext; zoeken/filteren waar toegestaan. |
| Casedetails | `/cases/:caseId` | Status, eigenaar, tijdlijn, toegestane onderzoeksnotities, observables en bewijsmateriaal. Bij afsluiten is een samenvatting met conclusie verplicht. Klantzichtbaarheid wordt per gegeven gecontroleerd; klantreacties zijn voor analisten herkenbaar als klantberichten. |
| Klanten beheren | `/admin/customers` | Klantoverzicht, aanmaken en deactiveren met bevestiging. |
| Gebruikers en rollen | `/admin/users` | Gebruikersoverzicht en formulier voor rol, klantkoppelingen en de rolgebonden 2FA-instelling. |
| API-sleutels | `/admin/api-keys` | Per klant/tool aanmaken en intrekken; nieuwe sleutel is eenmalig zichtbaar. |
| Auditlog | `/admin/audit-log` | Zoeken en filteren op gebruiker, klant, actie en periode. |
| Accountmanager-overzicht | `/account/cases` | Lopende cases van alleen de gekoppelde klanten; inhoud nader valideren. |

Na inloggen landt de gebruiker op het eerste scherm dat de backend voor die gebruiker toestaat: Alerts voor SOC-rollen, Cases voor klantgebruikers, het cases-overzicht voor accountmanagers en Klanten voor beheerders. Bij meerdere toepasselijke rollen/rechten kiest de frontend geen extra bevoegdheden; het toegestane menu en de landing moeten uit de backendcontext volgen.

### Hoofdnavigatie per rol

```text
Analist / senior analist
  Alerts -> Alertdetails -> Oppakken / triage / afsluiten
  Cases  -> Casedetails -> Onderzoeken / status wijzigen / overdragen
                           -> Case aanmaken vanuit alert

Accountmanager
  Cases van mijn klanten -> Casedetails (alleen gekoppelde klanten)

Klant
  Cases -> Casedetails (alleen klantzichtbare inhoud) -> Reageren

Beheerder
  Klanten | Gebruikers en rollen | API-sleutels | Auditlog
```

## Gedeelde layout en componenten

Indeling in `frontend/src/components/`; de gedeelde layout en schermstatussen zijn al als presentatieve componenten aangemaakt:

| Component | Verantwoordelijkheid en afspraken |
| --- | --- |
| `AppShell` | Gedeelde paginaconstructie: paginakop, navigatie, context en inhoud. Schermcomponenten leveren alleen hun eigen inhoud. |
| `PageHeader` | Paginalabel/titel, optionele toelichting, breadcrumbs en primaire acties. Een scherm heeft één duidelijke paginatitel. |
| `AppMenu` | Toont toegestane navigatie-items, actieve locatie en ingeklapte mobiele/tabletweergave. Menu open/dicht is met toetsenbord te bedienen; verberg menu-items is geen vervanging voor backendautorisatie. |
| `CustomerContext` | Toont de actieve klant. Een wisselaar is alleen beschikbaar voor SOC-medewerkers en bevat uitsluitend door de backend toegestane klanten; klantgebruikers krijgen een vaste context. |
| `UserMenu` | Toont ingelogde gebruiker en rol, plus uitloggen. Uitloggen beëindigt de sessie via de backend. |
| `FormField` | Herbruikbare label-, invoer-, hint- en validatieopmaak. Koppel labels programmatisch aan invoervelden en fouten; gebruik native HTML-invoertypen waar passend. |
| `FormActions` | Consistente opslaan/annuleren-acties, laadstatus, dubbel-submitpreventie en focusgedrag bij fouten. |
| `ConfirmDialog` | Bevestigt onomkeerbare of toegangsgevoelige acties, zoals klantdeactivatie of wijziging van klanttoegang. Benoem gevolgen en focus de dialoog correct. |
| `Notification` | Inline meldingen voor succes, informatie, waarschuwing en fout. Fouten beschrijven de volgende stap; succes verschijnt alleen na bevestigde API-opslag. |
| `StatusBadge` | Tekstuele statusweergave voor alerts, cases, klanten en SLA. Gebruik consistente labels en niet alleen kleur. |
| `DataTable` | Gedeelde kolomkoppen, sortering, rij-acties, filters/paginering waar nodig, lege status en responsieve weergave. Gebruik een semantische HTML-tabel en behoud rij-/kolomkoppen. |
| `LoadingState`, `EmptyState`, `ErrorState`, `AccessDenied` | Herbruikbare schermstatussen met begrijpelijke tekst; fout- en toegangsstatussen bieden waar mogelijk een veilige herstelroute. |

### Componenten inzetten op schermen

| Schermtype | Gedeelde onderdelen | Domeinspecifieke inhoud |
| --- | --- | --- |
| Alert- en caseoverzichten | `AppShell`, `PageHeader`, `DataTable`, `StatusBadge`, `Notification`, statuscomponenten | Filters, FIFO/SLA-kolommen, zoeken en toegestane rij-acties. |
| Details en onderzoek | `AppShell`, `PageHeader`, `StatusBadge`, `Notification`, `FormField`, `FormActions` | Alertpayload, casetijdlijn, interne notities, observables, bewijs en klantzichtbaarheid. |
| Beheeroverzichten | `AppShell`, `PageHeader`, `DataTable`, `StatusBadge`, `ConfirmDialog` | Klanten, gebruikers/rollen, API-sleutels of auditlog. |
| Inloggen en 2FA | `PageHeader` in compacte variant, `FormField`, `FormActions`, `Notification`, statuscomponenten | Inlog-, authenticator- en herstelcodeflows; beperk gevoelige foutdetails. |
| Klant- en accountmanagercases | `AppShell`, `PageHeader`, `DataTable`, `StatusBadge`, `FormField`, `Notification` | Uitsluitend door de backend vrijgegeven klantinformatie en toegestane klantreacties. |

## Formulieren, meldingen en tabellen

### Formulieren

- Elk invoerveld heeft een zichtbaar label; placeholders vervangen geen labels.
- Markeer verplichte velden en toon validatie bij het betreffende veld én een samenvatting bovenaan wanneer meerdere velden fout zijn.
- Behoud ingevoerde waarden bij herstelbare serverfouten. Laat een mislukte actie niet als geslaagd lijken.
- Toon bij verzenden een laadstatus en voorkom dubbel indienen; maak bij fout de foutmelding en het eerste ongeldige veld toegankelijk.
- Bevestig wijzigingen in rollen of klanttoegang voordat ze worden opgeslagen.
- API-sleutels zijn alleen direct na aanmaak zichtbaar; maskeer ze daarna en maak ze niet opnieuw beschikbaar in de UI.
- Behandel wachtwoorden, 2FA-codes en herstelcodes als gevoelige invoer. Toon herstelcodes alleen wanneer de backend ze uitgeeft en volgens de story eenmalig.

### Meldingen en lege/laden/error-statussen

- Gebruik inline veldfouten voor invoer, een melding bij de betreffende actie voor API-resultaten en een pagina-/sectiestatus voor laden, geen resultaten of een algemene storing.
- Waarschuwingen bij SLA en bevestigingen van risicovolle beheeracties zijn tekstueel, duidelijk en niet uitsluitend kleurgecodeerd.
- Geef bij inloggen geen informatie die bevestigt of een account bestaat.
- Bij een verlopen sessie: meld dat opnieuw inloggen nodig is en stuur naar het inlogscherm; toon geen beschermde gegevens achter de melding.
- Bij een verboden route: toon een toegankelijke toegang-geweigerd weergave. De API blijft bij elke aanvraag autoriseren.

### Tabellen

- Toon betekenisvolle kolomkoppen en gebruik een stabiele sortering. De alertwachtrij start op oudste eerst (FIFO).
- Zet filters dicht bij de tabel en behoud filters tijdens navigatie terug waar dat nuttig is.
- Elke rij met een detailpagina heeft een herkenbare, toetsenbordtoegankelijke link; zet acties niet uitsluitend op hover.
- Bied een duidelijke melding wanneer er geen resultaten zijn en onderscheid dit van een fout of nog lopende laadactie.
- Verberg kolommen met interne informatie voor klantgebruikers; baseer de dataset en zichtbaarheid op de serverautorisatie.
- Maak complexe/ruime tabellen op kleine schermen horizontaal scrollbaar zonder bediening of context te verliezen.

## Koppeling met Taiga-stories

| Stories | Schermen/componenten die ze ondersteunen |
| --- | --- |
| US-31, US-21, US-22, US-23 | `AppShell`, klantcontext, rolgebaseerd menu, route-/toegangsfout, laden/leeg/fout en sessieverloop. |
| US-32, US-24, US-33, US-25 | Inloggen/uitloggen, 2FA instellen en verifiëren, herstelcodes, formulieren en veilige meldingen. |
| US-04 t/m US-08 | Alertoverzicht, FIFO-tabel, filters, alertdetails, oppakken, triage en SLA-melding. US-01/US-02 zijn ingest/opslag en vragen geen zelfstandig gebruikersscherm. |
| US-10 t/m US-18 | Cases, detail/tijdlijn, case aanmaken, status/eigenaar, zoeken, notities, observables en bewijs. |
| US-19 | Playbooks zijn expliciet "Won't have" in deze fase en horen niet in het eerste menu of de schermscope. |
| US-03, US-34, US-20 | API-sleutels en klantenbeheer via de beheeronderdelen, inclusief eenmalige sleutelweergave en bevestiging bij deactiveren. |
| US-35, US-22, US-23 | Gebruikersoverzicht en gebruikersformulier voor rollen en klantkoppelingen. |
| US-26, US-27 | Auditregistratie in de backend en beheerder-scherm voor zoeken/filteren van auditregels. |
| US-28, US-29 | Klant-cases en klantreactie; toon alleen klantzichtbare informatie. |
| US-30 | Accountmanager-overzicht met cases van gekoppelde klanten; inhoud nog te valideren. |
| US-09 | SLA-rapportage is een functionele story, maar de precieze roltoewijzing moet nog worden afgestemd (zie open punten). |

## Implementatiegrenzen

- De huidige frontend heeft nog geen routerbibliotheek, API-koppeling of authenticatie. De routekeuze voor het startscherm en klantenbeheer is voorlopig minimaal en niet afgeschermd; kies en implementeer routering en autorisatie afzonderlijk. Dit document schrijft geen dependency voor.
- Houd gedeelde presentatielogica in `components/` en domein-/schermlogica bij de featuremap (`features/alerts`, `features/cases`, `features/auth`); voeg voor beheer nieuwe featuremappen toe wanneer die schermen worden gebouwd.
- De frontend mag klant- of rolcontext niet zelf kiezen buiten opties die de backend teruggeeft en mag nooit een SQL-schema selecteren.
- Behandel routeafscherming en het verbergen van acties als UX. De backend moet toegang altijd zelfstandig afdwingen.