using System;

namespace ImGuiNET.BepInEx
{
    /// <summary>
    /// ImGui context for BepInEx plugins (no Unity dependencies)
    /// </summary>
    public sealed class ImGuiBepInExContext
    {
        public IntPtr state;                    // ImGui internal state
        public BepInExTextureManager textures;  // texture / font state
    }

    /// <summary>
    /// ImGui manager for BepInEx plugins
    /// </summary>
    public static unsafe partial class ImGuiBepInEx
    {
        // Layout event for custom ImGui rendering
        public static event Action Layout;
        
        // Current context
        internal static ImGuiBepInExContext s_currentContext;

        /// <summary>
        /// Create a new ImGui context for BepInEx
        /// </summary>
        public static ImGuiBepInExContext CreateContext()
        {
            return new ImGuiBepInExContext
            {
                state = ImGui.CreateContext(),
                textures = new BepInExTextureManager(),
            };
        }

        /// <summary>
        /// Destroy the ImGui context
        /// </summary>
        public static void DestroyContext(ImGuiBepInExContext context)
        {
            if (context != null)
            {
                ImGui.DestroyContext(context.state);
                context.textures?.Shutdown();
            }
        }

        /// <summary>
        /// Set the current ImGui context
        /// </summary>
        public static void SetCurrentContext(ImGuiBepInExContext context)
        {
            s_currentContext = context;
            ImGui.SetCurrentContext(context?.state ?? IntPtr.Zero);
        }

        /// <summary>
        /// Get a texture ID for use with ImGui
        /// </summary>
        public static int GetTextureId(object texture) 
        {
            return s_currentContext?.textures.GetTextureId(texture) ?? -1;
        }

        /// <summary>
        /// Invoke the global layout event
        /// </summary>
        internal static void DoLayout() => Layout?.Invoke();
    }
}