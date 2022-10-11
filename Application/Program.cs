using Raylib_cs;

namespace Application;

internal static class Program
{
    private static void InitWindow(int width, int height, string title)
    {
        Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT |
                              ConfigFlags.FLAG_VSYNC_HINT |
                              ConfigFlags.FLAG_WINDOW_RESIZABLE);
        Raylib.SetTraceLogLevel(TraceLogLevel.LOG_WARNING);
        Raylib.InitWindow(width, height, title);
        Raylib.SetWindowMinSize(640, 480);
    }

    private static void Main()
    {
        InitWindow(1280, 720, "Raylib + Dear ImGui app");
        UI.UiManager.Setup();

        while (!Raylib.WindowShouldClose())
        {
            // Update:
            UI.UiManager.Update();

            // Render:
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            UI.UiManager.Render();
            Raylib.DrawFPS(0, 0);
            Raylib.EndDrawing();
        }

        UI.UiManager.Shutdown();
        Raylib.CloseWindow();
    }
}