@ECHO OFF

REM The following directory is for .NET v4.0.30319
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo Uninstalling EventLogger Service...
echo ---------------------------------------------------
InstallUtil /u EventLogger.exe
echo ---------------------------------------------------
echo Done.
pause