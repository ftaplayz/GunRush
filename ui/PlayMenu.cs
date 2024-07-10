using Godot;
using System;
using GunRush.Classes;

public partial class PlayMenu : Control
{
	private LineEdit _seedInput;
	private Button _startButton;
	private Button _hardButton;
	private Button _normalButton;
	private Button _easyButton;
	private int _seed;
	private Random _random;
	private Global _global;

	public override void _Ready()
	{
		ResourceLoader.LoadThreadedRequest("res://scene/world/world.tscn");
		this._global = this.GetNode<Global>("/root/Global");
		this._random = new Random();
		this._seed = _random.Next(int.MinValue, int.MaxValue);
		this._seedInput = this.GetNode<LineEdit>("LevelSettings/VBoxContainer/Seed/HBoxContainer/SeedEdit");
		this._startButton = this.GetNode<Button>("StartButton");
		this._hardButton = this.GetNode<Button>("LevelSettings/VBoxContainer/Difficulties/HardButton");
		this._normalButton = this.GetNode<Button>("LevelSettings/VBoxContainer/Difficulties/NormalButton");
		this._easyButton = this.GetNode<Button>("LevelSettings/VBoxContainer/Difficulties/EasyButton");
		/*this._worldScene = ResourceLoader.LoadThreadedGet("res://scene/world/world.tscn") as PackedScene;
		this._node3D = this._worldScene.Instantiate() as Node3D;
		GD.Print(this._node3D.Name);*/
	}
	
	private void _Play()
	{
		var worldScene = ResourceLoader.LoadThreadedGet("res://scene/world/world.tscn") as PackedScene;
		var worldNode = worldScene.Instantiate() as Node3D;
		var dungeonGenerate = worldNode.GetNode<DungeonGenerate>("Dungeon");
		dungeonGenerate.Seed = this._seed;
		_global.Seed = this._seed;
		_global.GameOver = false;
		_global.EnemyKills = 0;
		_global.GameTime = 0;
		
		dungeonGenerate.Enabled = true;
		//dungeonGenerate.Generate();
		this.GetNode("/root/")?.AddChild(worldNode);
		this.QueueFree();
		GD.Print("Loaded");
	}

	private void _SetSeed(string text)
	{
		try
		{
			this._seed = int.Parse(text);
		}
		catch (Exception e)
		{
			this._seedInput.Text = "";
			this._seed = this._random.Next(int.MinValue, int.MaxValue);
		}
		GD.Print(_seed);
	}

	

	private void _SetDifficulty(int diff)
	{
		this._global.Difficulty = diff;
		GD.Print("Difficulty set to " + diff);
	}

	private void _Hard()
	{
		this._SetDifficulty(2);
	}
	private void _Normal()
	{
		this._SetDifficulty(1);
	}
	private void _Easy()
	{
		this._SetDifficulty(0);
	}
}
