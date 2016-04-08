@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set EXECUTABLE_PATH=..\..\..\Code\Sif3FrameworkDemo\Sif.Framework.Demo.Setup\bin\Debug\
set EXECUTABLE=Sif.Framework.Demo.Setup.exe

set HOSTS=..\..\..\Tools\Hosts\hosts.exe
set HOST_ENVIRONMENT=Sif.Framework.EnvironmentProvider
set HOST_PROVIDER=Sif.Framework.Demo.Uk.Provider

echo EXECUTABLE=%EXECUTABLE_PATH%%EXECUTABLE%

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist %EXECUTABLE_PATH%%EXECUTABLE% goto okExec
echo Could not find : %EXECUTABLE_PATH%%EXECUTABLE%
pause
goto end
:okExec

if exist %HOSTS% goto okHOSTS
echo Could not find : %HOSTS%
pause
goto end
:okHOSTS

rem ============================================================================
rem == Start executable
rem ============================================================================

title %EXECUTABLE%
%HOSTS% add %HOST_ENVIRONMENT% %HOST_PROVIDER%
start /D %EXECUTABLE_PATH% /WAIT /B %EXECUTABLE% UK
:end
