using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SomeShit.Engine.Renderer;

namespace SomeShit.Renderer;

public class Sphere : IShape
{
    private readonly List<Vector3> _vertices = new();
    private readonly List<int[]> _indices = new();
    private readonly float _radius;
    private readonly int _latitudeSegments;
    private readonly int _longitudeSegments;

    public float AngleX { get; set; }
    public float AngleY { get; set; }
    public float AngleZ { get; set; }
    public bool AutoRotate { get; set; } = false;

    public Sphere(float radius, int latitudeSegments, int longitudeSegments)
    {
        _radius = radius;
        _latitudeSegments = latitudeSegments;
        _longitudeSegments = longitudeSegments;

        GenerateSphere();
    }

    private void GenerateSphere()
    {
        for (int lat = 0; lat <= _latitudeSegments; lat++)
        {
            float theta = MathF.PI * lat / _latitudeSegments;
            float sinTheta = MathF.Sin(theta);
            float cosTheta = MathF.Cos(theta);

            for (int lon = 0; lon <= _longitudeSegments; lon++)
            {
                float phi = 2 * MathF.PI * lon / _longitudeSegments;
                float sinPhi = MathF.Sin(phi);
                float cosPhi = MathF.Cos(phi);

                float x = _radius * sinTheta * cosPhi;
                float y = _radius * sinTheta * sinPhi;
                float z = _radius * cosTheta;

                _vertices.Add(new Vector3(x, y, z));
            }
        }

        for (int lat = 0; lat < _latitudeSegments; lat++)
        {
            for (int lon = 0; lon < _longitudeSegments; lon++)
            {
                int first = lat * (_longitudeSegments + 1) + lon;
                int second = first + _longitudeSegments + 1;

                _indices.Add(new[] { first, second, first + 1 });
                _indices.Add(new[] { second, second + 1, first + 1 });
            }
        }
    }

    public void RotateSphere(float angleX, float angleY, float angleZ)
    {
        AngleX = angleX;
        AngleY = angleY;
        AngleZ = angleZ;
    }

    public void Rotate(float angleX, float angleY, float angleZ)
    {
        AngleX += angleX;
        AngleY += angleY;
        AngleZ += angleZ;
    }

    public void UpdateRotation(float deltaTime)
    {
        if (AutoRotate)
        {
            Rotate(10f * deltaTime, 15f * deltaTime, 20f * deltaTime);
        }
    }

    public void SetAutoRotate(bool autoRotate)
    {
        AutoRotate = autoRotate;
    }

    public void Delete()
    {
        _vertices.Clear();
        _indices.Clear();
        Console.WriteLine("Sphere deleted.");
    }

    public void Draw()
    {
        GL.LoadIdentity();
        GL.Translate(0, 0, -4);
        GL.Color3(1.0f, 1.0f, 1.0f);

        GL.Begin(PrimitiveType.LineLoop);
        foreach (var triangle in _indices)
        {
            foreach (var index in triangle)
            {
                GL.Vertex3(_vertices[index]);
            }
        }
        GL.End();
    }
}
