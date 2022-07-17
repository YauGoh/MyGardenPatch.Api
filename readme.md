 Database Migrations

## My Garden Patch Database

To Add a new Migration:

```
dotnet ef migrations add <name_of_migration> `
                         -s .\src\MyGardenPatch.Webapi\ `
                         -p .\src\MyGardenPatch.SqlServer\ `
                         --context MyGardenPatch.SqlServer.MyGardenPatchDbContext
```

To Update the Database:
``` 
dotnet ef database update -s .\src\MyGardenPatch.Webapi\ `
                          -p .\src\MyGardenPatch.SqlServer\ `
                          --context MyGardenPatch.SqlServer.MyGardenPatchDbContext
```

## Local Identity Database

To Add a new Migration:

```
dotnet ef migrations add <name_of_migration> `
                         -s .\src\MyGardenPatch.Webapi\ `
                         -p .\src\MyGardenPatch.LocalIdentity\ `
                         --context MyGardenPatch.LocalIdentity.LocalIdentityDbContext
```

To Update the Database:
``` 
dotnet ef database update -s .\src\MyGardenPatch.Webapi\ `
                          -p .\src\MyGardenPatch.LocalIdentity\ `
                          --context MyGardenPatch.LocalIdentity.LocalIdentityDbContext
```
