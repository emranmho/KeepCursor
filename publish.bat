@echo off
echo Building CursorKeep standalone executable...
echo.
dotnet publish CursorKeep/CursorKeep.csproj -r win-x64 -c Release -p:PublishSingleFile=true --self-contained true --nologo
echo.
echo Done! Your executable is at:
echo CursorKeep\bin\Release\net10.0-windows\win-x64\publish\CursorKeep.exe
echo.
pause
