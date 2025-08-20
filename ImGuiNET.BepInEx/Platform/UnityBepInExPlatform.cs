using System;

namespace ImGuiNET.BepInEx.Unity
{
    /// <summary>
    /// Unity platform implementation for BepInEx plugins
    /// This provides Unity-specific input handling and display management
    /// </summary>
    public class UnityBepInExPlatform : IBepInExPlatform
    {
        private bool _wantCaptureKeyboard;
        private bool _wantCaptureMouse;

        public bool Initialize(ImGuiIOPtr io)
        {
            io.BackendPlatformName = "BepInEx_Unity_Platform";
            
            // Configure for Unity
            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            
            // Set up key mapping
            SetupKeyMapping(io);
            
            return true;
        }

        public void Shutdown(ImGuiIOPtr io)
        {
            // Cleanup
        }

        public void PrepareFrame(ImGuiIOPtr io)
        {
            // Update display size from Unity's Screen
            try
            {
                // Use reflection to access Unity's Screen class to avoid direct dependency
                var screenType = Type.GetType("UnityEngine.Screen, UnityEngine.CoreModule");
                if (screenType != null)
                {
                    var widthProp = screenType.GetProperty("width");
                    var heightProp = screenType.GetProperty("height");
                    
                    if (widthProp != null && heightProp != null)
                    {
                        int width = (int)widthProp.GetValue(null);
                        int height = (int)heightProp.GetValue(null);
                        io.DisplaySize = new System.Numerics.Vector2(width, height);
                    }
                }
            }
            catch
            {
                // Fallback to default resolution
                io.DisplaySize = new System.Numerics.Vector2(1920, 1080);
            }

            // Update delta time
            try
            {
                var timeType = Type.GetType("UnityEngine.Time, UnityEngine.CoreModule");
                if (timeType != null)
                {
                    var deltaTimeProp = timeType.GetProperty("unscaledDeltaTime");
                    if (deltaTimeProp != null)
                    {
                        float deltaTime = (float)deltaTimeProp.GetValue(null);
                        io.DeltaTime = deltaTime > 0.0f ? deltaTime : 1.0f / 60.0f;
                    }
                }
            }
            catch
            {
                io.DeltaTime = 1.0f / 60.0f;
            }

            // Update input
            UpdateInput(io);
        }

        private void SetupKeyMapping(ImGuiIOPtr io)
        {
            // Map Unity KeyCodes to ImGui keys using reflection to avoid direct Unity dependency
            try
            {
                var keyCodeType = Type.GetType("UnityEngine.KeyCode, UnityEngine.InputLegacyModule");
                if (keyCodeType != null)
                {
                    io.KeyMap[(int)ImGuiKey.Tab] = (int)Enum.Parse(keyCodeType, "Tab");
                    io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Enum.Parse(keyCodeType, "LeftArrow");
                    io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Enum.Parse(keyCodeType, "RightArrow");
                    io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Enum.Parse(keyCodeType, "UpArrow");
                    io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Enum.Parse(keyCodeType, "DownArrow");
                    io.KeyMap[(int)ImGuiKey.PageUp] = (int)Enum.Parse(keyCodeType, "PageUp");
                    io.KeyMap[(int)ImGuiKey.PageDown] = (int)Enum.Parse(keyCodeType, "PageDown");
                    io.KeyMap[(int)ImGuiKey.Home] = (int)Enum.Parse(keyCodeType, "Home");
                    io.KeyMap[(int)ImGuiKey.End] = (int)Enum.Parse(keyCodeType, "End");
                    io.KeyMap[(int)ImGuiKey.Insert] = (int)Enum.Parse(keyCodeType, "Insert");
                    io.KeyMap[(int)ImGuiKey.Delete] = (int)Enum.Parse(keyCodeType, "Delete");
                    io.KeyMap[(int)ImGuiKey.Backspace] = (int)Enum.Parse(keyCodeType, "Backspace");
                    io.KeyMap[(int)ImGuiKey.Space] = (int)Enum.Parse(keyCodeType, "Space");
                    io.KeyMap[(int)ImGuiKey.Enter] = (int)Enum.Parse(keyCodeType, "Return");
                    io.KeyMap[(int)ImGuiKey.Escape] = (int)Enum.Parse(keyCodeType, "Escape");
                    io.KeyMap[(int)ImGuiKey.A] = (int)Enum.Parse(keyCodeType, "A");
                    io.KeyMap[(int)ImGuiKey.C] = (int)Enum.Parse(keyCodeType, "C");
                    io.KeyMap[(int)ImGuiKey.V] = (int)Enum.Parse(keyCodeType, "V");
                    io.KeyMap[(int)ImGuiKey.X] = (int)Enum.Parse(keyCodeType, "X");
                    io.KeyMap[(int)ImGuiKey.Y] = (int)Enum.Parse(keyCodeType, "Y");
                    io.KeyMap[(int)ImGuiKey.Z] = (int)Enum.Parse(keyCodeType, "Z");
                }
            }
            catch
            {
                // If reflection fails, set up basic key mapping with hardcoded values
                io.KeyMap[(int)ImGuiKey.Tab] = 9;       // Tab
                io.KeyMap[(int)ImGuiKey.LeftArrow] = 276; // LeftArrow
                io.KeyMap[(int)ImGuiKey.RightArrow] = 275; // RightArrow
                io.KeyMap[(int)ImGuiKey.UpArrow] = 273;    // UpArrow
                io.KeyMap[(int)ImGuiKey.DownArrow] = 274;  // DownArrow
                io.KeyMap[(int)ImGuiKey.Enter] = 13;      // Return
                io.KeyMap[(int)ImGuiKey.Escape] = 27;     // Escape
            }
        }

        private void UpdateInput(ImGuiIOPtr io)
        {
            // This is a simplified input update
            // Real implementation would hook into Unity's input system
            // For now, we just clear the input state
            
            // Mouse
            io.MousePos = new System.Numerics.Vector2(-1, -1);
            for (int i = 0; i < io.MouseDown.Count; i++)
                io.MouseDown[i] = false;
            io.MouseWheel = 0.0f;

            // Keyboard  
            for (int i = 0; i < io.KeysDown.Count; i++)
                io.KeysDown[i] = false;
            
            io.KeyCtrl = false;
            io.KeyShift = false;
            io.KeyAlt = false;
            io.KeySuper = false;
        }
    }
}