git pull


@echo off
for /f "tokens=5" %%i in ('netstat -aon ^| findstr ":8082"') do (
    set n=%%i
)
taskkill /f /pid %n%




dotnet build

cd AdmBoots.Api



dotnet run

cmd