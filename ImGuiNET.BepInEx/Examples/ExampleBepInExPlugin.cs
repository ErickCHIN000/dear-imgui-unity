using System;
using UnityEngine;
using ImGuiNET.Unity;

namespace ImGuiNET.Unity.Examples
{
    /// <summary>
    /// Example BepInEx plugin using Dear ImGui with Unity MonoBehaviour
    /// This shows how to use DearImGuiBepInEx in a BepInEx plugin without editor dependencies
    /// </summary>
    public class ExampleBepInExPlugin : MonoBehaviour
    {
        private DearImGuiBepInEx _imgui;
        private bool _showDemoWindow = true;
        private bool _showAnotherWindow = false;
        private float[] _clearColor = new float[] { 0.45f, 0.55f, 0.60f, 1.00f };

        /// <summary>
        /// Initialize the plugin - called automatically by Unity
        /// </summary>
        void Awake()
        {
            // Create ImGui component
            _imgui = gameObject.AddComponent<DearImGuiBepInEx>();
            
            // Set up our custom layout
            _imgui.Layout += OnImGuiLayout;
            
            Debug.Log("Dear ImGui BepInEx plugin initialized");
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
            ImGui.Text("Using Unity MonoBehaviour but no editor dependencies.");
            
            ImGui.Separator();
            
            ImGui.ColorEdit3("Clear color", ref _clearColor[0]);
            
            if (ImGui.Button("Click me!"))
            {
                Debug.Log("Button clicked in ImGui!");
            }
            
            ImGui.Text($"Application average {1000.0f / ImGui.GetIO().Framerate:F3} ms/frame ({ImGui.GetIO().Framerate:F1} FPS)");
            
            ImGui.End();
        }
    }
}