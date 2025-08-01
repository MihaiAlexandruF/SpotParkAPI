# 🚗 SpotPark API – Smart Urban Parking Backend

SpotPark is a smart digital platform that connects drivers with available private parking spots in urban areas, optimizing space usage, saving time, and promoting sustainability. This backend project, built with **ASP.NET Core**, powers the secure, scalable, and real-time engine of the SpotPark mobile app.

---

## 🔧 Tech Stack

- **Language**: C#
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT Bearer
- **Payment & Wallet**: Internal wallet system + Stripe
- **Architecture**: Clean Architecture (Controller → Service → Repository)
- **Cloud Ready**: Containerizable and scalable

---

## 📁 Project Structure

```text
SpotParkAPI/
│
├── Controllers/           → RESTful API endpoints
├── Services/              → Business logic (interfaces + implementations)
├── Repositories/          → Data access layer
├── Models/                → Entities, DTOs, Requests, Enums
├── Helpers/               → Utility services (TimeZoneService, etc.)
├── Middlewares/           → JWT handling, global errors
├── Migrations/            → Entity Framework DB schema
├── wwwroot/               → Uploaded images (parking photos)
└── Program.cs / Startup.cs → Entry point & configuration


---

## ✅ Core Features

- 🔐 **Authentication**: JWT-secured login & registration with hashed passwords.
- 🅿️ **Parking Spot Management**: Add, edit, schedule, and upload images for parking spaces.
- 📆 **Reservation System**: Time-aware, UTC-based bookings with vehicle & payment selection.
- 💳 **Wallet System**: Drivers and owners have real-time balances, with commission logic.
- 📈 **Owner Dashboard**: View revenue, active clients, and parking usage.
- 🗺️ **Map Integration**: Return only available, active, and unreserved spots for map view.
- 🧾 **My Reservations**: Active + historical reservations with accurate timer & info.
- 📤 **Stripe Integration**: Fund your in-app wallet with real money (optional).
- 🧪 **Unit Tested Services**: Business logic tested for resilience and clarity.

---

## 🔒 Security Highlights

- JWT-based stateless authentication
- Secure password hashing
- Global error handling (with `ServiceResult`)
- Input validation on all request models
- Cross-Origin Resource Sharing (CORS) enabled

---

## 🧪 How to Run Locally

### Requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server (local or Docker)
- Visual Studio / VS Code
- Stripe test keys (optional)

### Steps
```bash
# Clone the project
git clone https://github.com/your-username/SpotParkAPI.git
cd SpotParkAPI

# Restore and build
dotnet restore
dotnet build

# Update your DB connection in appsettings.json
# Run migrations (optional)
dotnet ef database update

# Run the app
dotnet run

