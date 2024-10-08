using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class OpenXURL : MonoBehaviour
{
    // Import the JavaScript function using the DllImport attribute
    [DllImport("__Internal")]
    private static extern void openURL(string url);

    // Import the JavaScript function to copy Display Name
    [DllImport("__Internal")]
    private static extern void copyDisplayName(string name);

    // Import the JavaScript function to post on X
    [DllImport("__Internal")]
    private static extern void postToX(string message);

    private const string fixedMessage = "Let's play Dark Machine Battle Front 🔥\n\nInvite a friend and receive 10,000 points 🎁\n\nhttps://t.me/DarkMachineBot";

    public void OnMouseDown()
    {
        // Detect if the mouse is over the circleSpriteRenderer object
        Debug.Log("URL clicked.");
        #if !UNITY_EDITOR && UNITY_WEBGL
        openURL("https://x.com/DarkMachineGame");
        #endif
    }

    public void CopyDisplayNameToClipboard(string displayName)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
        copyDisplayName(displayName);
        #endif
    }

    public void PostToX()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
        postToX(fixedMessage);
        #endif
    }
}
