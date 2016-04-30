@echo off

set APPCMD=%systemroot%\system32\inetsrv\AppCmd.exe
set WEBAPP=..\..\..\Code\Sif3Framework\.vs\config\applicationhost.config
set DIR=..\..\..\Code\Sif3Framework\Sif.Framework.EnvironmentProvider
set DIRX=..\..\..\Code\Sif3Framework\.vs\config

if exist "%~dp0%WEBAPP%" goto okWEBAPP
echo Could not find : %WEBAPP%
pause
goto end
:okWEBAPP

REM %APPCMD% list config /apphostconfig "%~dp0%WEBAPP%"
REM %APPCMD% list config %DIR%
REM %APPCMD% list config "%~dp0%DIRX%"
REM start /D "%~dp0%DIRX%" /WAIT /B %APPCMD% add vdir /app.name:"Sif.Framework.EnvironmentProvider/" /path:/ /physicalPath:"%~dp0%DIR%"
REM %APPCMD% add vdir /app.name:"Sif.Framework.EnvironmentProvider/" /path:/ /physicalPath:"%~dp0%DIR%" /apphostconfig "%~dp0%WEBAPP%"
REM %APPCMD% set config -section:system.applicationHost/sites /+"[name='Sif.Framework.EnvironmentProvider'].[path='/'].[path='/',physicalPath='%~dp0%DIR%']" /commit:"%~dp0%WEBAPP%"
REM %APPCMD% list config /commit:"%~dp0%DIRX%"
REM %APPCMD% list vdirs /text:physicalPath

REM start /D "%~dp0%DIRX%" /WAIT /B %APPCMD% add vdir /app.name:"Sif.Framework.EnvironmentProvider/" /path:/ /physicalPath:"%~dp0%DIR%" /commit:apphost
REM start /D "%~dp0%DIRX%" /WAIT /B %APPCMD% set config -section:system.applicationHost/sites /+"[name='Sif.Framework.EnvironmentProvider'].[path='/'].[path='/',physicalPath='%~dp0%DIR%']" /commit:apphost

start /D "%~dp0%DIRX%" /WAIT /B %APPCMD% set app /app.name:Sif.Framework.EnvironmentProvider/[path='/'].physicalPath:"%~dp0%DIR%"
REM CD "%~dp0%DIRX%"
REM %APPCMD% add vdir /app.name:"Sif.Framework.EnvironmentProvider/" /path:/ /physicalPath:"%~dp0%DIR%" /commit:apphost

REM %APPCMD% list vdir /apphostconfig:applicationhost.config

REM set MSDEPLOY="C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\MSDeploy.exe"

pause