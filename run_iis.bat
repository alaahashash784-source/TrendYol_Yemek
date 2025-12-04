@echo off
REM ============================================
REM TrendYol Yemek - IIS Express Runner
REM ============================================
REM PowerShell command (copy/paste):
REM taskkill /F /IM iisexpress.exe 2>$null; & "C:\Program Files\IIS Express\iisexpress.exe" /path:"c:\Users\victus\OneDrive\projects\mvc_full\mvcfull" /port:8085
REM ============================================

powershell -NoProfile -Command "taskkill /F /IM iisexpress.exe 2>$null; & 'C:\Program Files\IIS Express\iisexpress.exe' /path:'c:\Users\victus\OneDrive\projects\mvc_full\mvcfull' /port:8085"
