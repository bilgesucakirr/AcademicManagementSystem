# 🧠 JARVIS - Backend Microservices

This repository contains the server-side microservices for the **Jarvis Academic Management System**. Built with **.NET 8** and **Entity Framework Core**, it facilitates the complete academic publishing workflow including user identity, submission handling, peer reviews, and venue management.

## 🏗️ Architecture

The system is composed of 4 independent microservices:

| Service | Description | Port (HTTPS) |
| :--- | :--- | :--- |
| **Identity.Api** | Manages User Auth (JWT), Profiles, Photos, and Expertise Areas. | `7041` |
| **Submission.Api** | Handles manuscript uploads, revision workflows, and file validation. | `7004` |
| **Review.Api** | Manages reviewer assignments, scoring, and blind review logic. | `7047` |
| **Venue.Api** | Defines Conferences, Journals, Tracks, and Call for Papers (CFP). | `7190` |

---

## ✨ Key Features & Business Logic

### 🔐 Identity Service
*   **Extended User Profile:** Stores academic details including `Affiliation`, `Country`, `Biography`, and `Title`.
*   **Profile Pictures:** Handles secure upload and retrieval of user avatars via `FileService`.
*   **Standardized Interests:** Implements an Admin-managed `AreaOfInterest` table to standardize expertise selection (preventing free-text chaos).

### 📄 Submission Service
*   **Strict File Validation:** Enforces **.docx (Word)** format for all manuscript uploads to ensure review consistency.
*   **New Submission Types:** Added support for `Abstract Paper` type.
*   **Camera Ready Workflow:** Implemented a specific state flow (`CameraReadyRequested` -> `CameraReadySubmitted`) for accepted papers.
*   **Review Integration:** Logic to fetch anonymous reviewer comments for authors.

### 👁️ Review Service
*   **Advanced Decision Support:** Reviewers can submit specific recommendations:
    *   *Accept*
    *   *Minor Revision*
    *   *Major Revision*
    *   *Reject*
*   **Blind Review Enforcement:** Ensures reviewer identities are never exposed in the API responses sent to authors.
*   **Workflow:** Manages the lifecycle from `Invited` -> `Accepted` -> `Submitted`.

### 🏛️ Venue Service
*   **Dynamic Configuration:** Manages multiple editions (years) and tracks for Conferences and Journals.

---

## 🛠️ Getting Started

### Prerequisites
*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   **SQL Server** (LocalDB or SQL Express)

### 1. Database Setup (Migrations)

Before running the services, you must apply the Entity Framework Core migrations to create/update the databases.

Run these commands in your **terminal** from the solution root:

```bash
# 1. Identity DB (Users, Interests, Photos)
dotnet ef database update --project Identity.Api

# 2. Submission DB (Papers, Files)
dotnet ef database update --project Submission.Infrastructure --startup-project Submission.Api

# 3. Review DB (Assignments, Scores)
dotnet ef database update --project Review.Infrastructure --startup-project Review.Api

# 4. Venue DB (Conferences, Tracks)
dotnet ef database update --project Venue.Infrastructure --startup-project Venue.Api


### 2. Seeding Data (Optional but Recommended)
To populate the Areas of Interest list (e.g., AI, Machine Learning, Security), execute the SQL script located in Identity.Api or use the provided seed migration.

### 3. Running the Services
You need to run all 4 services simultaneously for the system to function correctly.
Option A: Visual Studio
Right-click the Solution in Solution Explorer.
Select Set Startup Projects.
Choose Multiple startup projects.
Set Identity.Api, Submission.Api, Review.Api, and Venue.Api to Start.
Press F5.
Option B: CLI
Open 4 separate terminal windows and run:
dotnet run --project Identity.Api
dotnet run --project Submission.Api
dotnet run --project Review.Api
dotnet run --project Venue.Api
```

## 🔌 API Documentation (Swagger)

Once running, you can explore and test the APIs via Swagger UI:

*   **Identity API:** [https://localhost:7041/swagger](https://localhost:7041/swagger)
*   **Submission API:** [https://localhost:7004/swagger](https://localhost:7004/swagger)
*   **Review API:** [https://localhost:7047/swagger](https://localhost:7047/swagger)
*   **Venue API:** [https://localhost:7190/swagger](https://localhost:7190/swagger)

## 📦 Tech Stack

*   **Framework:** .NET 8
*   **ORM:** Entity Framework Core (Code-First)
*   **Database:** MSSQL
*   **Communication:** HTTP (REST)
*   **Patterns:** CQRS (via MediatR), Repository Pattern
*   **Validation:** FluentValidation
