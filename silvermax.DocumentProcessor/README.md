# Document Processor

A .NET console application built for [The C# Academy's Document Processor project](https://thecsharpacademy.com/project/20/document-processor). It imports contact data from Excel or CSV files stored in Azure Blob Storage, persists it to a SQL Server database, displays it in the console, and exports it back out as PDF and CSV reports, which are themselves uploaded to Blob Storage.

## Features

- **Dual-format import** — choose Excel (`.xlsx`) or CSV at runtime; both are implemented behind the same `IImportService` interface via keyed dependency injection.
- **Cloud-hosted source files** — the chosen source file is downloaded from Azure Blob Storage before each import, so updating the file in the cloud (adding a new contact row) is picked up on the next run.
- **Incremental seeding** — contacts are matched by phone number; only genuinely new contacts are inserted, so re-running the app never creates duplicates or re-imports contacts that already exist.
- **Console display** — all contacts are printed in a formatted table via Spectre.Console.
- **Dual-format export** — generates both a PDF (QuestPDF) and a CSV (CsvHelper) report of all contacts.
- **Cloud report archival** — both generated reports are automatically uploaded to Azure Blob Storage after generation.
- **Error handling** — missing files, corrupted/invalid file formats, and locked output files are caught and reported with clear messages rather than crashing with a raw stack trace.

## Tech stack

| Purpose | Library |
|---|---|
| Runtime | .NET 10 |
| Database / ORM | Entity Framework Core 10 + SQL Server |
| Excel parsing | ClosedXML |
| CSV read/write | CsvHelper |
| PDF generation | QuestPDF (Community license) |
| Console UI | Spectre.Console |
| Cloud storage | Azure.Storage.Blobs |

## Prerequisites

- .NET 10 SDK
- A SQL Server instance (local or remote)
- An Azure Storage account with a Blob container

## Setup

### 1. Clone and restore
```
git clone https://github.com/SylvesterNdwata/CodeReviews.Console.DocumentProcessor.git
cd silvermax.DocumentProcessor
dotnet restore
```

### 2. Configure the database connection
Edit `appsettings.json` and set your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Contacts;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply EF Core migrations
```
dotnet ef database update
```

### 4. Configure Azure Blob Storage (via .NET User Secrets — do **not** put these in `appsettings.json`)
```
dotnet user-secrets set "ConnectionStrings:AzureBlobStorage" "<your storage account connection string>"
dotnet user-secrets set "Azure:containerName" "<your container name>"
```
Your storage account connection string is available in the Azure Portal under your Storage Account → **Access keys**. Note that Azure container names must be lowercase.

### 5. Prepare the Blob container
In your container, upload two source files named exactly:
- `Contacts.xlsx`
- `Contacts.csv`

Each should have the columns `Name`, `Email`, `PhoneNumber` (header row required; column order doesn't matter, but names are matched by header).

### 6. Environment variable for local secrets to load
`Properties/launchSettings.json` is already included and sets `DOTNET_ENVIRONMENT=Development` for the default launch profile, this is required for .NET User Secrets to load at runtime. If you're running via `dotnet run` outside of that profile, set it manually:
```
set DOTNET_ENVIRONMENT=Development   # Windows cmd
$env:DOTNET_ENVIRONMENT="Development" # PowerShell
```

## Running

```
dotnet run
```
or run/debug from Visual Studio (F5 / Ctrl+F5).

On startup, the app will:
1. Prompt you to choose **Excel** or **CSV** as the import source.
2. Download the corresponding file from Blob Storage.
3. Insert any contacts not already in the database (matched by phone number).
4. Print all contacts in a formatted console table.
5. Generate `ContactReport.pdf` and `ContactReport.csv`.
6. Upload both reports to Blob Storage.

## Project structure

```
├── Contact.cs                  # Core entity
├── DbAcess/                    # EF Core DbContext + design-time factory
├── Dtos/                       # Output-shaping DTOs
├── Migrations/                 # EF Core migrations
├── Services/
│   ├── ExcelImportService.cs   # IImportService — Excel source
│   ├── CsvImportService.cs     # IImportService — CSV source
│   ├── SeedDatabaseService.cs  # Orchestrates download + incremental import
│   ├── ExportPDFService.cs     # PDF report generation
│   ├── ExportCSVService.cs     # CSV report generation
│   ├── ContactReportDocument.cs# QuestPDF document layout
│   └── BlobStorageService.cs   # Azure Blob upload/download
├── UserInterface.cs            # Orchestrates the full run: seed → display → export → upload
└── Program.cs                  # DI container setup and entry point
```
