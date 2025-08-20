using System;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Profiling;
using ImGuiNET.Unity;

namespace ImGuiNET.BepInEx
{
    /// <summary>
    /// Configuration for ImGui initialization
    /// </summary>
    [Serializable]
    public class ImGuiConfig
    {
        public bool GlobalLayout = true;
        public bool MouseDrawCursor = false;
        public bool KeyboardNavigation = false;
        public bool GamepadNavigation = false;
    }

    /// <summary>
    /// Dear ImGui manager for BepInEx plugins - Unity MonoBehaviour based but editor-independent
    /// </summary>
    public class DearImGuiBepInEx : MonoBehaviour
    {
        private ImGuiUnityContext _context;
        private IImGuiPlatform _platform;
        private IImGuiRenderer _renderer;
        private CommandBuffer _cmd;

        [Header("Configuration")]
        [SerializeField] private ImGuiConfig _configuration = new ImGuiConfig();
        [SerializeField] private Camera _camera = null;

        [Header("Advanced")]
        [SerializeField] private Platform.Type _platformType = Platform.Type.InputManager;
        [SerializeField] private RenderUtils.RenderType _rendererType = RenderUtils.RenderType.Mesh;

        /// <summary>
        /// Layout event for this ImGui instance
        /// </summary>
        public event Action Layout;

        /// <summary>
        /// Configuration
        /// </summary>
        public ImGuiConfig Configuration => _configuration;

        const string CommandBufferTag = "DearImGuiBepInEx";
        static readonly ProfilerMarker s_prepareFramePerfMarker = new ProfilerMarker("DearImGuiBepInEx.PrepareFrame");
        static readonly ProfilerMarker s_layoutPerfMarker = new ProfilerMarker("DearImGuiBepInEx.Layout");
        static readonly ProfilerMarker s_drawListPerfMarker = new ProfilerMarker("DearImGuiBepInEx.RenderDrawLists");

        void Awake()
        {
            _context = ImGuiUn.CreateUnityContext();
        }

        void OnDestroy()
        {
            ImGuiUn.DestroyUnityContext(_context);
        }

        void OnEnable()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    Debug.LogError("DearImGuiBepInEx: No camera assigned and Camera.main is null");
                    enabled = false;
                    return;
                }
            }

            _cmd = RenderUtils.GetCommandBuffer(CommandBufferTag);
            _camera.AddCommandBuffer(CameraEvent.AfterEverything, _cmd);

            ImGuiUn.SetUnityContext(_context);
            ImGuiIOPtr io = ImGui.GetIO();

            ApplyConfiguration(io);
            
            _context.textures.BuildFontAtlas(io, null);
            _context.textures.Initialize(io);

            SetPlatform(Platform.Create(_platformType, null, null), io);
            SetRenderer(RenderUtils.Create(_rendererType, null, _context.textures), io);
            
            if (_platform == null || _renderer == null)
            {
                Debug.LogError("DearImGuiBepInEx: Failed to initialize platform or renderer");
                OnDisable();
                enabled = false;
            }
        }

        void OnDisable()
        {
            if (_context == null) return;

            ImGuiUn.SetUnityContext(_context);
            ImGuiIOPtr io = ImGui.GetIO();

            SetRenderer(null, io);
            SetPlatform(null, io);

            ImGuiUn.SetUnityContext(null);

            _context.textures.Shutdown();
            _context.textures.DestroyFontAtlas(io);

            if (_camera != null && _cmd != null)
                _camera.RemoveCommandBuffer(CameraEvent.AfterEverything, _cmd);

            if (_cmd != null)
                RenderUtils.ReleaseCommandBuffer(_cmd);
            _cmd = null;
        }

        void Reset()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            if (_context == null) return;

            ImGuiUn.SetUnityContext(_context);
            ImGuiIOPtr io = ImGui.GetIO();

            s_prepareFramePerfMarker.Begin(this);
            _context.textures.PrepareFrame(io);
            _platform.PrepareFrame(io, _camera.pixelRect);
            ImGui.NewFrame();
            s_prepareFramePerfMarker.End();

            s_layoutPerfMarker.Begin(this);
            try
            {
                if (_configuration.GlobalLayout)
                    ImGuiUn.DoLayout();   // Global handlers
                Layout?.Invoke();         // Instance-specific handlers
            }
            finally
            {
                ImGui.Render();
                s_layoutPerfMarker.End();
            }

            s_drawListPerfMarker.Begin(this);
            _cmd.Clear();
            _renderer.RenderDrawLists(_cmd, ImGui.GetDrawData());
            s_drawListPerfMarker.End();
        }

        /// <summary>
        /// Apply configuration to ImGui IO
        /// </summary>
        private void ApplyConfiguration(ImGuiIOPtr io)
        {
            if (_configuration.MouseDrawCursor)
                io.ConfigFlags |= ImGuiConfigFlags.NoMouseCursorChange;
            
            if (_configuration.KeyboardNavigation)
                io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            
            if (_configuration.GamepadNavigation)
                io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        }

        void SetRenderer(IImGuiRenderer renderer, ImGuiIOPtr io)
        {
            _renderer?.Shutdown(io);
            _renderer = renderer;
            _renderer?.Initialize(io);
        }

        void SetPlatform(IImGuiPlatform platform, ImGuiIOPtr io)
        {
            _platform?.Shutdown(io);
            _platform = platform;
            _platform?.Initialize(io);
        }

        /// <summary>
        /// Reload ImGui (disable and re-enable)
        /// </summary>
        public void Reload()
        {
            OnDisable();
            OnEnable();
        }
    }
}