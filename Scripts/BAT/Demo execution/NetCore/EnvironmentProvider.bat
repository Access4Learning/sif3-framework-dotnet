@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set EXECUTABLE_PATH=..\..\..\..\Code\Sif3Framework\Sif.Framework.AspNetCore.EnvironmentProvider
set EXECUTABLE=Sif.Framework.AspNetCore.EnvironmentProvider.csproj
set FULL_PATH=%EXECUTABLE_PATH%\%EXECUTABLE%

echo Running %FULL_PATH%

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist %FULL_PATH% goto okExec
echo Could not find %FULL_PATH%
pause
goto end
:okExec

rem ============================================================================
rem == Start executable
rem ============================================================================

title %EXECUTABLE%
cd %EXECUTABLE_PATH%
dotnet run %EXECUTABLE%
:end
