using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SomeShit.Renderer;

public class Cube
{
    private Vector3[] _vertices;

    public Cube(Vector3[] vertices)
    {
        if (vertices.Length != 8)
            throw new ArgumentException("Cube requires exactly 8 vertices.");
        _vertices = vertices;
    }

    public void Draw(float angle)
    {
        GL.LoadIdentity();
        GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
        GL.Color3(Color.White);
        GL.Scale(0.5,0.25,1);

        // Front face
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex3(_vertices[0]);
        GL.Vertex3(_vertices[1]);
        GL.Vertex3(_vertices[2]);
        GL.Vertex3(_vertices[3]);
        GL.End();

        // Back face
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