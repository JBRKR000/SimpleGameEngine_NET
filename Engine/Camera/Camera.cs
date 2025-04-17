using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SomeShit.Engine.Camera;

public class Camera
{
    private Matrix4 _projectionMatrix;
    private Matrix4 _viewMatrix;

    public Camera(float fov, float aspectRatio, float near, float far)
    {
        _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(fov),
            aspectRatio,
            near,
            far
        );
        _viewMatrix = Matrix4.Identity;
    }

    public void SetView(Vector3 position, Vector3 target, Vector3 up)
    {
        _viewMatrix = Matrix4.LookAt(position, target, up);
    }

    public void Apply()
    {
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref _projectionMatrix);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadMatrix(ref _viewMatrix);
    }
}