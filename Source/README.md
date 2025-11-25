# Source

CSharpSmith outputs live here. Target .NET Framework 4.7.2 for RimWorld 1.6, reference the game `Assembly-CSharp.dll`, `UnityEngine*.dll`, and Harmony. Emit compiled DLLs into `Assemblies/`.

Build locally (Steam default install):
```
dotnet build Source/ChildrenOfArrakis.csproj /p:RimWorldManagedDir="C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed"
```
Adjust `RimWorldManagedDir` to your install if different.
