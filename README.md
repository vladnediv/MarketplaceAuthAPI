# 🛡️ MarketplaceAuthAPI
Authentifizierungs- und Autorisierungsservice für den Marketplace  
Cloud-ready · Clean Architecture · Mehrstufiges Rollensystem

---

## 🧩 Projektüberblick

Die **MarketplaceAuthAPI** ist der zentrale Identity- und Security-Dienst des Marketplaces.  
Sie verwaltet die Authentifizierung, Autorisierung und Nutzeridentitäten für alle weiteren Systeme und stellt sicher, dass nur verifizierte und berechtigte Anfragen Zugriff auf die Business-APIs erhalten.

Die API übernimmt:
- Registrierung & Login (E-Mail/Passwort, Google OAuth2, OTP/Telefon)
- Rollen- und Berechtigungslogik (User, Shop, Admin, SuperAdmin)
- Profilverwaltung
- Token-Verwaltung (Access & Refresh Tokens)
- Sicherheit, Validierung und Zuständigkeitsgrenzen

---

## 🏗 Architektur

Die API basiert auf einer **Clean-/Layered-Architecture**, bestehend aus:

| Schicht | Aufgabe |
|--------|---------|
| **API Layer** | Controller, Routing, Input/Output |
| **Application Layer** | Businesslogik, Services, Validierung |
| **Domain Layer** | Domänenmodelle & Geschäftsregeln |
| **Infrastructure Layer** | Datenzugriff (EF Core, Repositories) |

Weitere Architekturentscheidungen:

- **DTOs & Domainmodelle getrennt** → Sicherheit & reduzierte Exposition
- **Repository Pattern** → gekapselte Datenzugriffe
- **Dependency Injection** → lose Kopplung & Testbarkeit
- **Identity entkoppelt von Domain** → saubere Verantwortlichkeiten

---

## 🔐 Authentifizierungs- & Login-Flows

✅ E-Mail & Passwort  
✅ Google OAuth2 (automatische Registrierung, falls nicht vorhanden)  
✅ OTP (Telefonverifizierung) bei Registrierung **und** Login  
✅ Token-System mit Access & Refresh Tokens  
✅ Logout → Refresh Token wird ungültig gemacht


---

## 👥 Rollenmodell

| Rolle | Beschreibung | Erstellung |
|------|--------------|------------|
| **User** | Standard-Nutzer | Selbstregistrierung |
| **Shop** | Shop-Betreiber | Selbstregistrierung |
| **Admin** | Verwaltungsebene | Nur durch SuperAdmin |
| **SuperAdmin** | höchste Instanz | per Seeding angelegt |

---

## 📦 Domainmodelle

| Modell | Zweck |
|--------|-------|
| `ApplicationUser` | Identität auf Auth-/Security-Ebene |
| `MarketplaceUser` | Benutzerprofil inkl. Adresse, Namen, Bild |
| `MarketplaceShop` | Shopprofil (Name, Logo, mehrere Adressen) |
| `MarketplaceAdmin` | Admin-Profil, gebunden an ApplicationUser |
| `Address` | wiederverwendbares Adressmodell (User/Shop) |

---

## 📡 Controller & Verantwortlichkeiten

### ✅ AuthController
Bereits voll implementiert.  
Funktionen:

| Endpoint | Zweck |
|---------|-------|
| /check-login | Prüft E-Mail oder Telefonnummer & Existenz |
| /register-user | Registrierung eines Users |
| /register-shop | Registrierung eines Shop-Kontos |
| /register-admin | (nur SuperAdmin) für Admin-Accounts |
| /login | Login mit E-Mail/Telefon |
| /google | OAuth2 Social Login |
| /verify-otp | Verifizierung per SMS/Telefon |
| /refresh-token | Token-Erneuerung |
| /logout | Refresh Token ungültig machen |

### ✅ UserController
| Funktion | Zweck |
|----------|--------|
| GetPersonalInfo | Eigene Nutzerdaten abrufen |
| UpdatePersonalInfo | Profildaten aktualisieren |
| DeleteAccount | Account löschen (sicherheitsgeprüft) |

### 🏛 AdminController (architektonisch vorbereitet)
Grundlage für erweiterbare administrative Funktionen.  
Berechtigt nur für Admin/SuperAdmin.

### 🏬 ShopController (architektonisch vorbereitet)
Basis für spätere shop-spezifische Geschäftslogik & Profilverwaltung.

---

## 🧠 Fehlerhandling & Response-Design

Die API nutzt ein einheitliches Antwortschema über `ServiceResponse<T>`, um Businesslogik-konsistente Rückgaben zu garantieren.

| Element | Beschreibung |
|--------|--------------|
| `IsSuccess` | Ergebnisstatus |
| `Entity / Entities` | Rückgabedaten |
| `Message` | standardisierte Fehlermeldung |

Fehler werden **im Businesslayer erzeugt**, nicht im Controller → saubere Separation of Concerns.  
HTTP-Statuscodes werden korrekt genutzt (`200/401/403/404/...`).

---

## ☁️ Deployment & Cloud Readiness

- Datenbank ist bereits **extern gehostet** (MonsterASP.net, produktionsnah).
- API aktuell lokal, jedoch **deploy-ready**.
- Architektur ausgelegt für Cloud-Hosting (z. B. Azure App Service).
- Azure KeyVault & Secret-Management bereits vorgesehen.

Kein monolithischer Ansatz – die AuthAPI ist als **unabhängiger Security-Service** konzipiert.

---

## 🧭 Roadmap

| geplanter Ausbau | Beschreibung |
|------------------|--------------|
| ShopController | Konkrete Shop-Funktionen (Profil/Branding/Business-Regeln) |
| AdminController | Systemweite Verwaltungsfunktionen |
| Azure-Deployment | Cloudhosting + KeyVault-Integration |
| MFA/2FA Ausbau | Erweiterung mit zusätzlicher Security-Schicht |

---

## ✅ Fazit

Die MarketplaceAuthAPI dient als sichere, modular erweiterbare Grundlage des gesamten Marketplaces und bildet die technische Identitäts- und Rechteverwaltung für alle weiteren Services ab.  
Sie ist cloud-ready, sauber strukturiert und folgt Best Practices aus dem Enterprise-Umfeld.

---
