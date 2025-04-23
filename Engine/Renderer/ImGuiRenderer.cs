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
    private Sphere _lastaddedsphere;
    private bool autoRotate;
    private string _consoleInput = string.Empty;
    private readonly List<string> _consoleOutput = new();
    private StringWriter _stringWriter;
    private bool _isConsoleVisible = true;

    public ImGuiRenderer(GameMain gameMain)
    {
        _gameMain = gameMain;
        _stringWriter = new StringWriter();
        Console.SetOut(_stringWriter);
    }

    private readonly List<string> _availableCommands = new()
{
    "sp_create_sphere",
    "sp_rotate",
    "sp_delete",
    "cb_create_cube",
    "cb_move",
    "cb_setangle",
    "cb_autorotate",
    "cb_reset",
    "cb_resetall",
    "clear",
    "exit",
    "help",
    "sp_autorotate"
};

private List<string> _filteredCommands = new();

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

        // Pole komend z podpowiedziami
        ImGuiNET.ImGui.PushItemWidth(700);
        ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Text, new System.Numerics.Vector4(1f, 1f, 1f, 1f)); // Biały tekst dla wpisywanego tekstu
        if (ImGuiNET.ImGui.InputTextWithHint("##ConsoleInput", "Type a command...", ref _consoleInput, 256, ImGuiNET.ImGuiInputTextFlags.EnterReturnsTrue))
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

        // Filtruj podpowiedzi
        _filteredCommands = _availableCommands
            .Where(cmd => cmd.StartsWith(_consoleInput, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Wyświetl listę rozwijaną z podpowiedziami
        if (_filteredCommands.Count > 0 && !string.IsNullOrEmpty(_consoleInput))
        {
            if (ImGuiNET.ImGui.BeginListBox("##Suggestions", new System.Numerics.Vector2(700, 100)))
            {
                foreach (var suggestion in _filteredCommands)
                {
                    if (ImGuiNET.ImGui.Selectable(suggestion))
                    {
                        _consoleInput = suggestion; // Wybierz podpowiedź
                        ImGuiNET.ImGui.SetKeyboardFocusHere(-1); // Utrzymaj fokus
                    }
                }
                ImGuiNET.ImGui.EndListBox();
            }
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

    private void AddSphere()
    {
        var sphere = new Sphere(1.0f, 20, 20);
        _gameMain.AddShape(sphere);
        _lastaddedsphere = sphere;
        Console.WriteLine("Sphere added.");
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

    private void RotateSphere(float angleX, float angleY, float angleZ)
    {
        if (_lastaddedsphere != null)
        {
            _lastaddedsphere.Rotate(angleX, angleY, angleZ);
            Console.WriteLine($"Sphere rotated by ({angleX}, {angleY}, {angleZ}).");
        }
        else
        {
            Console.WriteLine("No sphere to rotate.");
        }
    }

    private void DeleteSphere()
    {
        if (_lastaddedsphere != null)
        {
            _gameMain.RemoveShapes(_lastaddedsphere);
            _lastaddedsphere = null;
            Console.WriteLine("Sphere deleted.");
        }
        else
        {
            Console.WriteLine("No sphere to delete.");
        }
    }

    private void HandleCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var mainCommand = parts[0].ToLower();

        switch (mainCommand)
        {
            case "sp_create_sphere":
                AddSphere();
                break;
            
            case "cb_create_cube":
                AddCube();
                break;

            case "help":
                Console.WriteLine("Available commands:");
                Console.WriteLine("  sp_create_sphere - Add a new sphere");
                Console.WriteLine("  sp_rotate <angleX> <angleY> <angleZ> - Rotate the last added sphere");
                Console.WriteLine("  sp_delete - Delete the last added sphere");
                Console.WriteLine("  cb_create_cube - Add a new cube");
                Console.WriteLine("  cb_move <x y z> - Move the last added cube");
                Console.WriteLine("  cb_setangle <value> - Set rotation angle for the last added cube");
                Console.WriteLine("  cb_autorotate <true/false> - Enable/disable autorotate for the last added cube");
                Console.WriteLine("  cb_reset - Remove the last added cube");
                Console.WriteLine("  cb_resetall - Remove all cubes");
                Console.WriteLine("  clear - Clear the console output");
                Console.WriteLine("  exit - Exit the application");
                break;

            case "cb_setangle":
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

            case "cb_move":
                if (parts.Length > 3 && int.TryParse(parts[1], out var x) && int.TryParse(parts[2], out var y) && int.TryParse(parts[3], out var z))
                {
                    if (_lastaddedcube != null)
                    {
                        _lastaddedcube.TranslateCube(x, y, z);
                        Console.WriteLine($"Cube moved to ({x}, {y}, {z})");
                    }
                    else
                    {
                        Console.WriteLine("No cube to move.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid coordinates. Usage: move <x> <y> <z>");
                }
                break;
            
            case "cb_autorotate":
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

            case "cb_reset":
                ResetLastCube();
                break;

            case "cb_resetall":
                ResetAllCubes();
                break;

            case "sp_rotate":
                if (parts.Length > 3 &&
                    float.TryParse(parts[1], out var angleX) &&
                    float.TryParse(parts[2], out var angleY) &&
                    float.TryParse(parts[3], out var angleZ))
                {
                    RotateSphere(angleX, angleY, angleZ);
                }
                else
                {
                    Console.WriteLine("Invalid rotation values. Usage: sp_rotate <angleX> <angleY> <angleZ>");
                }
                break;

            case "sp_delete":
                DeleteSphere();
                break;

            case "sp_autorotate":
                if (parts.Length > 1 && bool.TryParse(parts[1], out var autorotate))
                {
                    if (_lastaddedsphere != null)
                    {
                        _lastaddedsphere.SetAutoRotate(autorotate);
                        Console.WriteLine($"Sphere autorotate set to {autorotate}");
                    }
                    else
                    {
                        Console.WriteLine("No sphere to set autorotate for.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid value. Usage: sp_autorotate <true/false>");
                }
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

