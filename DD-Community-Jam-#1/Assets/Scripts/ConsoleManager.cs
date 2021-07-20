using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager instance;

    public TMP_Text buffer;
    public TMP_InputField input;

    public int linesCount;
    private string[] lines;

    private uint pointer;
    public bool isConsole;
    private bool cheatsEnabled;

    [Header("Command References")]
    public GameObject globalLight;
    public Transform player;

    void Awake() => instance = this;

    void Start()
    {
        lines = new string[linesCount];
        pointer = 0;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.C))
            isConsole = !isConsole;

        if (isConsole)
            transform.GetChild(0).gameObject.SetActive(true);
        else
            transform.GetChild(0).gameObject.SetActive(false);

        
    }

    public void UnderstandCommand(string command)
    {
        if (!command.EndsWith(";"))
            return;

        HandleCommand(command);

        UpdateText();
        input.text = "";
    }

    void WriteLine(string text)
    {
        if (pointer <= linesCount - 1)
        {
            lines[pointer] = text + "\n";
            pointer++;
        } else
        {
            for (int i = 1; i < linesCount - 1; i++)
            {
                lines[i - 1] = lines[i];
            }
            lines[linesCount - 1] = text;
        }
    }

    void UpdateText()
    {
        string final = "";

        for (int i = 0; i < lines.Length; i++)
        {
            final += lines[i];
        }

        buffer.text = final;
    }

    void HandleCommand(string input)
    {
        if (input.ToLower() == "sv_cheats 1;" && !cheatsEnabled)
        {
            cheatsEnabled = true;
            WriteLine($"<color=yellow>CHEATS ARE NOW ENABLED");
            return;
        }

        if (!cheatsEnabled)
            WriteLine($"<color=red>Cheats are dissabled.");
        if (input.ToLower() == "light;")
        {
            LightToggle();
            return;
        }
        else if (input.ToLower().StartsWith("chunktp"))
        {
            string[] args = input.ToLower().Split(' ');

            if(args.Length != 3)
            {
                WriteLine($"<color=yellow>\"{input}\"<color=red> has an error.");
                return;
            }

            int x = 0;
            int y = 0;

            try
            {
                x = int.Parse(args[1]);
            } catch
            {
                WriteLine($"<color=red>[{args[1]}]<color=yellow> is not a valid number.");
                return;
            }

            try
            {
                y = int.Parse(args[2].Remove(args[2].Length - 1));
            }
            catch
            {
                WriteLine($"<color=red>[{args[2].Remove(args[2].Length - 1)}]<color=yellow> is not a valid number.");
                return;
            }

            ChunkTP(x, y);
            return;
        }
        else if (input.ToLower() == "god;")
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            health.godMode = !health.godMode;
            WriteLine($"<color=yellow>Godmode is now {(health.godMode ? "<color=green>[enabled]" : "<color=red>[dissabled]")}<color=yellow>.");
            return;
        }
        else if (input.ToLower() == "heal;")
        {
            player.GetComponent<PlayerHealth>().Heal();
            return;
        }
        else if (input.ToLower().StartsWith("up"))
        {
            string[] args = input.ToLower().Split(' ');
            if (args.Length != 2)
                return;

            int by = 0;

            try
            {
                by = int.Parse(args[1].Remove(args[1].Length - 1));
            }
            catch
            {
                WriteLine($"<color=red>[{args[1].Remove(args[1].Length - 1)}]<color=yellow> is not a valid number.");
                return;
            }

            player.GetComponent<PlayerHealth>().UpgradeHealth(by);
            return;
        }
        else if (input.ToLower() == "help;")
        {
            WriteLine($"<color=yellow>light;\nchunktp (x:int) (y:int);\ngod;\nheal;\nup (number:int);\nhelp;");
            return;
        }
        else
        {
            WriteLine($"<color=yellow>\"{input}\"<color=red> is not a valid command.");
        }
    }

    void LightToggle()
    {
        globalLight.SetActive(!globalLight.activeInHierarchy);
        WriteLine($"<color=yellow>Full lights are now {(globalLight.activeInHierarchy ? "<color=green>[enabled]" : "<color=red>[dissabled]")}<color=yellow>.");
    }

    void ChunkTP(int x, int y)
    {
        player.position = new Vector2(x * 50, y * 50);
    }
}
