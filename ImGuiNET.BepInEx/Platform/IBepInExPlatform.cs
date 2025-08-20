namespace ImGuiNET.BepInEx
{
    /// <summary>
    /// Platform abstraction for ImGui (input handling, windowing)
    /// </summary>
    public interface IBepInExPlatform
    {
        bool Initialize(ImGuiIOPtr io);
        void Shutdown(ImGuiIOPtr io);
        void PrepareFrame(ImGuiIOPtr io);
    }

    /// <summary>
    /// Stub platform implementation
    /// </summary>
    public class BepInExPlatformStub : IBepInExPlatform
    {
        public bool Initialize(ImGuiIOPtr io)
        {
            // Basic initialization
            io.BackendPlatformName = "BepInEx_Platform_Stub";
            return true;
        }

        public void Shutdown(ImGuiIOPtr io)
        {
            // Cleanup
        }

        public void PrepareFrame(ImGuiIOPtr io)
        {
            // Update display size - this should be implemented by specific platform
            io.DisplaySize = new System.Numerics.Vector2(1920, 1080);
            io.DeltaTime = 1.0f / 60.0f; // Assume 60 FPS
        }
    }
}