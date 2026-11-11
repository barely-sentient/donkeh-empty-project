# Empty Donkeh UI Project

A minimal starter project for building desktop UIs with the **Donkeh** CSS-driven
UI engine for .NET. Donkeh gives you a React-shaped component model, a real CSS
engine, flexbox/grid layout, OpenGL rendering and built-in devtools — all in
pure C#, with no JavaScript runtime.

## What is Donkeh?

Donkeh is a declarative, component-based UI engine for .NET desktop applications.
You describe your UI as a tree of HTML-like nodes (`div`, `h1`, `button`,
`input`, ...), style it with **actual CSS**, and the engine handles selector
matching, cascade, layout, painting, input routing and re-rendering.

It is built from seven independent, strictly layered assemblies, so you pay only
for what you use:

| Module | Responsibility |
| --- | --- |
| `Donkeh.Parser` | CSS tokenizer and parser (zero dependencies). |
| `Donkeh.Data` | CSS table, selector matching, specificity, cascade. |
| `Donkeh.DataEngine` | CSS stream parsing and style resolution. |
| `Donkeh.LayoutLab` | Block/inline, flexbox, CSS grid, absolute/fixed positioning, scroll. |
| `Donkeh.UILab` | Virtual DOM, components, hooks, context, keyed reconciliation. |
| `Donkeh.Windowing` | Silk.NET window, OpenGL renderer, input, text editing, frame loop. |
| `Donkeh.Inspector` | Live DOM tree, style editor, colour picker, animation browser, profiler. |

## Why Donkeh

- **A real CSS engine, not a subset.** Full tokenizer, selector matching with
  specificity and source order, cascade layers, custom properties, `!important`,
  shorthand expansion, logical properties, media/container/supports conditions,
  keyframes and per-property animations.
- **React-like, C#-native.** Components, props, hooks (`State`, `Reducer`,
  `Effect`, `LayoutEffect`, `Memo`, `Callback`, `UseRef`, `UseContext`), context
  providers, keyed children and in-place reconciliation — without a JS runtime.
  The host writes C#.
- **Actual stylesheets.** Author real `.css` files, embed them as resources and
  declare them with `[Stylesheet(...)]`. The engine merges them with the built-in
  user-agent sheet, so plain markup already looks like a document.
- **Animation support.** CSS keyframe animations and transitions with delay,
  iteration, direction and fill modes. Layout-changing and paint-only animations
  are distinguished, so a paint-only frame can reuse the previous layout.
- **Extremely low memory footprint.** Interned strings and dense integer ids,
  flat arenas instead of per-rule objects, intrusive hash buckets, reusable
  instruction and constant buffers, and style/text/box caches keyed by revision
  and viewport. Hot paths use `ReadOnlySpan<char>`, value-type tokens and pooled
  buffers — no per-frame object graph.
- **Responsive by construction.** Layout re-runs on viewport/DPI/scroll changes;
  percentage, viewport (`vw`/`vh`) and container units are first-class; the
  engine relayouts only when something actually changed.
- **HTML semantics.** A built-in user-agent stylesheet (`HtmlDefaults`) supplies
  display defaults for common tags, so semantic markup (`h1`, `p`, `button`,
  `ul`/`li`, `input`, ...) renders sensibly out of the box. Author rules win
  the cascade.
- **Built-in devtools.** Live DOM tree, style editor, colour picker, animation
  browser and frame/layout/paint profiling. Toggle with **Ctrl+Shift+I** or
  **F12**; runs in-process or in a separate OS window.
- **Host-agnostic core.** Layout and paint produce data (`LayoutResult`,
  `InstructionBuffer`), not draw calls. The OpenGL renderer is one consumer;
  tests consume the same instructions.

## Project layout

```
src/
  Components/   # one folder per component: {Name}.cs + {Name}.css
  Providers/    # context providers / service implementations
App.cs          # entry point: creates the window and mounts the UI tree
```

## Running

```bash
dotnet run
```

A 1280×720 window titled "Empty Donkeh UI Project" opens and renders
"Hello world".

## Component model

Components inherit from `Donkeh.UILab.Component` (or `Component<TProps>`) and
override `Render()`:

```csharp
using Donkeh.UILab;
using static Donkeh.UILab._;

public sealed class Hello : Component
{
    public override Node Render()
        => H("div", new Props { ClassName = "hello" },
               Text("Hello from a component"));
}
```

Mount a tree in `App.cs`:

```csharp
window.Run(
    H("div", new Props { ClassName = "app" },
        H("h1", Text("Hello world")),
        C<Hello>(new())),
    """
    .app { padding: 32px; }
    """);
```

## Hooks

Hooks are protected methods on `Component` and must run in the same order every
render:

- `State` / `StateWithUpdate` — local state with setter.
- `Reducer` — state driven by a reducer.
- `Effect` / `LayoutEffect` — side effects after commit / during commit.
- `Memo` / `Callback` — memoised values and stable callbacks.
- `UseRef` — mutable refs.
- `UseContext` — consume a context value.

Lifecycle overrides: `OnMount`, `OnUpdate`, `OnUnmount`, `ShouldRender`.

## Context providers

Wrap subtrees with `Provider<T>` to supply context values consumed via
`UseContext<T>()`:

```csharp
Provider(new ThemeContext { Accent = "#3b82f6" },
    H("div", new Props { ClassName = "app" },
        C<ThemedButton>(new())));
```

## Styling and stylesheets

Author real `.css` files and register them:

1. Embed the file in the project:

```xml
<ItemGroup>
  <EmbeddedResource Include="src/Components/Hello/Hello.css" />
</ItemGroup>
```

2. Declare it with the `Stylesheet` attribute (assembly, class, struct or module):

```csharp
using Donkeh.Windowing;

[assembly: Stylesheet("src/Components/Hello/Hello.css")]
```

Paths are relative to the project root and use `/` or `\`. Files must be embedded
resources in the same assembly. Multiple attributes merge in declaration order.

Inline styles are also supported via `Props.Style`, and the engine resolves
custom properties, cascade layers and `!important` exactly like a browser.

## Animation

Use standard CSS `@keyframes` and the `animation` / `transition` properties.
The engine distinguishes layout-changing from paint-only animations, so a
paint-only frame reuses the previous layout instead of rebuilding it.

```css
.fade-in { animation: fade 300ms ease-out both; }
@keyframes fade { from { opacity: 0; } to { opacity: 1; } }
```

## Devtools

Press **Ctrl+Shift+I** or **F12** to toggle the inspector (DOM tree, styles,
colour picker, animation browser, performance view). Disable with
`WindowSettings.EnableDevTools = false`.

## Further reading

See `src/Components/README.md` for the component file layout and more examples,
including `State`, `Effect`, and keyed children.