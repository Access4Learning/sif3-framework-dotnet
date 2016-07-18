@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
set NUGET=nuget.exe

set SPEC=..\..\..\Code\Sif3Specification\Sif3Specification.sln
set FRAMEWORK=..\..\..\Code\Sif3Framework\Sif3Framework.sln
set DEMOS=..\..\..\Code\Sif3FrameworkDemo\Sif3FrameworkDemo.sln
set DOCS=..\..\..\Code\Sif3Documentation\Sif3Documentation.sln

set OUTPUT=output

REM You can specify the following verbosity levels: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
set VERBOSITY=m

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist %MSBUILD% goto okMSBUILD
echo Could not find : %MSBUILD%
echo You may need to modify this script (the MSBUILD variable) to point to the correct location for your system.
pause
goto end
:okMSBUILD

if exist %NUGET% goto okNUGET
echo Could not find : %NUGET%
echo NuGet Command Line executable not found.
echo Download it from https://www.nuget.org/ and either:
echo A) put it in your path/in the same directory as this script, or
echo B) modify this script (the NUGET variable)to point to its location
pause
goto end
:okNUGET

if exist %SPEC% goto okSPEC
echo Could not find : %SPEC%
pause
goto end
:okSPEC

if exist %FRAMEWORK% goto okFRAMEWORK
echo Could not find : %FRAMEWORK%
pause
goto end
:okFRAMEWORK

if exist %DEMOS% goto okDEMOS
echo Could not find : %DEMOS%
pause
goto end
:okDEMOS

if exist %DOCS% goto okDOCS
echo Could not find : %DOCS%
pause
goto end
:okDOCS

if exist %OUTPUT% goto okOUTPUT
echo Creating folder : %OUTPUT%
mkdir %OUTPUT%
:okOUTPUT

rem ============================================================================
rem == Start executable
rem ============================================================================

title Compile demo apps

choice /c yn /m "Do you want to build the INFRASTRUCTURE and DATA MODEL projects? [Y] Yes or [N] no.   "
if errorlevel 2 goto buildFRAMEWORK

echo Rebuilding Infrastructure and Data Model projects
echo ---
%NUGET% restore %SPEC%
%MSBUILD% %SPEC% /nologo /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /v:%VERBOSITY% > %OUTPUT%\build.idm.log
echo.

:buildFRAMEWORK
echo.
choice /c yn /m "Do you want to build the FRAMEWORK projects? [Y] Yes or [N] no.   "
if errorlevel 2 goto buildDEMOS
echo Rebuilding Framework projects
echo ---
%NUGET% restore %FRAMEWORK%
%MSBUILD% %FRAMEWORK% /nologo /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /v:%VERBOSITY% > %OUTPUT%\build.framework.log
echo.

:buildDEMOS
echo.
choice /c yn /m "Do you want to build DEMONSTRATION projects? [Y] Yes or [N] no.   "
if errorlevel 2 goto buildDOCS
echo Rebuilding Demo projects
echo ---
%NUGET% restore %DEMOS%
%MSBUILD% %DEMOS% /nologo /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /v:%VERBOSITY% > %OUTPUT%\build.demos.log
echo.

:buildDOCS
echo.
choice /c yn /m "Do you want to build the framework's DOCUMENTATION? [Y] Yes or [N] no.   "
if errorlevel 2 goto end
echo.
echo Rebuilding Demo projects
echo ---
%NUGET% restore %DOCS%
%MSBUILD% %DOCS% /nologo /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /v:%VERBOSITY% > %OUTPUT%\build.docs.log
pause
:end
