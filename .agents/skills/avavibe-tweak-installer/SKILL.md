---
name: avavibe-tweak-installer
description: Автоматически скачивает и интегрирует утилиту AvaVibeTweak в Avalonia-проект. Активируется по команде "установи AvaVibeTweak".
---

# AvaVibeTweak Installer Skill

Твоя задача — безопасно, правильно и без ошибок установить визуальный редактор `AvaVibeTweak` в C#/.NET проект пользователя (Avalonia UI). Ты должен строго следовать этому алгоритму, чтобы не сломать архитектуру и не добавить лишнего мусора в финальный (Release) билд.

## Алгоритм установки (Строго следовать!)

### Шаг 1. Клонирование репозитория (Git Submodule)
AvaVibeTweak должен лежать в отдельной изолированной папке.
- Выполни команду в терминале (в корне проекта пользователя):
  `git submodule add https://github.com/SHIZOSHAZIA/AvaVibeTweak.git Tools/AvaVibeTweak`
- *(Если репозиторий пользователя еще не инициализирован в git, сначала выполни `git init`)*.

### Шаг 2. Интеграция в `.csproj` (Защита финального билда)
Найди главный файл проекта пользователя (например, `MyApp.csproj`).
- **ЗАПРЕЩЕНО** добавлять стандартный `ProjectReference`.
- **ОБЯЗАТЕЛЬНО** добавь ссылку с условием конфигурации Debug, чтобы утилита не попала к конечным пользователям:
```xml
  <ItemGroup>
    <ProjectReference Include="Tools\AvaVibeTweak\src\AvaVibeTweak\AvaVibeTweak.csproj" Condition="'$(Configuration)' == 'Debug'" />
  </ItemGroup>
```

### Шаг 3. Инициализация в коде (`Program.cs` или `App.axaml.cs`)
Найди файл, где происходит настройка `AppBuilder` (обычно метод `BuildAvaloniaApp`).
- В начале файла добавь:
  `using AvaVibeTweak;`
- Внутри метода `BuildAvaloniaApp`, перед `return builder;`, добавь вызов утилиты, **ОБЯЗАТЕЛЬНО** обернув его в директиву препроцессора `#if DEBUG`:
```csharp
#if DEBUG
        builder.UseAvaVibeTweak();
#endif
```

### Шаг 4. Установка ИИ-Навыка интегратора
Чтобы пользователь мог автоматически применять визуальные патчи к своему XAML, ему нужен навык `avavibe-tweak-integrator`.
- Скопируй папку навыков из клонированного модуля в корень проекта пользователя:
  Для Windows PowerShell: `Copy-Item -Path "Tools\AvaVibeTweak\.agents" -Destination "." -Recurse -Force`

### Шаг 5. Завершение
- Выполни сборку `dotnet build`, чтобы убедиться, что проект компилируется.
- Сообщи пользователю: *"AvaVibeTweak успешно установлен! Запустите проект в режиме отладки (Debug) и нажмите F11 для открытия оверлея. Когда сохраните изменения, просто напишите мне `примени патч UI`."*
