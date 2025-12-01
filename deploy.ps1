Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Leave Management System - Deployment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Step 1: Cleaning previous build..." -ForegroundColor Yellow
if (Test-Path "./publish") {
    Remove-Item -Path "./publish" -Recurse -Force
}
Write-Host ""

Write-Host "Step 2: Building application for Release..." -ForegroundColor Yellow
dotnet publish LeaveManagement/LeaveManagement.csproj -c Release -o ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Build failed! Please check errors above." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Build Successful!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Files are ready in ./publish folder" -ForegroundColor Green
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "1. Update appsettings.Production.json with your Somee database credentials"
Write-Host "   - Database Server: YourDatabase.mssql.somee.com"
Write-Host "   - Username: YourUsername_db"
Write-Host "   - Password: YourPassword"
Write-Host "   - Database: YourDatabase_db"
Write-Host ""
Write-Host "2. Upload all files from ./publish folder to Somee /wwwroot via FTP"
Write-Host "   - FTP Host: ftp.somee.com"
Write-Host "   - Use FileZilla or any FTP client"
Write-Host ""
Write-Host "3. Configure .NET Core version in Somee control panel"
Write-Host ""
Write-Host "4. Test your application at: http://yourusername.somee.com"
Write-Host ""
Write-Host "5. Login with default credentials:"
Write-Host "   - Username: admin"
Write-Host "   - Password: password"
Write-Host ""
Write-Host "For detailed instructions, see DEPLOYMENT.md" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Read-Host "Press Enter to exit"
