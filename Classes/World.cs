using Godot;
using System;
using GunRush.Classes;
using GunRush.Models;

public partial class World : Node3D
{
	private Player _player;
	private DungeonGenerate _dungeonGenerate;
	private Global _global;
	private bool _logging = true;
	public override void _Ready()
	{
		this._global = this.GetNode<Global>("/root/Global");
		this._player = this.GetNode<Player>("Player");
		this._dungeonGenerate = this.GetNode<DungeonGenerate>("Dungeon");
		this._player.GlobalPosition = new Vector3(this._dungeonGenerate.Spawn.GlobalPosition.X, this._dungeonGenerate.Spawn.GlobalPosition.Y+5, this._dungeonGenerate.Spawn.GlobalPosition.Z);
		GD.Print("Moved player to spawn");
	}

	public void LogRun()
	{
		var run = new Run();
		run.Kills = this._global.EnemyKills;
		run.Difficulty = this._global.Difficulty;
		run.Seed = this._global.Seed;
		run.Won = !this._global.GameOver;
		run.Duration = (decimal)this._global.GameTime;
		DatabaseHandler.Instance.LogRun(run);
	}

	public override void _Process(double delta)
	{
		if(this._logging) this._global.GameTime += (float)delta;
	}
	
	private void _on_dungeon_dungeon_completed()
	{
		this._logging = false;
		this.LogRun();
		this.QueueFree();
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().ChangeSceneToFile("res://ui/menu.tscn");
	}

}


