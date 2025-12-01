@echo off
echo ========================================
echo Leave Management System - Deployment
echo ========================================
echo.

echo Step 1: Cleaning previous build...
if exist publish rmdir /s /q publish
echo.

echo Step 2: Building application for Release...
dotnet publish LeaveManagement/LeaveManagement.csproj -c Release -o ./publish
echo.

if %ERRORLEVEL% NEQ 0 (
    echo Build failed! Please check errors above.
    pause
    exit /b 1
)

echo ========================================
echo Build Successful!
echo ========================================
echo.
echo Files are ready in ./publish folder
echo.
echo NEXT STEPS:
echo ========================================
echo 1. Update appsettings.Production.json with your Somee database credentials
echo    - Database Server: YourDatabase.mssql.somee.com
echo    - Username: YourUsername_db
echo    - Password: YourPassword
echo    - Database: YourDatabase_db
echo.
echo 2. Upload all files from ./publish folder to Somee /wwwroot via FTP
echo    - FTP Host: ftp.somee.com
echo    - Use FileZilla or any FTP client
echo.
echo 3. Configure .NET Core version in Somee control panel
echo.
echo 4. Test your application at: http://yourusername.somee.com
echo.
echo 5. Login with default credentials:
echo    - Username: admin
echo    - Password: password
echo.
echo For detailed instructions, see DEPLOYMENT.md
echo ========================================
echo.
pause
