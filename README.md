# FileFolderExplorer

This project is a File and Folder Explorer application that uses .NET Core for the backend and React for the frontend. The backend uses Entity Framework Core with PostgreSQL as the database provider.

## Prerequisites

Before you begin, ensure you have the following installed on your system:

- .NET Core SDK 8.0+
- Node.js and npm
- PostgreSQL

## Getting Started

Follow these steps to set up and run the project.

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/FileFolderExplorer.git
cd FileFolderExplorer
```

### 2. Backend Setup

#### 2.1 Install EF Core
Install the EF Core tools globally if you haven't already:
```bash
dotnet tool install --global dotnet-ef
```

#### 2.2 Configure PostgreSQL Connection
Update the PostgreSQL connection string in appsettings.json located in the root of the backend project directory. Adjust the port and other parameters as needed.
```json
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FileFolderExplorerDb;Username=yourusername;Password=yourpassword"
}
```

#### 2.3 Migrate the Database
Navigate to the backend project directory and run the following commands to apply migrations and update the database:
```bash
cd FileFolderExplorer
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### 2.4 Run the Backend
Start the backend:
```bash
dotnet run
```
The backend should now be running on https://localhost:5001 by default.  

### 3. Frontend Setup

#### 3.1 Navigate to the Frontend Directory
```bash
cd ..
cd .\file-folder-explorer\
```

#### 3.2 Install Dependencies
Install the required npm packages:
```bash
npm install
```

#### 3.3 Update API URL
If your backend is running on a different port from port http://localhost:5054 (e.g. https://localhost:5001), update the API URL in the frontend configuration. Locate and modify the API_URL in apiService.ts.
```typescript
const API_URL = 'http://localhost:5054/api';
```

#### 3.4 Run the Frontend
Start the frontend development server:
```bash
npm start
```
The frontend should now be running on http://localhost:3000 by default.

### 4. Running the Application
Open your browser and navigate to http://localhost:3000 to view and interact with the FileFolderExplorer application.

Example actions you can do:
-	Display a file directory tree view to navigate within your file and folder system.
-	Display a breadcrumb trail with links to parent or grandparent folders.
-	Display a list of files and folders for selected folders.
-	Create folders that can be named.
-	Select a local file and upload it to the file directory. Only csv or a geojson files can be uploaded.
-	Select a file from the folder display and visualise its contents

## Additional Notes
- Ensure your PostgreSQL server is running and accessible with the correct username and password.
- If you encounter issues with EF Core migrations, ensure your connection string is correct and the PostgreSQL server is running.

## Troubleshooting
- Database Connection Issues: Verify the connection string in appsettings.json and ensure PostgreSQL is running on the specified port.
- Port Conflicts: If there are port conflicts, update the ports in launchSettings.json for the backend and the relevant configuration files for the frontend.
- EF Core Issues: Ensure EF Core tools are installed globally and the database is reachable.

---------------------------------------------------------

## Database Schema

The database schema is created through an EF Core migration and can be viewed in FileFolderExplorer/Repositories/FileFolderExplorerContext.cs

In Summary, there are two tables:
- Folders
  - FolderId - UUID (Primary Key)
  - Name - text (not null)
  - ParentFolderId UUID (nullable) (Foreign Key linking folders to other folder's folderId)
- Files
  - FileId - UUID (Primary Key)
  - Name - text (not null)
  - Content - bytea (not null)
  - FolderId - UUID (not null) (Foreign Key linking files to folder's folderId)

