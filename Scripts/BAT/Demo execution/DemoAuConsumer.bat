@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set EXECUTABLE_PATH=Code\Sif3FrameworkDemo\Sif.Framework.Demo.Au.Consumer\bin\Debug\
set EXECUTABLE=Sif.Framework.Demo.Au.Consumer.exe

echo EXECUTABLE=%EXECUTABLE_PATH%%EXECUTABLE%

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist Code goto okWorkingDir
echo Could not find 'Code\' directory (SCRIPT RUNS FROM top level of Sif3Framework-dotNet project)
pause
goto end
:okWorkingDir

if exist %EXECUTABLE_PATH%%EXECUTABLE% goto okExec
echo Could not find : %EXECUTABLE_PATH%%EXECUTABLE%
pause
goto end
:okExec

rem ============================================================================
rem == Start executable
rem ============================================================================

title %EXECUTABLE%
start /D %EXECUTABLE_PATH% /WAIT /B %EXECUTABLE% %1
:end
