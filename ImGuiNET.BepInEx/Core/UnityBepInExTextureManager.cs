using System;

namespace ImGuiNET.BepInEx.Unity
{
    /// <summary>
    /// Unity-compatible texture manager for BepInEx plugins
    /// </summary>
    public class UnityBepInExTextureManager : BepInExTextureManager
    {
        /// <summary>
        /// Create font atlas texture using Unity's texture system
        /// </summary>
        protected override object CreateFontAtlasTexture(ImFontAtlasPtr fonts)
        {
            try
            {
                // Get texture data from ImGui
                fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);
                
                if (pixels == IntPtr.Zero || width <= 0 || height <= 0)
                    return null;

                // Use reflection to create Unity Texture2D to avoid direct dependency
                var texture2DType = Type.GetType("UnityEngine.Texture2D, UnityEngine.CoreModule");
                var textureFormatType = Type.GetType("UnityEngine.TextureFormat, UnityEngine.CoreModule");
                
                if (texture2DType == null || textureFormatType == null)
                    return null;

                // Create Texture2D instance (width, height, TextureFormat.RGBA32, false)
                var rgba32Format = Enum.Parse(textureFormatType, "RGBA32");
                var texture = Activator.CreateInstance(texture2DType, width, height, rgba32Format, false);
                
                if (texture == null)
                    return null;

                // Copy pixel data
                var setPixelDataMethod = texture2DType.GetMethod("SetPixelData", new[] { typeof(IntPtr), typeof(int), typeof(int) });
                var applyMethod = texture2DType.GetMethod("Apply", new[] { typeof(bool), typeof(bool) });
                
                if (setPixelDataMethod != null && applyMethod != null)
                {
                    // SetPixelData(pixels, 0, 0)
                    setPixelDataMethod.Invoke(texture, new object[] { pixels, 0, 0 });
                    
                    // Apply(false, false) - don't generate mipmaps, don't make non-readable
                    applyMethod.Invoke(texture, new object[] { false, false });
                }
                else
                {
                    // Fallback: use LoadRawTextureData if SetPixelData is not available
                    var loadRawMethod = texture2DType.GetMethod("LoadRawTextureData", new[] { typeof(IntPtr), typeof(int) });
                    if (loadRawMethod != null && applyMethod != null)
                    {
                        int dataSize = width * height * bytesPerPixel;
                        loadRawMethod.Invoke(texture, new object[] { pixels, dataSize });
                        applyMethod.Invoke(texture, new object[] { false, false });
                    }
                }

                // Set texture properties
                var filterModeType = Type.GetType("UnityEngine.FilterMode, UnityEngine.CoreModule");
                if (filterModeType != null)
                {
                    var filterModeProp = texture2DType.GetProperty("filterMode");
                    if (filterModeProp != null)
                    {
                        var linearFilter = Enum.Parse(filterModeType, "Bilinear");
                        filterModeProp.SetValue(texture, linearFilter);
                    }
                }

                var textureWrapModeType = Type.GetType("UnityEngine.TextureWrapMode, UnityEngine.CoreModule");
                if (textureWrapModeType != null)
                {
                    var wrapModeProp = texture2DType.GetProperty("wrapMode");
                    if (wrapModeProp != null)
                    {
                        var clampWrap = Enum.Parse(textureWrapModeType, "Clamp");
                        wrapModeProp.SetValue(texture, clampWrap);
                    }
                }

                return texture;
            }
            catch (Exception ex)
            {
                // If Unity texture creation fails, fall back to base implementation
                System.Diagnostics.Debug.WriteLine($"Failed to create Unity font atlas texture: {ex.Message}");
                return base.CreateFontAtlasTexture(fonts);
            }
        }

        /// <summary>
        /// Get Unity texture ID - works with Unity Texture objects
        /// </summary>
        public int GetUnityTextureId(object unityTexture)
        {
            if (unityTexture == null) return -1;

            try
            {
                // Check if it's a Unity Texture
                var textureType = Type.GetType("UnityEngine.Texture, UnityEngine.CoreModule");
                if (textureType != null && textureType.IsAssignableFrom(unityTexture.GetType()))
                {
                    return GetTextureId(unityTexture);
                }
            }
            catch
            {
                // Ignore reflection errors
            }

            return -1;
        }
    }
}