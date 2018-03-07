@ECHO OFF

REM The following directory is for .NET v4.0.30319
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo UNInstalling EventLogger Service...
echo ---------------------------------------------------
InstallUtil /u %~dp0%/EventLogger.exe
echo ---------------------------------------------------
echo Done.

pause

echo Installing EventLogger Service...
echo ---------------------------------------------------
InstallUtil /i %~dp0%/EventLogger.exe
echo ---------------------------------------------------
echo Done.

pause