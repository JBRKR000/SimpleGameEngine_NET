using System.IO;
using System.Text;
using ClickableTransparentOverlay;
using ImGuiNET;
using OpenTK.Mathematics;
using SomeShit.Engine.Core;
using SomeShit.Renderer;

public class ImGuiRenderer : Overlay
{
    private GameMain _gameMain;
    private Cube _lastaddedcube;
    private bool autoRotate;
    private string _consoleInput = string.Empty;
    private readonly List<string> _consoleOutput = new();
    private StringWriter _stringWriter;
    private bool _isConsoleVisible = true;

    public ImGuiRenderer(GameMain gameMain)
    {
        _gameMain = gameMain;

        // Przechwytywanie danych z Console.WriteLine
        _stringWriter = new StringWriter();
        Console.SetOut(_stringWriter);
    }

    protected override void Render()
    {
        if (_isConsoleVisible)
        {
            ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.WindowBg, new System.Numerics.Vector4(0f, 0f, 0f, 0.7f));
            ImGuiNET.ImGui.PushStyleVar(ImGuiNET.ImGuiStyleVar.FrameRounding, 2.0f);
            ImGuiNET.ImGui.PushStyleVar(ImGuiNET.ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(4, 3));

            ImGuiNET.ImGui.Begin("Engine Console by JBRKR", ref _isConsoleVisible, ImGuiNET.ImGuiWindowFlags.NoCollapse | ImGuiNET.ImGuiWindowFlags.AlwaysAutoResize);

            // Output (scrollowalny)
            ImGuiNET.ImGui.BeginChild("ScrollingRegion", new System.Numerics.Vector2(800, 400), true, ImGuiNET.ImGuiWindowFlags.HorizontalScrollbar);
            lock (_consoleOutput)
            {
                foreach (var line in _consoleOutput)
                {
                    if (line.StartsWith("> ")) // Komenda użytkownika
                    {
                        ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Text, new System.Numerics.Vector4(1f, 1f, 1f, 1f)); // Biały tekst
                        ImGuiNET.ImGui.TextUnformatted(line);
                        ImGuiNET.ImGui.PopStyleColor();
                    }
                    else // Informacja
                    {
                        ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Text, new System.Numerics.Vector4(0f, 1f, 0f, 1f)); // Zielony tekst
                        ImGuiNET.ImGui.TextUnformatted(line);
                        ImGuiNET.ImGui.PopStyleColor();
                    }
                }
                if (ImGuiNET.ImGui.GetScrollY() >= ImGuiNET.ImGui.GetScrollMaxY())
                    ImGuiNET.ImGui.SetScrollHereY(1.0f); // auto-scroll na dół
            }
            ImGuiNET.ImGui.EndChild();

            ImGuiNET.ImGui.Separator();

            // Pole komend
            ImGuiNET.ImGui.PushItemWidth(700);
            ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Text, new System.Numerics.Vector4(1f, 1f, 1f, 1f)); // Biały tekst dla wpisywanego tekstu
            if (ImGuiNET.ImGui.InputText("##ConsoleInput", ref _consoleInput, 256, ImGuiNET.ImGuiInputTextFlags.EnterReturnsTrue))
            {
                lock (_consoleOutput)
                {
                    _consoleOutput.Add($"> {_consoleInput}"); // Dodanie wpisanej komendy do outputu
                }
                HandleCommand(_consoleInput);
                _consoleInput = string.Empty;

                // Utrzymanie fokusu na polu tekstowym
                ImGuiNET.ImGui.SetKeyboardFocusHere(-1);
            }
            ImGuiNET.ImGui.PopStyleColor();
            ImGuiNET.ImGui.PopItemWidth();

            ImGuiNET.ImGui.PopStyleColor();
            ImGuiNET.ImGui.PopStyleVar(2);
            ImGuiNET.ImGui.End();
        }
        else
        {
            // Przycisk do przywrócenia okna
            ImGuiNET.ImGui.Begin("Console Toggle", ImGuiNET.ImGuiWindowFlags.AlwaysAutoResize | ImGuiNET.ImGuiWindowFlags.NoCollapse);
            if (ImGuiNET.ImGui.Button("Show Console"))
            {
                _isConsoleVisible = true;
            }
            ImGuiNET.ImGui.End();
        }

        UpdateConsoleOutput();
    }

    private void UpdateConsoleOutput()
    {
        lock (_consoleOutput)
        {
            var newOutput = _stringWriter.ToString();
            if (!string.IsNullOrEmpty(newOutput))
            {
                _consoleOutput.AddRange(newOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
                _stringWriter.GetStringBuilder().Clear();
            }
        }
    }

    private void AddCube()
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
        Console.WriteLine("Cube added.");
    }

    private void ResetLastCube()
    {
        if (_lastaddedcube != null)
        {
            autoRotate = false;
            _gameMain.RemoveShapes(_lastaddedcube);
            _lastaddedcube = null;
            Console.WriteLine("Last cube reset.");
        }
    }

    private void ResetAllCubes()
    {
        _gameMain.RemoveAllShapes();
        _lastaddedcube = null;
        Console.WriteLine("All cubes reset.");
    }

    private void ExitApplication()
    {
        Console.WriteLine("Exiting application...");
        Environment.Exit(0);
    }

    private void HandleCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var mainCommand = parts[0].ToLower();

        switch (mainCommand)
        {
            case "cube":
                AddCube();
                break;

            case "help":
                Console.WriteLine("Available commands:");
                Console.WriteLine("  cube - Add a new cube");
                Console.WriteLine("  setangle <value> - Set rotation angle for the last added cube");
                Console.WriteLine("  autorotate <true/false> - Enable/disable autorotate for the last added cube");
                Console.WriteLine("  reset - Remove the last added cube");
                Console.WriteLine("  resetall - Remove all cubes");
                Console.WriteLine("  clear - Clear the console output");
                Console.WriteLine("  exit - Exit the application");
                break;

            case "setangle":
                if (parts.Length > 1 && float.TryParse(parts[1], out var angle))
                {
                    if (_lastaddedcube != null)
                    {
                        _lastaddedcube.Angle = angle;
                        Console.WriteLine($"Rotation angle set to {angle}");
                    }
                    else
                    {
                        Console.WriteLine("No cube to set angle for.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid angle value. Usage: setangle <value>");
                }
                break;

            case "autorotate":
                if (parts.Length > 1 && bool.TryParse(parts[1], out var rotate))
                {
                    if (_lastaddedcube != null)
                    {
                        _lastaddedcube.AutoRotate = rotate;
                        Console.WriteLine($"Autorotate set to {rotate}");
                    }
                    else
                    {
                        Console.WriteLine("No cube to set autorotate for.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid value. Usage: autorotate <true/false>");
                }
                break;

            case "reset":
                ResetLastCube();
                break;

            case "resetall":
                ResetAllCubes();
                break;

            case "exit" or "quit":
                ExitApplication();
                break;
            case "clear":
                lock (_consoleOutput)
                {
                    _consoleOutput.Clear();
                }
                Console.WriteLine("Console cleared.");
                break;

            default:
                Console.WriteLine($"Unknown command: {command}. Type 'help' for a list of commands.");
                break;
        }
    }
}