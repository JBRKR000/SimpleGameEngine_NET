
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace SomeShit;

public class GameMain:GameWindow
{
    public GameMain():base(new GameWindowSettings(), new NativeWindowSettings())
    {
        Title = "Game Window";
        Size = new OpenTK.Mathematics.Vector2i(800, 600);
    }
    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(Color.Chocolate);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        
        base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        SwapBuffers();
    }
}