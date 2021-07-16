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

        WriteLine($"<color=yellow>\"{command}\"<color=red> is not a valid command.");

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
}
