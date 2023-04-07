using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Basic launch command processor (Multiplay prefers passing IP and port along)
/// </summary>
static public class ApplicationStartupData
{
    /// <summary>
    /// Commands Dictionary
    /// Supports flags and single variable args (eg. '-argument', '-variableArg variable')
    /// </summary>
    static private Dictionary<string, Action<string>> m_commandDictionary = new();

    static private readonly string k_ipCmd = "ip";
    static private readonly string k_portCmd = "port";
    static private readonly string k_queryPortCmd = "queryPort";

    //Ensure this gets instantiated Early on
    static public void InitializeApplicationData()
    {
        SetIP("127.0.0.1");
        SetPort("7777");
        SetQueryPort("7787");

        m_commandDictionary["-" + k_ipCmd] = SetIP;
        m_commandDictionary["-" + k_portCmd] = SetPort;
        m_commandDictionary["-" + k_queryPortCmd] = SetQueryPort;

        ProcessCommandLinearguments(Environment.GetCommandLineArgs());
    }

    static public string IP()
    {
        return PlayerPrefs.GetString(k_ipCmd);
    }

    static public int Port()
    {
        return PlayerPrefs.GetInt(k_portCmd);
    }

    static public int QPort()
    {
        return PlayerPrefs.GetInt(k_queryPortCmd);
    }

    static private void ProcessCommandLinearguments(string[] args)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Launch Args: ");

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var nextArg = string.Empty;

            // if we are evaluating the last item in the array, it must be a flag
            if (i + 1 < args.Length)
                nextArg = args[i + 1];

            if (EvaluatedArgs(arg, nextArg))
            {
                stringBuilder.Append(arg);
                stringBuilder.Append(" : ");
                stringBuilder.AppendLine(nextArg);

                i++;
            }
        }

        Debug.Log(stringBuilder);
    }

    /// <summary>
    /// Commands and values come in the args array in pairs, so we
    /// </summary>
    static private bool EvaluatedArgs(string arg, string nextArg)
    {
        if (!IsCommand(arg))
            return false;

        if (IsCommand(nextArg)) // If you have need for flags, make a separate dict for those.
        {
            return false;
        }

        m_commandDictionary[arg].Invoke(nextArg);
        return true;
    }

    static private void SetIP(string ipArgument)
    {
        PlayerPrefs.SetString(k_ipCmd, ipArgument);
    }

    static private void SetPort(string portArgument)
    {
        if (int.TryParse(portArgument, out int parsedPort))
        {
            PlayerPrefs.SetInt(k_portCmd, parsedPort);
        }
        else
        {
            Debug.LogError($"{portArgument} does not contain a parseable port!");
        }
    }

    static private void SetQueryPort(string qPortArgument)
    {
        if (int.TryParse(qPortArgument, out int parsedQPort))
        {
            PlayerPrefs.SetInt(k_queryPortCmd, parsedQPort);
        }
        else
        {
            Debug.LogError($"{qPortArgument} does not contain a parseable query port!");
        }
    }

    static private bool IsCommand(string arg)
    {
        return !string.IsNullOrEmpty(arg) &&
            m_commandDictionary.ContainsKey(arg) &&
            arg.StartsWith("-");
    }
}