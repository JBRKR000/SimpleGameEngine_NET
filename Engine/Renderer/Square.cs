using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SomeShit.Engine.Renderer;

namespace SomeShit.Renderer;

public class Square: IShape
{
    private Vector2 _a;
    private Vector2 _b;
    private Vector2 _c;
    private Vector2 _d;

    public Square(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        _a = a;
        _b = b;
        _c = c;
        _d = d;
    }

    public void Draw()
    {
        GL.LoadIdentity();
        GL.Color3(1.0f, 1.0f, 1.0f);
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(_a);
        GL.Vertex2(_b);
        GL.Vertex2(_c);
        GL.Vertex2(_d);
        GL.End();
    }
}