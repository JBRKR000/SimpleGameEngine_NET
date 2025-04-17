using ClickableTransparentOverlay;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SomeShit.Engine.Core;
using SomeShit.Renderer;

namespace SomeShit.Engine.Renderer;

public class ImGuiRenderer:Overlay
{
    private GameMain _gameMain;
    private Cube _lastaddedcube;
    public ImGuiRenderer(GameMain gameMain)
    {
        _gameMain = gameMain;
    }
    
    protected override void Render()
    {
        ImGuiNET.ImGui.Begin("Renderer");

        if (ImGuiNET.ImGui.BeginTabBar("Tabs"))
        {
            if (ImGuiNET.ImGui.BeginTabItem("Cube"))
            {
                if (ImGuiNET.ImGui.Button("Add Cube"))
                {
                    var cube = new Cube(new Vector3[]
                    {
                        new Vector3(-1, -1, -1),
                        new Vector3(1, -1, -1),
                        new Vector3(1, 1, -1),
                        new Vector3(-1, 1, -1),
                        new Vector3(-1, -1, 1),
                        new Vector3(1, -1, 1),
                        new Vector3(1, 1, 1),
                        new Vector3(-1, 1, 1)
                    });
                    _gameMain.AddShape(cube);
                    _lastaddedcube = cube;
                }

                if (_lastaddedcube != null)
                {
                    float angle = _lastaddedcube.Angle;
                    if (ImGuiNET.ImGui.SliderFloat("Rotation Angle", ref angle, 0.0f, 360.0f))
                    {
                        _lastaddedcube.Angle = angle;
                    }

                    if (ImGuiNET.ImGui.Button("Reset"))
                    {
                        _lastaddedcube = null;
                    }
                }

                ImGuiNET.ImGui.EndTabItem();
            }

            ImGuiNET.ImGui.EndTabBar();
        }

        ImGuiNET.ImGui.End();
    }
}