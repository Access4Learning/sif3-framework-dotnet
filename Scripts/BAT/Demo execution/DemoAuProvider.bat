@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set IISEXPRESS_EXE="C:\Program Files\IIS Express\iisexpress.exe"
set WEBAPP=..\..\..\Code\Sif3FrameworkDemo\.vs\config\applicationhost.config
set SITE=Sif.Framework.Demo.Au.Provider

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist %IISEXPRESS_EXE% goto okIIS
echo Could not find : %IISEXPRESS_EXE%
pause
goto end
:okIIS

if exist "%~dp0%WEBAPP%" goto okExec
echo Could not find : %WEBAPP%
pause
goto end
:okExec

rem ============================================================================
rem == Start Webapp
rem ============================================================================

START "%SITE%" /WAIT /B %IISEXPRESS_EXE% /config:"%~dp0%WEBAPP%" /site:%SITE%

:end
