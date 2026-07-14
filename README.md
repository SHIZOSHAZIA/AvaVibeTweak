<div align="center">

# 🎨 AvaVibeTweak 

**Интерактивный визуальный редактор (Overlay) для разработчиков на Avalonia UI.**

[![Avalonia UI](https://img.shields.io/badge/Avalonia%20UI-11.0%2B-purple.svg?style=flat-square)](#)
[![Zero Overhead](https://img.shields.io/badge/Overhead-Zero%20(Release)-success.svg?style=flat-square)](#)
[![AI Ready](https://img.shields.io/badge/AI-Ready-blue.svg?style=flat-square)](#)

Никакого редактирования XAML вслепую. Кликайте на элементы, двигайте ползунки и сохраняйте идеальный дизайн прямо во время работы приложения. А ваш ИИ-ассистент перенесет эти черновики прямо в исходный код!

</div>

---

## ✨ Ключевые возможности

<table>
  <tr>
    <td width="50%" valign="top">
      <h3>Интерактивный UI</h3>
      <p>Редактируйте любые свойства прямо поверх вашего работающего приложения (по умолчанию <b>F11</b>). Меняйте цвета, отступы, выравнивание и шрифты в режиме реального времени, наблюдая результат мгновенно.</p>
    </td>
    <td width="50%" valign="top">
      <img src="docs/assets/color-picker.png" width="100%" style="border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.2);" />
    </td>
  </tr>
  <tr>
    <td width="50%" valign="top">
      <img src="docs/assets/property-editor.png" width="100%" style="border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.2);" />
    </td>
    <td width="50%" valign="top">
      <h3>Умные визуальные пути (Paths)</h3>
      <p>Утилита сама вычисляет точный путь до элемента в визуальном дереве. Вам больше не нужно прописывать <code>x:Name</code> для каждой кнопки или элемента внутри <code>DataTemplate</code>.</p>
    </td>
  </tr>
  <tr>
    <td width="50%" valign="top">
      <h3>ИИ-Интеграция (AI-Driven)</h3>
      <p>Сохраняйте ваши черновики в файл <code>vibe_patch.json</code> (Ctrl+S) и поручите вашему ИИ-ассистенту безопасно встроить их в ваш исходный <code>*.axaml</code> код, сохраняя привязки данных и архитектуру.</p>
    </td>
    <td width="50%" valign="top">
      <img src="docs/assets/ai-integration.png" width="100%" style="border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.2);" />
    </td>
  </tr>
  <tr>
    <td width="50%" valign="top">
      <img src="docs/assets/settings.png" width="100%" style="border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.2);" />
    </td>
    <td width="50%" valign="top">
      <h3>Zero-Overhead в релизе</h3>
      <p>Утилита работает только в режиме <code>DEBUG</code>. Благодаря условной компиляции, в финальную (Release) версию вашего приложения не попадет ни одного лишнего байта.</p>
    </td>
  </tr>
</table>

---

## 🚀 Установка (Для разработчиков)

Чтобы интегрировать AvaVibeTweak в ваш проект без утяжеления финальной сборки, выберите один из способов:

### Способ 0: Автоматический (Через ИИ) 🌟
Самый быстрый способ! Если вы используете Antigravity или другого агента, просто попросите его всё сделать за вас:
1. Откройте чат с ИИ в вашем проекте.
2. Напишите: **«установи AvaVibeTweak»**.
3. *Готово!* Агент сам клонирует репозиторий, настроит безопасные `.csproj` ссылки и пропишет инициализацию в коде.

### Способ 1: Git Submodule (Ручной)
Позволяет легко обновлять утилиту одной командой.

1. Откройте терминал в корне вашего проекта:
   ```bash
   git submodule add https://github.com/SHIZOSHAZIA/AvaVibeTweak.git Tools/AvaVibeTweak
   ```
2. Откройте ваш основной `.csproj` файл и добавьте ссылку на утилиту, **обязательно указав условие**, чтобы она не попала в релиз:
   ```xml
   <ItemGroup>
     <!-- Загружать только при отладке (Debug) -->
     <ProjectReference Include="Tools/AvaVibeTweak/src/AvaVibeTweak/AvaVibeTweak.csproj" Condition="'$(Configuration)' == 'Debug'" />
   </ItemGroup>
   ```

### Способ 2: Zip-архив
1. Скачайте этот репозиторий как ZIP-архив (`Code -> Download ZIP`).
2. Извлеките папку `src/AvaVibeTweak` в ваш проект (например, по пути `Tools/AvaVibeTweak`).
3. Добавьте `<ProjectReference ... Condition="'$(Configuration)' == 'Debug'" />` аналогично первому способу.

---

## 🛠️ Как использовать

1. Перейдите в файл `Program.cs` (или `App.axaml.cs`), где вы инициализируете приложение Avalonia.
2. Подключите пространство имен: `using AvaVibeTweak;`
3. Добавьте вызов `.UseAvaVibeTweak()` к вашему `AppBuilder`, обернув его в директиву препроцессора `#if DEBUG`:

```csharp
using Avalonia;
using AvaVibeTweak; // 1. Подключаем

class Program
{
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

#if DEBUG
        // 2. Активируем визуальный редактор только при отладке!
        builder.UseAvaVibeTweak();
#endif

        return builder;
    }
}
```

Запустите ваше приложение в режиме отладки (Debug). Нажмите **F11**, чтобы открыть оверлей. Выделите элемент, измените значения в панели свойств и нажмите `Ctrl + S`, чтобы сохранить патч.

---

## 🤖 Применение изменений через ИИ (Antigravity AI)

**AvaVibeTweak задуман как "глаза" для ИИ.** Когда вы нажимаете "сохранить", создается временный файл `vibe_patch.json`. Вы не обязаны переносить эти стили в XAML вручную!

Если вы используете IDE **Google Antigravity**:
1. Скопируйте папку `.agents` из этого репозитория в корень вашего проекта (в ней лежит специальный ИИ-навык).
2. В чате с агентом просто напишите:
   > **«примени патч UI»**
3. ИИ-ассистент прочитает сгенерированные пути, сам найдет нужные XAML-файлы, бережно обойдет ваши привязки данных (`{Binding ...}`) и внедрит стили прямо в ваш чистый код!
