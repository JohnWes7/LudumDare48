﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config
{
    public static string HoriString { get; } = "Horizontal";
    public static string VerString { get; } = "Vertical";
    public static string StartSceneName { get; } = "StartScene";
    public static string GameSceneName { get; } = "GameScene";
    public static string ESCPanelPath { get; } = "UI/Prefabs/ESCPanel";
    public static string EventInfoJsonPath { get; } = "Json/Events/fallendless_event";
    public static string EventInfoTextPath { get; } = "Json/localisation/FE_day_events_l_simp_chinese_l_english";
    public static string ItemInfoJsonPath { get; } = "Json/Items/fallendless_item";
    public static string PlayerTag { get; } = "Player";
    public static string EventIconPath { get; } = "GameImage/EventIcon/";
    public static string ItemIconPath { get; } = "GameImage/ItemIcon/";

    public enum Language
    {
        l_simp_chinese = 0,
        l_english = 1
    }
}


