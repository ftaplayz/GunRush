using Godot;

namespace GunRush.Classes;

public partial class Global : Node
{
    public static int EASY_DIFFICULTY = 0;
    public static int MEDIUM_DIFFICULTY = 1;
    public static int HARD_DIFFICULTY = 2;
    public int Difficulty { get; set; } = EASY_DIFFICULTY;
}