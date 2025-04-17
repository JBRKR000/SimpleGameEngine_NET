using SomeShit.Engine.Core;
using SomeShit.Engine.Renderer;

GameMain game = new GameMain();
ImGuiRenderer renderer = new ImGuiRenderer(game);
Thread renderThread = new Thread(() => renderer.Start().Wait());
renderThread.Start();
game.Run();