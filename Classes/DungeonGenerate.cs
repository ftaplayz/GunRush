using Godot;

namespace GunRush.Classes;
[Tool]
public partial class DungeonGenerate : Node3D
{
	[Export] public Node3D Rooms {get;set;}
	[Export] public int Length {get; set;}
	private int _roomsCount = 0;

    public override void _Ready()
    {
        
    }
    public void Generate(){
	}
}
