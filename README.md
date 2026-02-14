# NuExt.Minimal.Behaviors.Wpf

`NuExt.Minimal.Behaviors.Wpf` is a **minimalistic, production‑ready** implementation of WPF **Attached Behaviors** for MVVM. It delivers deterministic, predictable interactivity with ready‑to‑use behaviors (`EventToCommand`, `KeyToCommand`) and template‑driven composition — **dynamic behavior injection via `BehaviorsTemplate`** and **runtime selection via `BehaviorsTemplateSelector`**.

[![NuGet](https://img.shields.io/nuget/v/NuExt.Minimal.Behaviors.Wpf.svg)](https://www.nuget.org/packages/NuExt.Minimal.Behaviors.Wpf)
[![Build](https://github.com/IvanGit/NuExt.Minimal.Behaviors.Wpf/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/IvanGit/NuExt.Minimal.Behaviors.Wpf/actions/workflows/ci.yml)
[![License](https://img.shields.io/github/license/IvanGit/NuExt.Minimal.Behaviors.Wpf?label=license)](https://github.com/IvanGit/NuExt.Minimal.Behaviors.Wpf/blob/main/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/NuExt.Minimal.Behaviors.Wpf.svg)](https://www.nuget.org/packages/NuExt.Minimal.Behaviors.Wpf)

**Package ecosystem:** The core package ships the foundational attached behavior infrastructure and essential behaviors for MVVM. For extra behaviors/services, see [`NuExt.Minimal.Mvvm.Wpf`](https://www.nuget.org/packages/NuExt.Minimal.Mvvm.Wpf).

---

## Why Minimal.Behaviors?

- **Simplicity & maintainability.** No trigger/action stacks. One behavior = one responsibility. Readable, testable, predictable.
- **Dynamic composition.** `BehaviorsTemplate` and `BehaviorsTemplateSelector` let you define and reuse behavior sets with minimal XAML.
- **Practical coverage.** Event-to-command binding, keyboard shortcuts, dynamic behavior sets — no hidden indirection.
- **Deterministic semantics.** Clear command targeting; no focus-based ambiguity.
- **Performance-focused.** Hot paths avoid allocations. No unnecessary plumbing.

### Compatibility

- **WPF (.NET 8/9/10 and .NET Framework 4.6.2+)**
- Works with any MVVM framework.
- No dependency on external behavior frameworks.

---

## Core Components
- `Interaction` – attached properties: `Behaviors`, `BehaviorsTemplate`, `BehaviorsTemplateSelector`, `BehaviorsTemplateSelectorParameter`.
- `BehaviorCollection` – observable collection managing behavior lifecycle.
- `EventToCommand`, `KeyToCommand` – ready-to-use behaviors with a predictable contract.

---

## Core Principle
```xml
<!-- Prefer concise attached behaviors over trigger/action composition -->
<minimal:Interaction.Behaviors>
  <minimal:EventToCommand EventName="Loaded" Command="{Binding LoadedCommand}" />
  <minimal:KeyToCommand Gesture="CTRL+S" Command="{Binding SaveCommand}" />
  <minimal:WindowPlacementService />
</minimal:Interaction.Behaviors>
```

---

### Quick Start
1.  **Add the namespace**:
    ```xml
    xmlns:minimal="http://schemas.nuext.minimal/xaml"
    ```
2.  **Attach a behavior**:
    ```xml
    <Button Content="Click Me">
      <minimal:Interaction.Behaviors>
        <minimal:EventToCommand EventName="Click" Command="{Binding MyCommand}" />
      </minimal:Interaction.Behaviors>
    </Button>
    ```
3.  **Define the command** in your ViewModel.

---

### Dynamic Behaviors with BehaviorsTemplate
Define once, apply many times. The template supports **two** concise formats:
- **Single behavior** via `ContentControl.Content`
- **Multiple behaviors** via `ItemsControl.Items`

#### Single:
```xml
<Window.Resources>
  <DataTemplate x:Key="SaveBehavior">
    <ContentControl>
      <minimal:KeyToCommand Gesture="CTRL+S" Command="{Binding SaveCommand}" />
    </ContentControl>
  </DataTemplate>
</Window.Resources>

<TextBox minimal:Interaction.BehaviorsTemplate="{StaticResource SaveBehavior}" />
```
#### Multiple:
```xml
<Window.Resources>
  <DataTemplate x:Key="EditBehaviors">
    <ItemsControl>
      <minimal:KeyToCommand Gesture="F2"     Command="{Binding StartEditCommand}" />
      <minimal:KeyToCommand Gesture="Ctrl+S" Command="{Binding SaveCommand}" />
      <minimal:KeyToCommand Gesture="Escape" Command="{Binding CancelCommand}" />
    </ItemsControl>
  </DataTemplate>
</Window.Resources>

<ListBox minimal:Interaction.BehaviorsTemplate="{StaticResource EditBehaviors}" />
```

---

### Dynamic Selection with BehaviorsTemplateSelector
Switch behavior sets at runtime:
```xml
<Window.Resources>
  <DataTemplate x:Key="ReadOnlyTemplate">
    <ContentControl>
      <minimal:KeyToCommand Gesture="F2" Command="{Binding StartEditCommand}" />
    </ContentControl>
  </DataTemplate>

  <DataTemplate x:Key="EditableTemplate">
    <ItemsControl>
      <minimal:KeyToCommand Gesture="Ctrl+S" Command="{Binding SaveCommand}" />
      <minimal:KeyToCommand Gesture="Escape" Command="{Binding CancelCommand}" />
    </ItemsControl>
  </DataTemplate>

  <local:MyBehaviorSelector x:Key="BehaviorSelector"
                            ReadOnlyTemplate="{StaticResource ReadOnlyTemplate}"
                            EditableTemplate="{StaticResource EditableTemplate}" />
</Window.Resources>

<TextBox minimal:Interaction.BehaviorsTemplateSelector="{StaticResource BehaviorSelector}" />
```

```csharp
public sealed class MyBehaviorSelector : DataTemplateSelector
{
    public DataTemplate? ReadOnlyTemplate { get; set; }
    public DataTemplate? EditableTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
        => (item is MyDataItem m && m.IsReadOnly) ? ReadOnlyTemplate : EditableTemplate;
}
```
> Tip: If no selector parameter is provided, the framework resolves the element’s data item (DataContext, Content, or Header).
> Bind `BehaviorsTemplateSelectorParameter="{Binding}"` to drive selection from the `DataContext` explicitly when needed.

---

### Practical Scenarios
#### Command on Loaded
```xml
<minimal:EventToCommand EventName="Loaded" Command="{Binding InitializeCommand}" />
```
#### Deterministic routed events
```xml
<ListView>
  <minimal:Interaction.Behaviors>
    <minimal:EventToCommand EventName="SelectionChanged"
                            Command="{Binding SelectionChangedCommand}"
                            PassEventArgsToCommand="True" />
  </minimal:Interaction.Behaviors>
</ListView>
```
#### MVVM-friendly per-control shortcuts
```xml
<TextBox>
  <minimal:Interaction.Behaviors>
    <minimal:KeyToCommand Gesture="Ctrl+Enter" Command="{Binding SubmitCommand}" />
    <minimal:KeyToCommand Gesture="Escape"     Command="{Binding CancelCommand}" />
  </minimal:Interaction.Behaviors>
</TextBox>
```

#### Selector re-evaluation on DataContext changes
```xml
<TextBox minimal:Interaction.BehaviorsTemplateSelector="{StaticResource BehaviorSelector}"
         minimal:Interaction.BehaviorsTemplateSelectorParameter="{Binding}" />
```

#### Handling already-handled routed events (Preview included)
```xml
<minimal:EventToCommand EventName="PreviewMouseDown" ProcessHandledEvent="True"
                        Command="{Binding PreviewMouseDownCommand}" />
```

#### Deterministic command parameter resolution
```xml
<minimal:EventToCommand EventName="SelectionChanged"
                        Command="{Binding SelectionChangedCommand}"
                        EventArgsParameterPath="OriginalSource.SelectedItem" />
```

---

## What Makes Teams Adopt It

- **Minimal and explicit API surface.** No trigger/action stacks; behaviors are explicit and composable.
- **Template-driven composition.** Reuse behavior sets via data templates (single or multiple behaviors) — concise and flexible.
- **Selector-driven switching.** Change behavior sets at runtime with `BehaviorsTemplateSelector` and a single parameter binding.
- **Consistent WPF semantics.** Clear command targeting; no implicit focus-based resolution.
- **MVVM-first design.** Clean event → `ICommand` binding with a well-defined parameter precedence.
- **Predictable behavior lifecycle.** Dynamic composition does not leave dangling references.
- **Source package option.** Drop-in sources for straightforward embedding and debugging.

---

### Migration from Blend/Interactivity

If your project uses **Blend Behaviors**, **System.Windows.Interactivity**, or **Microsoft.Xaml.Behaviors**, migration is straightforward. Minimal.Behaviors preserves the mental model but removes the heavy trigger/action stack.

|Blend / Interactivity|Minimal equivalent|Notes|
|---|---|---|
|Interaction.Behaviors|minimal:Interaction.Behaviors|Same structure, predictable lifecycle.|
|EventTrigger + InvokeCommandAction|minimal:EventToCommand|Direct event → `ICommand` binding.|
|KeyBinding / InputBinding|minimal:KeyToCommand|Keyboard gestures per control, MVVM-friendly.|
|Reusable stacks in resources|BehaviorsTemplate|Define behavior sets once, attach anywhere.|
|Runtime switching via triggers|BehaviorsTemplateSelector|Cleaner, deterministic selection.|

**No hidden dependencies, no extra assemblies, no performance traps.**
Your existing behavior patterns map directly to smaller, clearer equivalents.

---

### Performance Tips

- **Use explicit** `CommandParameter` for high-frequency events; avoid deep parameter extraction on high-frequency routes (e.g., mouse move).
- **Keep converters lightweight** and pass only required data.
- **Reuse behavior templates** instead of repeating inline declarations.
- **Control selector re-evaluation**: bind `BehaviorsTemplateSelectorParameter` (typically to `"{Binding}"`) to update only when needed.

---

### Installation

Via [NuGet](https://www.nuget.org/):

```sh
dotnet add package NuExt.Minimal.Behaviors.Wpf
```

Or via Visual Studio:

1. Go to `Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution...`.
2. Search for `NuExt.Minimal.Behaviors.Wpf`.
3. Install.

### Source Code Package (no external binary dependency)

Prefer to vendor the framework and keep your app dependency-flat?  
Use the **source package** to embed the entire behavior infrastructure directly into your project:

- **No external binary dependency** — sources compile as part of your app.
- **Easier debugging** — step into the framework code without symbol servers.
- **Deterministic builds** — you control updates via package version pinning.
- **Same API** — identical public surface to the binary package.

> This is ideal for teams that prefer **zero external runtime dependencies** and want to keep UI infrastructure fully **in-tree**.

NuGet: [`NuExt.Minimal.Behaviors.Wpf.Sources`](https://www.nuget.org/packages/NuExt.Minimal.Behaviors.Wpf.Sources).

```sh
dotnet add package NuExt.Minimal.Behaviors.Wpf.Sources
```

Or via Visual Studio:

1. Go to `Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution...`.
2. Search for `NuExt.Minimal.Behaviors.Wpf.Sources`.
3. Install.

### Contributing

Issues and PRs are welcome. Keep changes minimal and performance-conscious.

### License

MIT. See LICENSE.