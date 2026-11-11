using Donkeh.UILab;
using Donkeh.Windowing;
using static Donkeh.UILab._;

var window = new IgniteWindow(WindowSettings.Default with
{
    Width = 1280,
    Height = 720,
    Title = "Empty Donkeh UI Project"
});

window.Run(
    H("div", new Props { ClassName = "app" },
        H("h1", Text("Hello world"))),
    """
                .app { padding: 0px; }
                h1 { margin: 0px; }
             """);