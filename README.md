# 🛡️ MarketplaceAuthAPI

"Authentication & Authorization API for the Marketplace"


---

## 🧩 Project Overview

The MarketplaceAuthAPI serves as the central identity and security service of the marketplace.
It manages **authentication, authorization, and user identities** and ensures that **only verified and authorized requests** can access the business APIs.

The API's responsibilities:
- Registration and login (email/password, Google OAuth2, OTP/phone verification)
- Role and permission management (User, Shop, Admin, SuperAdmin)
- User profile management
- Token handling (access and refresh tokens)
- Security, validation, and access control boundaries

---

## 🏗 Architecture
The API is built on a layered architecture consisting of:

| Layer | Responsibility |
|------|-----------------|
| **AuthAPI** | Controllers, routing, request/response handling |
| **BLL (Business Logic Layer)** | Services, business rules, coordinates repositories |
| **DAL (Data Access Layer)** | Repositories, EF Core, persistence logic |
| **Domain** | Domain entities (C# classes used across BLL/DAL) |


Additional architectural decisions:
- DTOs and domain models are separated → **improves security and reduces exposure of internal data**
- Repository Pattern → **encapsulated and structured data access**
- Dependency Injection → **loose coupling and improved testability**

---

## 🔐 Authentication & Login Flows


✅ Email & password

✅ Google OAuth2 (automatic registration if the account does not already exist)

✅ OTP phone verification during both registration and login

✅ Token-based authentication using access & refresh tokens

✅ Logout → refresh token is invalidated


---

## 👥 Roles

| Role | Description | Created by |
|------|-------------|-------------|
| **User** | Standard user | Self-registration |
| **Shop** | Shop owner | Self-registration |
| **Admin** | Administrative role | Created only by SuperAdmin |
| **SuperAdmin** | Highest authority | Created via database seeding |

---

## 📦 Domain Models

| Model | Purpose |
|--------|---------|
| `ApplicationUser` | Identity layer for authentication and security |
| `MarketplaceUser` | User profile including address, name and profile picture |
| `MarketplaceShop` | Shop profile (name, logo and multiple addresses) |
| `MarketplaceAdmin` | Admin profile linked to an ApplicationUser |
| `Address` | Reusable address model (used by both users and shops) |

---

## 📡 Controllers & Responsibilities

### ✅ AuthController

| Endpoint | Purpose |
|----------|---------|
| <code>/check-login</code> | Checks whether the input is an email or phone number and verifies if it exists |
| <code>/register-user</code> | Registers a standard user account |
| <code>/register-shop</code> | Registers a shop owner account |
| <code>/register-admin</code> | (SuperAdmin only) creates admin accounts |
| <code>/login</code> | Login via email or phone number |
| <code>/google</code> | OAuth2 login |
| <code>/verify-otp</code> | Phone/SMS OTP verification |
| <code>/refresh-token</code> | Issues a new access & refresh token pair |
| <code>/logout</code> | Invalidates the refresh token and ends the session |

### ✅ UserController

| Endpoint | Purpose |
|----------|---------|
| <code>/GetPersonalInfo</code> | Retrieve the user's own personal data |
| <code>/UpdatePersonalInfo</code> | Update profile information |
| <code>/DeleteAccount</code> | Delete the account (secured and permission-checked) |

### 🏛 AdminController (architecturally prepared)
The controller is already secured and integrated into the role system, allowing future administrative features to be added without architectural changes.

### 🏬 ShopController (architecturally prepared)
Foundation for future shop-specific business logic and profile management.

---

## 🧠 Error Handling & Response Design

The API uses a unified response format through `ServiceResponse<T>` to ensure consistent business logic responses.

| Property | Description |
|--------|-------------|
| `IsSuccess` | Indicates whether the operation succeeded |
| `Entity / Entities` | Contains the returned data (single or collection) |
| `Message` | Standardized error or info message |

Errors are generated in the **business layer**, not in the controller — ensuring a clean separation of concerns.  
HTTP status codes are used correctly (`200 / 401 / 403 / 404 / ...`).

---

## ☁️ Deployment & Cloud Readiness

- The database is already hosted externally (MonsterASP.net, production-like environment).
- The API currently runs locally but is **fully deployment-ready**.
- Azure KeyVault and secret management are already planned.

The AuthAPI is not monolithic — it is designed as an **independent security service** within the overall platform.

---

## ✅ Conclusion

The MarketplaceAuthAPI serves as a secure and modular foundation for the entire marketplace, providing identity and access management for all other services.  
It is cloud-ready, cleanly structured, and follows best practices commonly used in modern enterprise environments.
