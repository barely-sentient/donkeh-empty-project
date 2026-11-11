# Components

Components live under `src/Components/{ComponentName}/` and are made of two files:

```
src/Components/{ComponentName}/{ComponentName}.cs
src/Components/{ComponentName}/{ComponentName}.css
```

## Creating a component

Base your component on `Donkeh.UILab.Component` (see
`C:\Users\jhuds\Desktop\UI Framework\CSS-Engine\Donkeh\UILab\Source\Component.cs`).

A component must:

1. Inherit from `Component`.
2. Expose a public parameterless constructor.
3. Override `Render()` and return a `Node` (use the `H(...)` / `Text(...)` helpers
   from `Donkeh.UILab._`).
4. Call hooks (`UseState`, `UseEffect`, ...) only from `Render`, unconditionally
   and in the same order every render.

Example:

```csharp
using Donkeh.UILab;
using static Donkeh.UILab._;

namespace EmptyDonkehProject.Components;

public sealed class Hello : Component
{
    public override Node Render()
        => H("div", new Props { ClassName = "hello" },
               Text("Hello from a component"));
}
```

## Stylesheets

Each component may ship a `.css` file. Register it in two places:

1. In the project file, embed it as a resource:

```xml
<ItemGroup>
  <EmbeddedResource Include="src/Components/Hello/Hello.css" />
</ItemGroup>
```

2. Declare it with the `Stylesheet` attribute so the engine merges it at runtime:

```csharp
using Donkeh.Windowing;

[assembly: Stylesheet("src/Components/Hello/Hello.css")]
```

The attribute can also be applied to a class/struct/module instead of the whole
assembly. Paths are relative to the project root and use `/` or `\`. Files must be
embedded resources in the same assembly.

## Examples

### Counter with `State`

```csharp
using Donkeh.UILab;
using static Donkeh.UILab._;

namespace EmptyDonkehProject.Components.Counter;

public sealed class CounterProps
{
    public string Label { get; init; } = "Counter";
}

public sealed class Counter : Component<CounterProps>
{
    public override Node Render()
    {
        var (count, setCount) = State(0);

        return H("button", new Props { ClassName = "counter" },
                Text($"{Props.Label}: {count}"))
            .OnClick(() => setCount(count + 1));
    }
}
```

Mount it:

```csharp
var root = window...; // in App.cs, pass the tree to window.Run
// H("div", new Props { ClassName = "app" },
//     C<Counter, CounterProps>(new CounterProps()))
```

### Toggle with `State` + `Effect`

```csharp
using Donkeh.UILab;
using static Donkeh.UILab._;

namespace EmptyDonkehProject.Components.Toggle;

public sealed class Toggle : Component
{
    public override Node Render()
    {
        var (on, setOn) = State(false);

        Effect(() =>
        {
            // runs after commit; return a cleanup if needed
            return null;
        }, on);

        return H("button", new Props { ClassName = on ? "on" : "off" },
                Text(on ? "On" : "Off"))
            .OnClick(() => setOn(!on));
    }
}
```

### Using keyed children

```csharp
H("ul", new Props { ClassName = "list" },
    items.Select((item, i) =>
        H("li", new Props { Key = item.Id }, Text(item.Name))).ToArray());
```

### Hooks available on `Component`

`State`, `StateWithUpdate`, `Reducer`, `Effect`, `LayoutEffect`, `Memo`,
`Callback`, `UseRef`, `UseContext`. Lifecycle overrides: `OnMount`, `OnUpdate`,
`OnUnmount`, `ShouldRender`. Hooks must run in the same order every render.