@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set EXECUTABLE_PATH=..\..\..\..\Code\Sif3FrameworkDemo\Sif.Framework.Demo.Consumer\bin\Debug\net6.0
set EXECUTABLE=Sif.Framework.Demo.Consumer.dll
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
dotnet %EXECUTABLE%
:end
