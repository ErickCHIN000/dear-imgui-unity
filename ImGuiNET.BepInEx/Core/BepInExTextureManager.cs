using System;
using System.Collections.Generic;

namespace ImGuiNET.BepInEx
{
    /// <summary>
    /// Simplified texture manager for BepInEx (without Unity dependencies)
    /// </summary>
    public class BepInExTextureManager
    {
        private object _fontAtlasTexture;
        private int _currentTextureId;
        private readonly Dictionary<int, object> _textures = new Dictionary<int, object>();
        private readonly Dictionary<object, int> _textureIds = new Dictionary<object, int>();

        /// <summary>
        /// Initialize the texture manager
        /// </summary>
        public void Initialize(ImGuiIOPtr io)
        {
            // Build default font atlas - simplified version
            _fontAtlasTexture = CreateFontAtlasTexture(io.Fonts);
            if (_fontAtlasTexture != null)
            {
                int id = RegisterTexture(_fontAtlasTexture);
                io.Fonts.SetTexID((IntPtr)id);
            }
        }

        /// <summary>
        /// Shutdown and cleanup
        /// </summary>
        public void Shutdown()
        {
            _currentTextureId = 0;
            _textures.Clear();
            _textureIds.Clear();
            _fontAtlasTexture = null;
        }

        /// <summary>
        /// Prepare frame - register font atlas texture
        /// </summary>
        public void PrepareFrame(ImGuiIOPtr io)
        {
            _currentTextureId = 0;
            _textures.Clear();
            _textureIds.Clear();
            
            if (_fontAtlasTexture != null)
            {
                int id = RegisterTexture(_fontAtlasTexture);
                io.Fonts.SetTexID((IntPtr)id);
            }
        }

        /// <summary>
        /// Register a texture and get its ID
        /// </summary>
        private int RegisterTexture(object texture)
        {
            if (texture == null) return -1;
            
            _textures[++_currentTextureId] = texture;
            _textureIds[texture] = _currentTextureId;
            return _currentTextureId;
        }

        /// <summary>
        /// Get texture by ID
        /// </summary>
        public object GetTexture(int id)
        {
            _textures.TryGetValue(id, out object texture);
            return texture;
        }

        /// <summary>
        /// Get texture ID for a texture object
        /// </summary>
        public int GetTextureId(object texture)
        {
            if (texture == null) return -1;
            
            return _textureIds.TryGetValue(texture, out int id) 
                ? id 
                : RegisterTexture(texture);
        }

        /// <summary>
        /// Create font atlas texture - to be implemented by specific platform
        /// </summary>
        protected virtual object CreateFontAtlasTexture(ImFontAtlasPtr fonts)
        {
            // This is a placeholder - actual implementation would depend on the rendering system
            // For BepInEx plugins, this might interface with Unity's texture system or other graphics APIs
            return null;
        }

        /// <summary>
        /// Destroy font atlas and clear fonts
        /// </summary>
        public unsafe void DestroyFontAtlas(ImGuiIOPtr io)
        {
            io.Fonts.Clear();
            io.NativePtr->FontDefault = default;
        }
    }
}