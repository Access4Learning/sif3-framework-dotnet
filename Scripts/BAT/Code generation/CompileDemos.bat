@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================

set MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
set NUGET=..\..\..\Tools\NuGet\nuget.exe

set SPEC=..\..\..\Code\Sif3Specification\Sif3Specification.sln
set FRAMEWORK=..\..\..\Code\Sif3Framework\Sif3Framework.sln
set DEMOS=..\..\..\Code\Sif3FrameworkDemo\Sif3FrameworkDemo.sln

set OUTPUT=output

REM You can specify the following verbosity levels: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
set VERBOSITY=m

rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================

if exist %MSBUILD% goto okMSBUILD
echo Could not find : %MSBUILD%
pause
goto end
:okMSBUILD

if exist %NUGET% goto okNUGET
echo Could not find : %NUGET%
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

if exist %OUTPUT% goto okOUTPUT
echo Creating folder : %OUTPUT%
mkdir %OUTPUT%
:okOUTPUT

rem ============================================================================
rem == Start executable
rem ============================================================================

title Compile demo apps
echo Rebuilding Infrastructure and Data Model projects
echo ---
%NUGET% restore %SPEC%
%MSBUILD% %SPEC% /nologo /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /v:%VERBOSITY% > %OUTPUT%\build.idm.log
echo.
echo.
echo Rebuilding Framework projects
echo ---
%NUGET% restore %FRAMEWORK%
%MSBUILD% %FRAMEWORK% /nologo /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /v:%VERBOSITY% > %OUTPUT%\build.framework.log
echo.
echo.
echo Rebuilding Demo projects
echo ---
%NUGET% restore %DEMOS%
%MSBUILD% %DEMOS% /nologo /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /v:%VERBOSITY% > %OUTPUT%\build.demos.log
pause
:end
