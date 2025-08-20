using System;
using ImGuiNET.BepInEx;
using ImGuiNET.BepInEx.Unity;

namespace ImGuiNET.BepInEx.Examples
{
    /// <summary>
    /// Example BepInEx plugin using Dear ImGui
    /// This shows how to integrate ImGui into a BepInEx plugin without Unity MonoBehaviour dependencies
    /// </summary>
    public class ExampleBepInExPlugin
    {
        private DearImGuiBepInEx _imguiManager;
        private bool _showDemoWindow = true;
        private bool _showAnotherWindow = false;
        private float[] _clearColor = new float[] { 0.45f, 0.55f, 0.60f, 1.00f };

        /// <summary>
        /// Initialize the plugin - call this from your BepInEx plugin's Awake() or Start()
        /// </summary>
        public void Initialize()
        {
            try
            {
                // Create ImGui manager with Unity platform support
                _imguiManager = new DearImGuiBepInEx(new ImGuiConfig
                {
                    GlobalLayout = false, // We'll handle layout ourselves
                    KeyboardNavigation = true,
                    GamepadNavigation = false
                });

                // Set up our custom layout
                _imguiManager.Layout += OnImGuiLayout;

                // Initialize with Unity-compatible platform and renderer
                var platform = new UnityBepInExPlatform();
                var renderer = new BepInExRendererStub(); // You'd want a real Unity renderer here
                
                bool success = _imguiManager.Initialize(platform, renderer);
                if (!success)
                {
                    throw new Exception("Failed to initialize ImGui");
                }

                Console.WriteLine("Dear ImGui initialized successfully for BepInEx plugin");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Dear ImGui: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update the ImGui - call this from your plugin's Update() method
        /// </summary>
        public void Update()
        {
            try
            {
                _imguiManager?.Render();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ImGui render error: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup - call this from your plugin's OnDestroy() or when shutting down
        /// </summary>
        public void Shutdown()
        {
            try
            {
                _imguiManager?.Dispose();
                _imguiManager = null;
                Console.WriteLine("Dear ImGui shut down successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ImGui shutdown error: {ex.Message}");
            }
        }

        /// <summary>
        /// ImGui layout callback - this is where you put your ImGui UI code
        /// </summary>
        private void OnImGuiLayout()
        {
            // Main menu bar
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Windows"))
                {
                    ImGui.MenuItem("Demo Window", null, ref _showDemoWindow);
                    ImGui.MenuItem("Another Window", null, ref _showAnotherWindow);
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }

            // Show demo window
            if (_showDemoWindow)
            {
                ImGui.ShowDemoWindow(ref _showDemoWindow);
            }

            // Show another simple window
            if (_showAnotherWindow)
            {
                ImGui.Begin("Another Window", ref _showAnotherWindow);
                ImGui.Text("Hello from another window!");
                if (ImGui.Button("Close Me"))
                    _showAnotherWindow = false;
                ImGui.End();
            }

            // Show our custom window
            ShowCustomWindow();
        }

        /// <summary>
        /// Example custom ImGui window
        /// </summary>
        private void ShowCustomWindow()
        {
            ImGui.Begin("BepInEx Plugin Example");
            
            ImGui.Text("This is a Dear ImGui window running in a BepInEx plugin!");
            ImGui.Text("No Unity MonoBehaviour dependency required.");
            
            ImGui.Separator();
            
            ImGui.ColorEdit3("Clear color", ref _clearColor[0]);
            
            if (ImGui.Button("Click me!"))
            {
                Console.WriteLine("Button clicked in ImGui!");
            }
            
            ImGui.Text($"Application average {1000.0f / ImGui.GetIO().Framerate:F3} ms/frame ({ImGui.GetIO().Framerate:F1} FPS)");
            
            ImGui.End();
        }
    }

    /// <summary>
    /// Example usage in a BepInEx plugin class
    /// </summary>
    /*
    [BepInPlugin("com.example.imguiplugin", "ImGui Example Plugin", "1.0.0")]
    public class MyBepInExPlugin : BaseUnityPlugin
    {
        private ExampleBepInExPlugin _imguiPlugin;

        void Awake()
        {
            _imguiPlugin = new ExampleBepInExPlugin();
            _imguiPlugin.Initialize();
        }

        void Update()
        {
            _imguiPlugin?.Update();
        }

        void OnDestroy()
        {
            _imguiPlugin?.Shutdown();
        }
    }
    */
}