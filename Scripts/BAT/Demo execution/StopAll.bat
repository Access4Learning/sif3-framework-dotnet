@echo off

rem ============================================================================
rem == User defined environment variables                                     ==
rem ============================================================================


rem ============================================================================
rem == Safety checks                                                          ==
rem ============================================================================


rem ============================================================================
rem == Start executable
rem ============================================================================

echo Checking for IISExpress instances...

for /f "delims=" %%F in ('tasklist ^| find /i /c "iisexpress.exe"') do set COUNT=%%F

echo.
echo Found %COUNT% instances
echo.

if %COUNT% equ 0 goto end

echo Instance details:
tasklist /fi "imagename eq iisexpress.exe"
echo.
echo Killing all IISExpress instances...
taskkill /f /im iisexpress.exe
echo.
echo Done

:end
pause
