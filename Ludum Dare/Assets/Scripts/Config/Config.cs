using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static string HoriString { get; } = "Horizontal";
    public static string VerString { get; } = "Vertical";
    public static string StartSceneName { get; } = "StartScene";
    public static string GameSceneName { get; } = "GameScene";
    public static string ESCPanelPath { get; } = "Prefabs/ESCPanel";
    public static string EventInfoJsonPath { get; } = "Json/fallendless_event";
}
