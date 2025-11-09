# Crime Management System – ASP.NET Core Web API

Backend API for managing **crime cases**, **reports**, **evidence**, and **users** for *District Core*.  
Built as part of the **Rihal Codestacker Challenge 2025 (Backend)** using **ASP.NET Core** and **Entity Framework Core**.

---

## 🧩 Overview

This project is a **role-based crime management platform** that helps police departments and officials to:

- Register and authenticate users with different roles and clearance levels  
- Create and manage **crime cases**  
- Attach **participants** (suspects, witnesses, victims, officers) to cases  
- Collect and manage **evidence** (text + file uploads)  
- Receive **crime reports** from residents  
- Maintain **audit logs** for sensitive operations  
- Send **email notifications** for crime awareness and alerts  

The API follows a **clean layered architecture**:

```

CrimeManagementSystem/
├── Auth/                 # JWT token creation & validation
├── Controllers/          # API endpoints (Cases, Evidence, Users, Auth, etc.)
├── DTOs/                 # Request and Response Data Transfer Objects
├── Data/                 # DbContext & database configuration
├── Helper/               # Constants, policies, helpers
├── Mapping/              # AutoMapper profiles (Entity ↔ DTO)
├── Middleware/           # Global exception & logging middleware
├── Migrations/           # EF Core migrations
├── Models/               # Entity classes (User, Case, Evidence, etc.)
├── Repos/                # Generic repository + specific repositories
├── Servises/             # Business logic services
├── uploads/              # Uploaded evidence files
├── Program.cs            # Application entry point
└── appsettings*.json     # Configuration (DB, JWT, Email, etc.)

```

## ⚙️ Tech Stack

| Category       | Technology |
|----------------|-------------|
| **Language**   | C# |
| **Framework**  | ASP.NET Core Web API (.NET 8) |
| **Database**   | SQL Server |
| **ORM**        | Entity Framework Core (Code-First + Migrations) |
| **Auth**       | JWT + Role & Policy-based Authorization |
| **Email**      | SendGrid |
| **Other**      | AutoMapper, Custom Middleware, File Uploads |


## 🔐 Roles & Security

### Roles
- **Admin** – manage users, assign roles, full access  
- **Investigator** – manage assigned cases & evidence  
- **Officer** – upload evidence, suspects, witnesses, victims  
- **Citizen** – submit crime reports & subscribe to alerts  

### Clearance Policies
Some operations require higher clearance, such as:
- `InvestigatorOrAbove`
- `ClearanceHighOrAbove`


## 🧠 Core Features

### 🔸 Authentication & Authorization
- JWT-based login  
- Login via username or email  
- Password hashing 
- Role & clearance validation  

**Endpoints**
POST /api/Auth/login
POST /api/Auth/register-citizen


---

### 🔸 User Management
Admins can:
- Manage users and assign roles  
- Set or update clearance levels  

---

### 🔸 Case Management
- Create, update, view, and delete crime cases  
- Includes fields such as case number, city, type, status, authorization level, etc.  
- Link cases to participants, evidence, and reports  

**Endpoints**
GET /api/Cases
GET /api/Cases/{id}
POST /api/Cases
PUT /api/Cases/{id}
DELETE /api/Cases/{id}


---

### 🔸 Participants
- Central `Participant` model (Suspect, Victim, Witness, Officer)  
- Many-to-many relationship via `CaseParticipant`  
- Add and manage participants per case  

---

### 🔸 Crime Reports & Case Reports
- **CrimeReport:** public reports by citizens  
- **CaseReport:** internal reports for investigators/officers  

---

### 🔸 Evidence Management
- Text or file-based evidence uploads  
- Files stored under `/uploads`  
- Supports **soft** and **hard delete**  
- **Audit logs** record all actions  

**Hard Delete Example**
POST /api/Evidence/hardDelete?id={id}
Body: { "confirmation": "yes" }


### 🔸 Email Notification System
- Integrated with **SendGrid**  
- Sends alerts to subscribed citizens  
- Citizens choose city/type via `UserSubscription`  

**Test Endpoint**
POST /api/Email/send?to=example@mail.com





---

## Contributors

Mathla Salim Alwahaibi – @MathlaALw

Rehab AlNairi – @Rehabalnairi
