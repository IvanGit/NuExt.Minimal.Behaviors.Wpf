# NuExt.Minimal.Behaviors.Wpf

**A minimalistic, production-ready implementation of the Attached Behaviors pattern for WPF.**

This library provides the essential building blocks for clean UI interactivity in WPF applications following the MVVM pattern. It streamlines the classic approach by offering ready-to-use behaviors like `EventToCommand` and `KeyToCommand`, and `BehaviorsTemplate` for dynamic behavior injection.

## Why Minimal.Behaviors?
*   **No unnecessary abstractions:** No separate `Trigger` or `Action` collections. One behavior does one job.
*   **`BehaviorsTemplate`:** A feature, allowing behaviors to be defined as resources and injected dynamically.
*   **Production-focused:** Solves real-world problems (event-to-command, window services, settings) with minimal, maintainable code.

## Core Principle
```xml
<!-- Instead of Trigger/Action composition, use a single, powerful behavior: -->
<minimal:Interaction.Behaviors>
    <minimal:EventToCommand EventName="Loaded" Command="{Binding LoadedCommand}"/>
    <minimal:KeyToCommand Gesture="CTRL+S" Command="{Binding SaveCommand}"/>
    <minimal:WindowPlacementService />
</minimal:Interaction.Behaviors>
```

### Installation

You can install `NuExt.Minimal.Behaviors.Wpf` via [NuGet](https://www.nuget.org/):

```sh
dotnet add package NuExt.Minimal.Behaviors.Wpf
```

Or through the Visual Studio package manager:

1. Go to `Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution...`.
2. Search for `NuExt.Minimal.Behaviors.Wpf`.
3. Click "Install".

### Source Code Package

In addition to the standard package, there is also a source code package available: [`NuExt.Minimal.Behaviors.Wpf.Sources`](https://www.nuget.org/packages/NuExt.Minimal.Behaviors.Wpf.Sources). This package allows you to embed the entire framework directly into your application, enabling easier source code exploring and debugging.

To install the source code package, use the following command:

```sh
dotnet add package NuExt.Minimal.Behaviors.Wpf.Sources
```

Or through the Visual Studio package manager:

1. Go to `Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution...`.
2. Search for `NuExt.Minimal.Behaviors.Wpf.Sources`.
3. Click "Install".

### Contributing

Contributions are welcome! Feel free to submit issues, fork the repository, and send pull requests. Your feedback and suggestions are highly appreciated.

### License

Licensed under the MIT License. See the LICENSE file for details.