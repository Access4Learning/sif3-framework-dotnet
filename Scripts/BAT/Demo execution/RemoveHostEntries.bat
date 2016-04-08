@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================
set HOSTS=..\..\..\Tools\Hosts\hosts.exe
set HOST_ENTRIES=Sif.Framework.*

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist %HOSTS% goto okHOSTS
echo Could not find : %HOSTS%
pause
goto end
:okHOSTS

rem ============================================================================
rem == Start executable
rem ============================================================================

call %HOSTS% rm %HOST_ENTRIES%
:end
