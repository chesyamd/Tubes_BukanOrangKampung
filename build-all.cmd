@echo off
echo === Build main bot ===
for /R src\main-bot %%f in (*.csproj) do (
  echo Building %%f
  dotnet build "%%f"
  if errorlevel 1 exit /b 1
)

echo === Build alternative bots ===
for /R src\alternative-bots %%f in (*.csproj) do (
  echo Building %%f
  dotnet build "%%f"
  if errorlevel 1 exit /b 1
)

echo All bots built successfully.
pause
