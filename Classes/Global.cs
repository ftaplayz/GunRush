using Godot;

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
}