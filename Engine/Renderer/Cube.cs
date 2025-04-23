using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SomeShit.Engine.Renderer;

namespace SomeShit.Renderer;

public class Cube:IShape
{
    private Vector3[] _vertices;
    private float _angle;
    private bool autoRotate;

    public bool AutoRotate
    {
        get => autoRotate;
        set => autoRotate = value;
    }

    public float Angle
    {
        get => _angle;
        set => _angle = value;
    }

    public Cube(Vector3[] vertices)
    {
        if (vertices.Length != 8)
            throw new ArgumentException("Cube requires exactly 8 vertices.");
        _vertices = vertices;
    }

    public Vector3 TranslateCube(int x, int y, int z)
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            _vertices[i].X += x;
            _vertices[i].Y += y;
            _vertices[i].Z += z;
        }
        return new Vector3(x, y, z);
    }

    public void Draw()
    {
        GL.LoadIdentity();
        GL.Translate(0,0,-4);
        GL.Rotate(_angle, 0, 1, 0);
        GL.Color3(Color.White);
        GL.Scale(1,1,1);
        
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex3(_vertices[0]);
        GL.Vertex3(_vertices[1]);
        GL.Vertex3(_vertices[2]);
        GL.Vertex3(_vertices[3]);
        GL.End();
        
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex3(_vertices[4]);
        GL.Vertex3(_vertices[5]);
        GL.Vertex3(_vertices[6]);
        GL.Vertex3(_vertices[7]);
        GL.End();

        // Left face
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex3(_vertices[0]);
        GL.Vertex3(_vertices[3]);
        GL.Vertex3(_vertices[7]);
        GL.Vertex3(_vertices[4]);
        GL.End();

        // Right face
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex3(_vertices[1]);
        GL.Vertex3(_vertices[2]);
        GL.Vertex3(_vertices[6]);
        GL.Vertex3(_vertices[5]);
        GL.End();

        // Top face
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex3(_vertices[0]);
        GL.Vertex3(_vertices[1]);
        GL.Vertex3(_vertices[5]);
        GL.Vertex3(_vertices[4]);
        GL.End();

        // Bottom face
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex3(_vertices[3]);
        GL.Vertex3(_vertices[2]);
        GL.Vertex3(_vertices[6]);
        GL.Vertex3(_vertices[7]);
        GL.End();
    
    }
    
}