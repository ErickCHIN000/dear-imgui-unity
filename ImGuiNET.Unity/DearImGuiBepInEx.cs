using System;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Profiling;

namespace ImGuiNET.Unity
{
    /// <summary>
    /// Dear ImGui integration for BepInEx plugins - MonoBehaviour based but editor-independent
    /// This version removes editor dependencies like URP render features while keeping Unity lifecycle
    /// </summary>
    public class DearImGuiBepInEx : MonoBehaviour
    {
        ImGuiUnityContext _context;
        IImGuiRenderer _renderer;
        IImGuiPlatform _platform;
        CommandBuffer _cmd;

        public event System.Action Layout;

        [SerializeField] Camera _camera = null;
        [SerializeField] RenderUtils.RenderType _rendererType = RenderUtils.RenderType.Mesh;
        [SerializeField] Platform.Type _platformType = Platform.Type.InputManager;

        [Header("Configuration")]
        [SerializeField] IOConfig _initialConfiguration = default;
        [SerializeField] FontAtlasConfigAsset _fontAtlasConfiguration = null;
        [SerializeField] IniSettingsAsset _iniSettings = null;

        [Header("Customization")]
        [SerializeField] ShaderResourcesAsset _shaders = null;
        [SerializeField] StyleAsset _style = null;
        [SerializeField] CursorShapesAsset _cursorShapes = null;

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
            // Auto-assign camera if not set
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    Debug.LogError($"DearImGuiBepInEx: No camera assigned and Camera.main is null on {gameObject.name}");
                    enabled = false;
                    return;
                }
            }

            // Create command buffer and add to camera (no URP support to avoid editor dependencies)
            _cmd = RenderUtils.GetCommandBuffer(CommandBufferTag);
            _camera.AddCommandBuffer(CameraEvent.AfterEverything, _cmd);

            ImGuiUn.SetUnityContext(_context);
            ImGuiIOPtr io = ImGui.GetIO();

            _initialConfiguration.ApplyTo(io);
            _style?.ApplyTo(ImGui.GetStyle());

            _context.textures.BuildFontAtlas(io, _fontAtlasConfiguration);
            _context.textures.Initialize(io);

            SetPlatform(Platform.Create(_platformType, _cursorShapes, _iniSettings), io);
            SetRenderer(RenderUtils.Create(_rendererType, _shaders, _context.textures), io);
            
            if (_platform == null) Fail(nameof(_platform));
            if (_renderer == null) Fail(nameof(_renderer));

            void Fail(string reason)
            {
                OnDisable();
                enabled = false;
                throw new System.Exception($"Failed to start DearImGuiBepInEx: {reason}");
            }
        }

        void OnDisable()
        {
            ImGuiUn.SetUnityContext(_context);
            ImGuiIOPtr io = ImGui.GetIO();

            SetRenderer(null, io);
            SetPlatform(null, io);

            ImGuiUn.SetUnityContext(null);

            _context.textures.Shutdown();
            _context.textures.DestroyFontAtlas(io);

            // Always use legacy camera command buffer (no URP)
            if (_camera != null && _cmd != null)
                _camera.RemoveCommandBuffer(CameraEvent.AfterEverything, _cmd);

            if (_cmd != null)
                RenderUtils.ReleaseCommandBuffer(_cmd);
            _cmd = null;
        }

        void Reset()
        {
            _camera = Camera.main;
            _initialConfiguration.SetDefaults();
        }

        public void Reload()
        {
            OnDisable();
            OnEnable();
        }

        void Update()
        {
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
                Layout?.Invoke();     // Only instance-specific handlers (no global layout)
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
    }
}