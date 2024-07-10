using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace GunRush.Classes;

public partial class Global : Node
{
    public static int EASY_DIFFICULTY = 0;
    public static int MEDIUM_DIFFICULTY = 1;
    public static int HARD_DIFFICULTY = 2;
    public int Difficulty { get; set; } = EASY_DIFFICULTY;
    public float Sensitivity { get; set; } = 0.5f;
    public bool Crosshair {get;set;} = true;
    public int CrosshairWidth { get; set; } = 2;
    public float CrosshairLength {get;set;} = 10.0f;
    public float CrosshairCenterOffset {get;set;} = 5.0f;
    public Color CrosshairColor {get;set;} = Colors.White;
    public bool CrosshairDot {get;set;} = false;
    public float CrosshairDotSize {get;set;} = 10.0f;
    public bool Fullscreen { get; set; } = false;
    public string Locale { get; set; } = "en";

    private Dictionary _keys = new Dictionary
    {
        {"sensitivity", "sensitivity"},
        {"fullscreen", "fullscreen"},
        {"locale", "locale"},
        {"crosshair", "crosshair"},
        {"crosshair_width", "crosshair_width"},
        {"crosshair_length", "crosshair_length"},
        {"crosshair_center_offset", "crosshair_center_offset"},
        {"crosshair_color", "crosshair_color"},
        {"crosshair_dot", "crosshair_dot"},
        {"crosshair_dot_size", "crosshair_dot_size"},
    };
    
    private string _configPath = "user://config.cfg";

    public int EnemyKills = 0;
    public bool GameOver = false;
    public float GameTime = 0.0f;
    public int Seed = 0;

    public override void _EnterTree()
    {
        this._LoadConfig();
    }

    public override void _ExitTree()
    {
        this._SaveConfig();
    }

    private void _LoadConfig()
    {
        var config = new ConfigFile();
        Error err = config.Load(_configPath);
        if(err != Error.Ok) return;
        GD.Print("Loading config");
        this.Sensitivity = (float)config.GetValue("general", (string)_keys["sensitivity"]);
        this.Fullscreen = (bool)config.GetValue("general", (string)_keys["fullscreen"]);
        this.Locale = (string)config.GetValue("general", (string)_keys["locale"]);
        this.Crosshair = (bool)config.GetValue("crosshair", (string)_keys["crosshair"]);
        this.CrosshairWidth = (int)config.GetValue("crosshair", (string)_keys["crosshair_width"]);
        this.CrosshairLength = (float)config.GetValue("crosshair", (string)_keys["crosshair_length"]);
        this.CrosshairCenterOffset = (float)config.GetValue("crosshair", (string)_keys["crosshair_center_offset"]);
        this.CrosshairColor = (Color)config.GetValue("crosshair", (string)_keys["crosshair_color"]);
        this.CrosshairDot = (bool)config.GetValue("crosshair", (string)_keys["crosshair_dot"]);
        this.CrosshairDotSize = (float)config.GetValue("crosshair", (string)_keys["crosshair_dot_size"]);
    }

    private void _SaveConfig()
    {
        GD.Print("Saving config");
        var config = new ConfigFile();
        config.SetValue("general", (string)_keys["sensitivity"], this.Sensitivity);
        config.SetValue("general", (string)_keys["fullscreen"], this.Fullscreen);
        config.SetValue("general", (string)_keys["locale"], this.Locale);
        config.SetValue("crosshair", (string)_keys["crosshair"], this.Crosshair);
        config.SetValue("crosshair", (string)_keys["crosshair_width"], this.CrosshairWidth);
        config.SetValue("crosshair", (string)_keys["crosshair_length"], this.CrosshairLength);
        config.SetValue("crosshair", (string)_keys["crosshair_center_offset"], this.CrosshairCenterOffset);
        config.SetValue("crosshair", (string)_keys["crosshair_color"], this.CrosshairColor);
        config.SetValue("crosshair", (string)_keys["crosshair_dot"], this.CrosshairDot);
        config.SetValue("crosshair", (string)_keys["crosshair_dot_size"], this.CrosshairDotSize);
        config.Save(_configPath);
    }
}