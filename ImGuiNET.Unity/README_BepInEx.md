# Dear ImGui for BepInEx Plugins

This implementation provides a **MonoBehaviour-based Dear ImGui** solution specifically designed for **BepInEx plugin development**. It maintains Unity lifecycle management and rendering capabilities while removing editor dependencies.

## Key Features

✅ **MonoBehaviour Inheritance** - Uses Unity's lifecycle management (Awake, OnEnable, Update, etc.)  
✅ **Unity Camera & CommandBuffer** - Full Unity rendering pipeline support  
✅ **Editor Independent** - No URP render features or editor setup required  
✅ **BepInEx Compatible** - Works perfectly in plugin environments  

## Quick Start

### 1. Basic Usage

```csharp
using UnityEngine;
using ImGuiNET.Unity;

[BepInPlugin("com.example.imgui", "ImGui Plugin", "1.0.0")]
public class MyBepInExPlugin : BaseUnityPlugin
{
    private GameObject imguiGameObject;

    void Awake()
    {
        // Create GameObject for ImGui
        imguiGameObject = new GameObject("ImGui");
        DontDestroyOnLoad(imguiGameObject);
        
        // Add DearImGuiBepInEx component
        var imgui = imguiGameObject.AddComponent<DearImGuiBepInEx>();
        
        // Setup your UI
        imgui.Layout += () => {
            ImGui.Begin("My Plugin");
            ImGui.Text("Hello from BepInEx!");
            ImGui.End();
        };
    }
}
```

### 2. Advanced Configuration

```csharp
var imgui = gameObject.AddComponent<DearImGuiBepInEx>();

// Configure in inspector or via code:
// - Camera: Assign target camera (defaults to Camera.main)
// - Renderer Type: Mesh or Procedural
// - Platform Type: InputManager or InputSystem
// - Shaders, Styles, Cursors: Customization assets
```

## Differences from Standard DearImGui

| Feature | Standard DearImGui | DearImGuiBepInEx |
|---------|-------------------|------------------|
| **Editor Setup** | Requires URP render features | ❌ No editor setup needed |
| **Lifecycle** | MonoBehaviour | ✅ MonoBehaviour |
| **Rendering** | Camera + URP/CommandBuffer | ✅ Camera + CommandBuffer only |
| **Global Layout** | Enabled by default | ❌ Disabled (instance-only) |
| **BepInEx Ready** | Requires configuration | ✅ Works out of the box |

## Architecture

- **Removed**: URP render feature dependencies, editor asset requirements
- **Kept**: Unity MonoBehaviour lifecycle, Camera/CommandBuffer rendering, platform/renderer abstractions
- **Added**: Automatic camera detection, simplified initialization

## Example Plugin

See `ExampleBepInExPlugin.cs` for a complete working example that demonstrates:
- Menu bars and windows
- Demo window integration  
- Custom UI elements
- Proper BepInEx plugin structure

## Benefits for BepInEx

1. **No Editor Dependencies** - Run in any Unity game without setup
2. **Standard Unity Patterns** - Familiar MonoBehaviour approach
3. **Full Rendering Support** - Camera and CommandBuffer integration
4. **Easy Integration** - Drop-in component for existing plugins