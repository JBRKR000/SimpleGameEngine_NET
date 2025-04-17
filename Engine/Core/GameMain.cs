using System.ComponentModel;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SomeShit.Engine.Renderer;
using SomeShit.Renderer;

namespace SomeShit.Engine.Core;

public class GameMain : GameWindow
{
    private static int width = 640;
    private static int height = 480;
    private Camera.Camera _camera;
    private readonly List<IShape> _shapes = new List<IShape>();
    private readonly object _shapesLock = new object();

    public GameMain() : base(
        new GameWindowSettings()
        {
            UpdateFrequency = 60.0,
        },
        new NativeWindowSettings()
        {
            Profile = ContextProfile.Compatability,
            ClientSize = new Vector2i(width, height),
            Title = "Game Window"
        })
    { }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        Environment.Exit(0);
        
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.Viewport(0, 0, Size.X, Size.Y);
        GL.ClearColor(Color.Black);
        GL.Enable(EnableCap.DepthTest);

        _camera = new Camera.Camera(45.0f, (float)Size.X / Size.Y, 0.5f, 100.0f);
        _camera.SetView(new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero, Vector3.UnitY);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        lock (_shapesLock)
        {
            foreach (var shape in _shapes)
            {
                if (shape is not Cube { AutoRotate: true } cube) continue;
                cube.Angle += 50.0f * (float)args.Time;
                if (cube.Angle >= 360.0f)
                {
                    cube.Angle -= 360.0f;
                }
            }
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _camera.Apply();

        lock (_shapesLock)
        {
            foreach (var shape in _shapes)
            {
                shape.Draw();
            }
        }

        var error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine("OpenGL Error: " + error);
        }
        SwapBuffers();
    }
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
    }
    
    
    public void AddShape(IShape shape)
    {
        lock (_shapesLock)
        {
            
            Console.WriteLine("Rendering Cube...");
            _shapes.Add(shape);
        }
    }

    public void RemoveShapes(IShape shape)
    {
        lock (_shapesLock)
        {
            Console.WriteLine("Removing cube...");
            _shapes.Remove(shape);
        }
        
    }

    public void RemoveAllShapes()
    {
        lock (_shapesLock)
        {
            _shapes.Clear();
        }
    }
}
