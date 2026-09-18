# MathGuide — дизайн макета на .NET MAUI

Три экрана из твоего макета, переписанные на XAML: главная с баллами и прогрессом,
список разделов с «лесенкой» градиентных карточек и умный калькулятор с вводом по фото.

## Как запустить

```bash
dotnet new maui -n MathGuide
cd MathGuide
# заменить содержимое папки файлами отсюда
dotnet build -t:Run -f net9.0-android
```

Проверено под .NET 9 (`net9.0-android`, `net9.0-ios`). Для .NET 8 достаточно
поменять `TargetFrameworks` в `.csproj` — синтаксис XAML тот же.

## Что подключить в `.csproj`

Иконки лежат в `Resources/Images` как SVG — MAUI сам конвертирует их в PNG
под нужную плотность экрана. Шаблон проекта уже содержит нужную строку:

```xml
<MauiImage Include="Resources\Images\*" />
```

Если ты будешь использовать `[ObservableProperty]` и команды вместо ручных
обработчиков, добавь пакет:

```bash
dotnet add package CommunityToolkit.Mvvm
```

## Разрешения для съёмки задачи

**Android** — `Platforms/Android/AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-feature android:name="android.hardware.camera" android:required="false" />
```

**iOS** — `Platforms/iOS/Info.plist`:

```xml
<key>NSCameraUsageDescription</key>
<string>Нужна камера, чтобы сфотографировать задачу</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>Нужен доступ к галерее, чтобы выбрать фото задачи</string>
```

## Структура

```
MathGuide/
├── App.xaml / AppShell.xaml        — ресурсы и нижняя навигация (3 вкладки)
├── Resources/Styles/Colors.xaml    — палитра и градиенты из макета
├── Resources/Styles/AppStyles.xaml — карточки, плитки, поле ввода
├── Pages/HomePage.xaml             — приветствие, баллы, «Продолжить», классы 1–11
├── Pages/RulesPage.xaml            — крупные градиентные карточки разделов
├── Pages/CalculatorPage.xaml       — ввод текстом и по фото, шаги решения
├── Models/AppModels.cs             — модели данных
├── ViewModels/                     — демо-содержимое для экранов
└── Services/ISolverService.cs      — интерфейс решателя + заглушка
```

## Что осталось на твоей стороне

**Контент 1–11 класса.** Правила удобно хранить как JSON в `Resources/Raw`
и грузить через `FileSystem.OpenAppPackageFileAsync`. Структура вроде
`{ grade, section, title, latex, explanation }` даст и фильтр по классу,
и поиск.

**Формулы.** У тебя уже есть сайт с KaTeX — самый дешёвый путь перенести его
в MAUI: положить KaTeX в `Resources/Raw` и рендерить формулы в `WebView`
с локальным HTML. Альтернатива — заранее отрендерить формулы в SVG при сборке
контента, тогда `WebView` не нужен вообще и листание будет плавнее.

**Распознавание задачи с фото.** `DemoSolverService` возвращает фиктивный ответ,
чтобы интерфейс можно было гонять без сети. Под реальную работу нужны два шага:
распознать формулу (OCR для математики) и решить её (CAS). Оба удобно
спрятать за `ISolverService` — тогда экран менять не придётся.

**Нижняя панель.** Здесь стандартный `TabBar` из Shell: он корректно работает
на Android и iOS из коробки. «Плавающая» пилюля из макета через Shell
не делается — под неё нужен свой `ContentView` поверх `Grid` на каждой
странице, либо platform handler. Скажи, если нужен такой вариант — соберу.
# MathGuide
