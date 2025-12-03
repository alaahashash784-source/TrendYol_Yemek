@echo off
taskkill /F /IM iisexpress.exe 2>nul
timeout /t 2 /nobreak >nul
"C:\Program Files\IIS Express\iisexpress.exe" /path:"c:\Users\victus\OneDrive\projects\mvc_full\mvcfull" /port:8085
