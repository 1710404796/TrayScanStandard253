@echo off
REM 启动第一个应用
start "" "vmapi/LinxUniverse.VM.Webapi.exe"

REM 等待10秒
timeout /t 10 /nobreak >nul

REM 启动第二个应用
start "" "TrayScanStandard.exe"

exit