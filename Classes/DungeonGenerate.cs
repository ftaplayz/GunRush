using System;
using Godot;

namespace GunRush.Classes;
[Tool]
public partial class DungeonGenerate : Node3D
{
	[Export] public Node3D Rooms {get;set;}
	[Export] public int Length {get; private set;}
	[Export] public int Seed { get; set; }
	[Export] public Node3D Dungeon { get; set; } = new Node3D();
	[Export] public bool GenerateButton
	{
		get => this._generateButton;
		set
		{
			if (!this._generateButton)
			{
				this._generateButton = true;
				this.Generate();
				GD.Print("Generating");
				this._generateButton = false;
			}
		}
	}
	
	private bool _generateButton = false;
	private int _roomsCount = 0;
	public override void _Ready(){
		GD.Print(this._CheckRooms());
		this.Generate();
	}
	
	private void _ClearDungeon()
	{
		this.Dungeon?.QueueFree();
		this.Dungeon = new Node3D();
	}
	private bool _CheckRooms()
	{
		if (this.Rooms != null)
		{
			if (this.Rooms.GetChildCount() >= 2)
			{
				var startEnd = this.Rooms.GetNodeOrNull<Node3D>("StartEnd");
				var mid = this.Rooms.GetNodeOrNull<Node3D>("Mid");
				if (startEnd != null && startEnd.GetChildCount() > 0 && mid != null && mid.GetChildCount() > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void _Generate()
	{
		GD.Print("Seed", this.Seed);
		var random = new Random(this.Seed);
		this.Length = random.Next(3, 150);
		GD.Print(this.Length);
		GD.Print(random.Next(0, 100000));
	}
	
	public void Generate()
	{
		if (this._CheckRooms())
		{
			this._ClearDungeon();
			this._Generate();
		}
	}
}
