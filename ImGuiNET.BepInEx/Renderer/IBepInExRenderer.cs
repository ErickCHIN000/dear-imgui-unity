namespace ImGuiNET.BepInEx
{
    /// <summary>
    /// Renderer abstraction for ImGui (drawing commands)
    /// </summary>
    public interface IBepInExRenderer
    {
        bool Initialize(ImGuiIOPtr io);
        void Shutdown(ImGuiIOPtr io);
        void RenderDrawData(ImDrawDataPtr drawData);
    }

    /// <summary>
    /// Stub renderer implementation
    /// </summary>
    public class BepInExRendererStub : IBepInExRenderer
    {
        public bool Initialize(ImGuiIOPtr io)
        {
            // Basic initialization
            io.BackendRendererName = "BepInEx_Renderer_Stub";
            return true;
        }

        public void Shutdown(ImGuiIOPtr io)
        {
            // Cleanup
        }

        public void RenderDrawData(ImDrawDataPtr drawData)
        {
            // This stub doesn't actually render anything
            // Real implementations would submit draw commands to graphics API
        }
    }
}