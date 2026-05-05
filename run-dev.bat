@echo off
setlocal enabledelayedexpansion

set ROOT=%~dp0
set FRONTEND=%ROOT%frontend
set API=%ROOT%src\TaskPlanner.Api
set WWWROOT=%API%\wwwroot

echo.
echo === TaskPlanner development runner ===
echo.

echo Restoring backend packages...
dotnet restore "%ROOT%TaskPlanner.slnx"
if errorlevel 1 exit /b 1

echo.
echo Installing frontend packages...
pushd "%FRONTEND%"
call npm install
if errorlevel 1 exit /b 1

echo.
echo Building frontend...
call npm run build
if errorlevel 1 exit /b 1
popd

echo.
echo Copying frontend build into API wwwroot...
if exist "%WWWROOT%" (
  rmdir /s /q "%WWWROOT%"
)
mkdir "%WWWROOT%"
xcopy "%FRONTEND%\dist\*" "%WWWROOT%\" /E /I /Y > nul
if errorlevel 1 exit /b 1

echo.
echo Building backend...
dotnet build "%ROOT%TaskPlanner.slnx"
if errorlevel 1 exit /b 1

echo.
echo Running backend tests...
dotnet test "%ROOT%TaskPlanner.slnx" --no-build
if errorlevel 1 exit /b 1

if "%~1"=="--no-run" (
  echo.
  echo Build, tests, and static asset copy completed.
  exit /b 0
)

echo.
echo Starting TaskPlanner API and frontend at the API endpoint...
dotnet run --project "%API%\TaskPlanner.Api.csproj"
