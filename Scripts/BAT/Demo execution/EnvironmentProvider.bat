@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set CONFIG_ONEDRIVE=%userprofile%\OneDrive\Documents\IISExpress\config\applicationhost.config
set CONFIG_ORIGINAL=%userprofile%\Documents\IISExpress\config\applicationhost.config
set CONFIG_VS2015=%~dp0..\..\..\Code\Sif3Framework\.vs\Sif3Framework\config\applicationhost.config
set IISEXPRESS_EXE="C:\Program Files\IIS Express\iisexpress.exe"
set SITE=Sif.Framework.EnvironmentProvider

echo Configuration file search order ...
echo     %CONFIG_VS2015%
echo     %CONFIG_ONEDRIVE%
echo     %CONFIG_ORIGINAL%

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist %IISEXPRESS_EXE% goto okIIS
echo IIS Express not find : %IISEXPRESS_EXE%
pause
goto end
:okIIS

if exist "%CONFIG_VS2015%" goto okConfigVS2015
if exist "%CONFIG_ONEDRIVE%" goto okConfigOneDrive
if exist "%CONFIG_ORIGINAL%" goto okConfigOriginal
echo No configuration file found!
pause
goto end

:okConfigVS2015
set IISEXPRESS_CONFIG=%CONFIG_VS2015%
echo Using configuration file : %CONFIG_VS2015%
goto okExec

:okConfigOneDrive
set IISEXPRESS_CONFIG=%CONFIG_ONEDRIVE%
echo Using configuration file : %CONFIG_ONEDRIVE%
goto okExec

:okConfigOriginal
set IISEXPRESS_CONFIG=%CONFIG_ORIGINAL%
echo Using configuration file : %CONFIG_ORIGINAL%
goto okExec

:okExec

rem ============================================================================
rem == Start Webapp
rem ============================================================================

START "%SITE%" /WAIT /B %IISEXPRESS_EXE% /config:"%IISEXPRESS_CONFIG%" /site:%SITE% /trace:error
:end
