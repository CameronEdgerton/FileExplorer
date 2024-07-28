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

EF Core provides the ability to retrieve objects with their related subfolders, parent folder and files based on these relationships.

I've also added rules such that deleting a folder deletes all files within it (cascade), but deleting a folder with other folders is restricted. 

---------------------------------------------------------

## System Limitations

There are a few limitations of my implementation.
- The database schema does not have any specific maximum length limits imposed. In order to keep the database lean, limits on columns like file names and folder names, should be added. 
- GetFolderTree returns the entire folder tree including files. This could be quite an expensive operation given it is tied to a useEffect which is called relatively frequently in the frontend. To improve this, I could alter the query to use projection to not retrieve the file content (Select only fileId and fileName).
- By nature of EF Core's ability to retrieve related entities, I encountered circular dependencies when retrieving folders. To resolve this, I had to add a section in the Program.cs which creates a mapping using Id and Value to ensure each folder only appears once in the result:
```cs
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
```
This meant the data was not as easily readable in the frontend, so I had to introduce a parsing algorithm in order to correctly read the data. It resulted in a fair bit of unnecessary complexity. I could altered my implementation to use a tree traversal algorithm in the backend that traverses through parentIds to generate the list. Instead, this sits in the frontend which I am not entirely happy with. 
- I have included unit tests for the Controllers and Services which have good coverage but I have not included Integration / End-to-end tests purely because of the time-cost benefit. I spent a bit of time trying to set these up but had trouble with Npgsql's in memory database capabilities. I would have liked to demonstarted integration testing but it just didn't eventuate.
- Storing the file content as a byte array and then sending it up as a string caused a bit of an issue in the frontend when trying to visualize the data for .csv files. The component libraries I tried out were all expecting .csv formatted strings, so I had to reformat the string to a csv in order to process the data. I would have ideally liked to handle that backend or provided a neater helper to achieve that mapping.
- Error handling in the frontend is fairly generic and does not give specific details to the actual cause of any backend errors. I would like to improve this, potentially through custom exceptions or an exception handler that makes the exceptions more consumable by the frontend.
- The endpoints accept strings and try to convert them to Guids. There's a lot of code bloat there. Ideally I think I could have handled the Guid typing in the frontend or created a middleware to do that parsing / validation before it reached the controller.
- There is no input validation / santisation in the front end. Validation could prevent unnecessary calls to the database (for example enter a single space in the folder name field). Sanitisation would help protect against any malicious SQL injection that is currently possible through the create folder form input. 

---------------------------------------------------------

 ## Future Development plans

 I would like to address all of the points mentioned in the system limitations.

 Additional changes that could be made:
 - Improve the styling, particularly the .csv visualisation (it isn't great).
 - Add caching so that the database doesn't get accessed so much. Particular focus on caching the files.
 - Add logging.
 - Add ability to delete folders / files.
 - Could potentially look at splitting the application into a microservices architecture.
 - Add blob storage for files hosting and store file metadata in the database instead of the bytearray in order to keep the database a bit leaner.
