# ImGuiNET.BepInEx

A .NET Framework 4.8 version of Dear ImGui designed for use with BepInEx plugins. This version removes all Unity MonoBehaviour dependencies and Unity editor-specific code, making it suitable for runtime-only use in Unity games through BepInEx plugins.

## Features

- **No Unity MonoBehaviour dependency**: Can be used in any .NET Framework 4.8 application or BepInEx plugin
- **Runtime-only**: No Unity editor dependencies
- **Minimal Unity integration**: Uses reflection to access Unity APIs when available, but can work without them
- **BepInEx compatible**: Designed specifically for BepInEx plugin development
- **Manual lifecycle management**: Initialize, update, and cleanup are all done manually

## Key Differences from Unity Version

| Unity Version | BepInEx Version |
|---------------|-----------------|
| Inherits from MonoBehaviour | Plain C# class |
| Uses Unity lifecycle (Awake, OnEnable, Update, etc.) | Manual initialization and update |
| Requires Unity editor setup | Runtime-only configuration |
| Automatic rendering integration | Manual rendering calls |
| Unity-specific input handling | Platform-abstracted input |

## Basic Usage

### 1. Initialize in your BepInEx plugin

```csharp
[BepInPlugin("com.example.imguiplugin", "ImGui Example Plugin", "1.0.0")]
public class MyBepInExPlugin : BaseUnityPlugin
{
    private DearImGuiBepInEx _imgui;

    void Awake()
    {
        // Create and initialize ImGui
        _imgui = new DearImGuiBepInEx(new ImGuiConfig
        {
            GlobalLayout = false,
            KeyboardNavigation = true
        });

        // Set up your UI layout
        _imgui.Layout += OnImGuiLayout;

        // Initialize with Unity-compatible platform
        var platform = new UnityBepInExPlatform();
        var renderer = new BepInExRendererStub(); // Use appropriate renderer
        
        _imgui.Initialize(platform, renderer);
    }

    void Update()
    {
        // Render ImGui each frame
        _imgui?.Render();
    }

    void OnDestroy()
    {
        // Clean up
        _imgui?.Dispose();
    }

    private void OnImGuiLayout()
    {
        // Your ImGui UI code here
        ImGui.Begin("My Plugin Window");
        ImGui.Text("Hello from BepInEx plugin!");
        ImGui.End();
    }
}
```

### 2. Configuration Options

```csharp
var config = new ImGuiConfig
{
    GlobalLayout = true,           // Enable global layout events
    KeyboardNavigation = true,     // Enable keyboard navigation
    GamepadNavigation = false,     // Enable gamepad navigation
    MouseDrawCursor = false        // Let ImGui draw mouse cursor
};
```

## Architecture

### Core Classes

- **`DearImGuiBepInEx`**: Main ImGui manager (replaces `DearImGui` MonoBehaviour)
- **`ImGuiBepInExContext`**: ImGui context management (replaces `ImGuiUnityContext`)
- **`BepInExTextureManager`**: Texture management without Unity dependencies

### Platform Abstraction

- **`IBepInExPlatform`**: Interface for platform-specific functionality (input, windowing)
- **`UnityBepInExPlatform`**: Unity-compatible platform implementation using reflection
- **`BepInExPlatformStub`**: Basic stub implementation

### Renderer Abstraction

- **`IBepInExRenderer`**: Interface for rendering ImGui draw commands
- **`BepInExRendererStub`**: Basic stub implementation

## Unity Integration

The BepInEx version uses reflection to access Unity APIs when available, avoiding direct dependencies:

```csharp
// Access Unity Screen without direct dependency
var screenType = Type.GetType("UnityEngine.Screen, UnityEngine.CoreModule");
var widthProp = screenType.GetProperty("width");
int screenWidth = (int)widthProp.GetValue(null);
```

This allows the library to work with Unity when available, but also function in non-Unity environments.

## Limitations

1. **No automatic rendering**: You must call `Render()` manually each frame
2. **Basic input handling**: The included platform implementations provide basic input handling
3. **Stub renderer**: You'll need to implement a proper renderer for your graphics backend
4. **No editor integration**: This is runtime-only, no Unity editor support

## Extending

### Custom Platform Implementation

```csharp
public class MyCustomPlatform : IBepInExPlatform
{
    public bool Initialize(ImGuiIOPtr io)
    {
        // Set up your platform-specific initialization
        io.BackendPlatformName = "My_Custom_Platform";
        return true;
    }

    public void PrepareFrame(ImGuiIOPtr io)
    {
        // Update input, display size, delta time, etc.
        io.DisplaySize = GetDisplaySize();
        io.DeltaTime = GetDeltaTime();
        UpdateInput(io);
    }

    public void Shutdown(ImGuiIOPtr io)
    {
        // Clean up platform resources
    }
}
```

### Custom Renderer Implementation

```csharp
public class MyCustomRenderer : IBepInExRenderer
{
    public bool Initialize(ImGuiIOPtr io)
    {
        io.BackendRendererName = "My_Custom_Renderer";
        return true;
    }

    public void RenderDrawData(ImDrawDataPtr drawData)
    {
        // Submit draw commands to your graphics API
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            var cmdList = drawData.CmdListsRange[i];
            // Process draw commands...
        }
    }

    public void Shutdown(ImGuiIOPtr io)
    {
        // Clean up renderer resources
    }
}
```

## Building

This project targets .NET Framework 4.8 and can be built with:

- Visual Studio 2019 or later
- .NET Framework 4.8 SDK
- MSBuild

```bash
msbuild ImGuiNET.BepInEx.csproj /p:Configuration=Release
```

## Dependencies

- .NET Framework 4.8
- ImGui.NET (core ImGui wrapper)
- System.Numerics (for vectors)

Note: Unity dependencies are accessed via reflection when available, but are not required.