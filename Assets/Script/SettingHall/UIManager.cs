using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<string, GameObject> panels;

    void Awake()
    {
        panels = new Dictionary<string, GameObject>();
        Debug.Log("UIManager Awake: Panels initialized.");
    }

    public void RegisterPanel(string name, GameObject panel)
    {
        if (!panels.ContainsKey(name))
        {
            panels.Add(name, panel);
            Debug.Log("Panel registered: " + name);
        }
        else
        {
            Debug.LogError("Panel already registered: " + name);
        }
    }

    public void ShowPanel(string name)
    {
        Debug.Log("ShowPanel called: " + name);
        foreach (var panel in panels.Values)
        {
            panel.SetActive(false);
        }

        if (panels.ContainsKey(name))
        {
            panels[name].SetActive(true);
            Debug.Log("Panel shown: " + name + " | Active state: " + panels[name].activeSelf);
        }
        else
        {
            Debug.LogError("Panel not found: " + name);
        }
    }

    public void HidePanel(string name)
    {
        Debug.Log("HidePanel called: " + name);
        if (panels.ContainsKey(name))
        {
            panels[name].SetActive(false);
            Debug.Log("Panel hidden: " + name);
        }
        else
        {
            Debug.LogError("Panel not found: " + name);
        }
    }
}