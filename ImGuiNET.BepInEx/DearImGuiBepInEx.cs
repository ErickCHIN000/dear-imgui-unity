using System;

namespace ImGuiNET.BepInEx
{
    /// <summary>
    /// Configuration for ImGui initialization
    /// </summary>
    public class ImGuiConfig
    {
        public bool GlobalLayout { get; set; } = true;
        public bool MouseDrawCursor { get; set; } = false;
        public bool KeyboardNavigation { get; set; } = false;
        public bool GamepadNavigation { get; set; } = false;
    }

    /// <summary>
    /// Dear ImGui manager for BepInEx plugins - no Unity MonoBehaviour dependency
    /// </summary>
    public class DearImGuiBepInEx : IDisposable
    {
        private ImGuiBepInExContext _context;
        private IBepInExPlatform _platform;
        private IBepInExRenderer _renderer;
        private bool _initialized;
        private bool _disposed;

        /// <summary>
        /// Layout event for this ImGui instance
        /// </summary>
        public event Action Layout;

        /// <summary>
        /// Configuration
        /// </summary>
        public ImGuiConfig Configuration { get; }

        /// <summary>
        /// Create a new Dear ImGui manager
        /// </summary>
        public DearImGuiBepInEx(ImGuiConfig config = null)
        {
            Configuration = config ?? new ImGuiConfig();
        }

        /// <summary>
        /// Initialize ImGui
        /// </summary>
        public bool Initialize(IBepInExPlatform platform = null, IBepInExRenderer renderer = null)
        {
            if (_initialized) return true;

            try
            {
                // Create context
                _context = ImGuiBepInEx.CreateContext();
                ImGuiBepInEx.SetCurrentContext(_context);

                // Get IO and apply configuration
                ImGuiIOPtr io = ImGui.GetIO();
                ApplyConfiguration(io);

                // Initialize texture manager
                _context.textures.Initialize(io);

                // Set platform and renderer
                _platform = platform ?? CreateDefaultPlatform();
                _renderer = renderer ?? CreateDefaultRenderer();

                if (_platform?.Initialize(io) != true)
                {
                    throw new Exception("Failed to initialize platform");
                }

                if (_renderer?.Initialize(io) != true)
                {
                    throw new Exception("Failed to initialize renderer");
                }

                _initialized = true;
                return true;
            }
            catch (Exception ex)
            {
                Shutdown();
                throw new Exception($"Failed to initialize Dear ImGui: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Shutdown ImGui
        /// </summary>
        public void Shutdown()
        {
            if (!_initialized) return;

            if (_context != null)
            {
                ImGuiBepInEx.SetCurrentContext(_context);
                ImGuiIOPtr io = ImGui.GetIO();

                _renderer?.Shutdown(io);
                _platform?.Shutdown(io);

                ImGuiBepInEx.SetCurrentContext(null);
                ImGuiBepInEx.DestroyContext(_context);
                _context = null;
            }

            _renderer = null;
            _platform = null;
            _initialized = false;
        }

        /// <summary>
        /// Render ImGui frame
        /// </summary>
        public void Render()
        {
            if (!_initialized) return;

            ImGuiBepInEx.SetCurrentContext(_context);
            ImGuiIOPtr io = ImGui.GetIO();

            // Prepare frame
            _context.textures.PrepareFrame(io);
            _platform?.PrepareFrame(io);
            ImGui.NewFrame();

            try
            {
                // Layout
                if (Configuration.GlobalLayout)
                {
                    ImGuiBepInEx.DoLayout();
                }
                Layout?.Invoke();
            }
            finally
            {
                ImGui.Render();
            }

            // Render draw data
            _renderer?.RenderDrawData(ImGui.GetDrawData());
        }

        /// <summary>
        /// Apply configuration to ImGui IO
        /// </summary>
        private void ApplyConfiguration(ImGuiIOPtr io)
        {
            if (Configuration.MouseDrawCursor)
                io.ConfigFlags |= ImGuiConfigFlags.NoMouseCursorChange;
            
            if (Configuration.KeyboardNavigation)
                io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            
            if (Configuration.GamepadNavigation)
                io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        }

        /// <summary>
        /// Create default platform implementation
        /// </summary>
        protected virtual IBepInExPlatform CreateDefaultPlatform()
        {
            return new BepInExPlatformStub();
        }

        /// <summary>
        /// Create default renderer implementation  
        /// </summary>
        protected virtual IBepInExRenderer CreateDefaultRenderer()
        {
            return new BepInExRendererStub();
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Shutdown();
                _disposed = true;
            }
        }
    }
}