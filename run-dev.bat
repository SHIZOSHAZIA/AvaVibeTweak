@echo off
title Running TestApp in Watch Mode
echo [DEV] Запуск проекта в режиме автоматической перезагрузки...
dotnet watch --project src/TestApp/TestApp.csproj run
