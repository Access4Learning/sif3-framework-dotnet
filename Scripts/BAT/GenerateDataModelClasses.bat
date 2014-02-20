@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set EXECUTABLE="C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe"

echo EXECUTABLE=%EXECUTABLE%

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist %EXECUTABLE% goto okExec
echo Could not find : %EXECUTABLE%
pause
goto end
:okExec

rem ============================================================================
rem == Start executable
rem ============================================================================

title "Generate Data Model classes"
cmd /C %EXECUTABLE% /parameters:.\DataModelTypes.xml
del /Q output\Au1_3DataModelTypes.cs
rename output\SIF_Message.cs Au1_3DataModelTypes.cs
pause
:end
