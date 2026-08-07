<div align="center">

# 🎛 Satur8

VST3-плагин на C# + WPF для гармонического насыщения и динамической обработки аудиосигнала.  
Реализует **нелинейные модели клиппинга** (Soft, Hard, Tube/Tanh, Arctan) с раздельным управлением чётными и нечётными гармониками, встроенным компрессором и облачным хранением пресетов через PostgreSQL.

<br>

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-UI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![VST3](https://img.shields.io/badge/VST3-AudioPlugSharp-FF6B35?style=for-the-badge)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-ORM-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Ableton](https://img.shields.io/badge/Ableton_Live-Compatible-000000?style=for-the-badge)

</div>

---

## 🛠 Стек технологий

<div align="center">

![C#](https://skillicons.dev/icons?i=cs)
![.NET](https://skillicons.dev/icons?i=dotnet)
![PostgreSQL](https://skillicons.dev/icons?i=postgresql)
![Visual Studio](https://skillicons.dev/icons?i=visualstudio)
![Ableton](https://skillicons.dev/icons?i=ableton)
![Git](https://skillicons.dev/icons?i=git)
![GitHub](https://skillicons.dev/icons?i=github)

</div>

<br>

## 🏗 Архитектура

Проект построен на принципах **Clean Architecture** с разделением на слои:

- **Domain** — сущности (`User`, `Preset`, `Category`, `Favourite`) без внешних зависимостей
- **Application** — интерфейсы сервисов, DTO, контракты
- **Persistence** — EF Core, PostgreSQL, `AuthService`, `PresetService`, миграции
- **Maths / Processors** — DSP-ядро: `Clipper`, `HarmonicSaturator`, `DynamicsProcessor`
- **UI** — WPF UserControl, ViewModel (MVVM), конвертеры, ResourceDictionary с темой
- **UI.VST** — точка входа VST3: `SaturatorPlugin`, `PluginEntryPoint`, интеграция с AudioPlugSharp

<br>

## 🚀 Возможности

- 🎚 **Гармоническое насыщение** — раздельное управление чётными и нечётными гармониками через параметры Even / Odd
- 🔊 **4 модели клиппинга** — Soft (кубический), Hard, Tube (Tanh), Arctan с автоматической нормировкой уровня
- ⚡ **Встроенный компрессор** — пиковый детектор огибающей с настраиваемыми порогом, атакой и релизом
- 🎛 **Параллельный Dry/Wet mix** — плавное смешивание исходного и обработанного сигнала
- 💾 **Облачные пресеты** — сохранение, загрузка и категоризация пресетов через PostgreSQL
- ⭐ **Избранные пресеты** — персональный список с фильтрацией
- 🔐 **Аутентификация** — регистрация и вход с SHA-256 хешированием паролей
- 🎨 **Кастомный WPF-интерфейс** — светлая тема, крутилки (`AudioPlugSharpWPF.Dial`), модальные overlay-панели
- 🔌 **VST3-совместимость** — работает в Ableton Live 11 и других DAW с поддержкой VST3

<br>

## 📐 DSP-модели

| Параметр | Диапазон | Описание |
|---|---|---|
| Drive | 0.5 — 5.0 | Усиление перед клиппером |
| Even | 0 — 1 | Уровень чётных гармоник |
| Odd | 0 — 1 | Уровень нечётных гармоник |
| Bias | −0.5 — 0.5 | DC-смещение для генерации чётных гармоник |
| Output | −20 — +20 dB | Выходное усиление |
| Mix | 0 — 100% | Dry/Wet баланс |
| Threshold | −30 — 0 dB | Порог компрессора |
| Clip Type | Soft / Hard / Tube / Arctan | Модель нелинейного насыщения |

**Разложение на гармоники:**
```
even(x) = (f(x) + f(−x)) / 2
odd(x)  = (f(x) − f(−x)) / 2
y = (even · EvenAmount + odd · OddAmount) · norm · OutputGain
```

<br>

## 🔧 Быстрый старт

```bash
git clone https://github.com/Reyme87/Satur8.git
cd Satur8
```

Настройте строку подключения в `Satur8.UI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=satur8;Username=postgres;Password=yourpassword"
  }
}
```

Примените миграции:

```bash
dotnet ef database update --project Satur8.Persistence --startup-project Satur8.Console
```

Соберите плагин (автоматически задеплоится в папку VST3):

```bash
dotnet build Satur8.UI/Satur8.UI.csproj -c Release
```

После сборки плагин появится в:
```
C:\Program Files\Common Files\VST3\Satur8.UIBridge.vst3\
```

Откройте Ableton Live → Preferences → Plugins → Rescan. Плагин появится в разделе **Audio Effects → ReyMusic → Satur8**.

<br>

## 📄 О проекте

Учебно-практический проект, разработанный в рамках курсовой работы 3-го курса по дисциплине "Разработка информационного обеспечения". В ходе работы изучены основы VST3-разработки на .NET, цифровой обработки сигналов и Clean Architecture. Демонстрирует применение нелинейных математических моделей в реальной аудио-системе с WPF-интерфейсом и облачным хранением состояния.
