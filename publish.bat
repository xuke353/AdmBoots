color B

del  .PublishFiles\*.*   /s /q

dotnet restore

dotnet build

cd AdmBoots.Api

dotnet publish -o ..\AdmBoots.Api\bin\Debug\netcoreapp3.1\

md ..\.PublishFiles

xcopy ..\AdmBoots.Api\bin\Debug\netcoreapp3.1\*.* ..\.PublishFiles\ /s /e 

echo "Successfully!!!! ^ please see the file .PublishFiles"

cmd