using System;
using System.Collections.Generic;
using Godot;

namespace GunRush.Classes;
[Tool]
public partial class DungeonGenerate : Node3D
{
	private static int ROOM_START_END = 0;
	private static int ROOM_MID = 1;

	[Signal] public delegate void DungeonGeneratedEventHandler();

	[Signal] public delegate void DungeonCompletedEventHandler();
	[Export] public Node3D Rooms {get;set;}
	[Export] public int Length {get; private set;}
	[Export] public Node3D Spawn {get;set;}
	[Export] public int Seed { get; set; }
	[Export] public bool Enabled { get; set; }
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

	[Export]
	public CharacterBody3D Enemy { get; set; }
	public int TotalEnemies { get; set; }
	private Enemy _enemy;
	private bool _generateButton = false;
	private List<Node3D> _rooms = new List<Node3D>();
	private int _kills;
	public override void _Ready(){
		Generate();
	}
	
	private void _ClearDungeon()
	{
		foreach(var room in this._rooms)
			room.QueueFree();
		this._rooms.Clear();
		this.Spawn = null;
	}
	private bool _CheckRooms()
	{
		GD.Print("Checking rooms");
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
		GD.Print("No rooms found");
		return false;
	}

	private bool _CheckEnemy()
	{
		GD.Print("Checking for enemy");
		if (this._enemy != null) return true; 
		if (this.Enemy is Enemy)
		{
			GD.Print("Enemy valid");
			
			this._enemy = this.Enemy as Enemy;
			this.Enemy.QueueFree();
			return true;
		}
		return false;
	}

	private void _Generate()
	{
		GD.Print("Seed", this.Seed);
		var zOffset = 0.0f;
		var seedRandom = new Random(this.Seed);
		this.Length = seedRandom.Next(3, 20);
		var startEnd = this.Rooms.GetNode<Node3D>("StartEnd");
		var mid = this.Rooms.GetNode<Node3D>("Mid");
		for(var i=1;i<=this.Length;i++)
		{
			var last = i == this.Length;
			Dungeon room = null;
			if (i == 1 || last)
			{
				room = startEnd.GetChild<Node3D>(seedRandom.Next(0, startEnd.GetChildCount())).Duplicate() as Dungeon;
				if(last)
				{
					room.RotateY(Mathf.DegToRad(180));
					GD.Print("Rotated last room");
				}
				else
					this.Spawn = room.GetNode<Node3D>("Spawn");
			}
			else
			{
				room = mid.GetChild<Node3D>(seedRandom.Next(0, mid.GetChildCount())).Duplicate() as Dungeon;
			}
			this._rooms.Add(room);
			var roomsCount = this._rooms.Count;
			var size = room.GetAabb().Size;
			GD.Print(size);
			var halfZ = size.Z / 2;
			zOffset += halfZ;
			room.Position = new Vector3(0, 0, zOffset);
			zOffset += halfZ;
			/*if(roomsCount >= 1)	
			{
				
				if (roomsCount >= 2)
				{
					this._ConnectRooms(this._rooms[i-2], this._rooms[i-1]);
				}
			}*/
			room.Visible = true;
			this.AddChild(room);
			if (i != 1 && this._CheckEnemy())
			{
				GD.Print("Has enemy");
				//this._enemy.Roams = room.Roams;
				this._enemy.SetRandom(seedRandom);
				GD.Print("Trying to spawn enemy");
				foreach (var spawn in room.Spawns)
				{
					var rand = seedRandom.Next(0, 2);
					GD.Print(rand);
					if (rand == 1)
					{
						TotalEnemies++;
						var enemy = _enemy.Duplicate() as Enemy;
						enemy.Position = spawn.Position;
						enemy.Visible = true;
						room.Enemies.Add(enemy);
						room.AddChild(enemy);
					}
				}

				
				
			}
			
		}

		EmitSignal(SignalName.DungeonGenerated);
		GD.Print("Total enemies ",TotalEnemies);
	}

	/*private void _ConnectRooms(Node3D room1, Node3D room2)
	{
		var room1Type = this._GetRoomType(room1);
		var room2Type = this._GetRoomType(room2);
		if (room1Type == ROOM_MID && room2Type == ROOM_MID)
		{
			GD.Print("Connecting two mid rooms");
		}
		else
		{
			GD.Print("Connecting start/end room with mid room");
		}

	}*/

	private int _GetRoomType(Node3D room)
	{
		return room.GetNodeOrNull<Node3D>("Door") == null ? ROOM_MID : ROOM_START_END;
	}

	
	
	
	public void Generate()
	{
		if (Enabled && this._CheckRooms())
		{
			this._ClearDungeon();
			this._Generate();
		}
	}
	
	private void _on_enemy_died()
	{
		_kills++;
		if(_kills== TotalEnemies)
			EmitSignal(SignalName.DungeonCompleted);
	}
}



