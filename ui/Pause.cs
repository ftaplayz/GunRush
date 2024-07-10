using Godot;
using System;
using GunRush.Classes;

public partial class Pause : Control
{
	private Button _resume;
	private Button _giveUp;
	private Player _player;
	private World _world;
	public override void _Ready()
	{
		this._resume = this.GetNode<Button>("VBoxContainer/ResumeButton");
		this._giveUp = this.GetNode<Button>("VBoxContainer/GiveUpButton");
		if(this.Owner is Player)
		{
			this._player = this.Owner as Player;
			this._world = this._player.Owner as World;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if(Input.IsActionJustPressed("pause"))
		{
			this._player.JustPaused = true;
			State(!GetTree().Paused);
		}
	}

	public void State(bool pause)
	{
		GD.Print("Trying to ", pause?"pause":"unpause");
		this._player.GetNode<Control>("Crosshair").Visible = !pause;
		this._player.GetNode<Control>("KillsCounter").Visible = !pause;
		this._player.GetNode<Control>("Health").Visible = !pause;
		this._player.GetNode<Control>("Ammo").Visible = !pause;
		this._player.GetNode<Control>("pause").Visible = pause;
		Input.MouseMode = pause ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		GetTree().Paused = pause;
	}

	private void _Resume()
	{
		State(false);
	}

	private void _GiveUp()
	{
		GetTree().Paused = false;
		this._OnEnd();
		this._world.QueueFree();
		GetTree().ChangeSceneToFile("res://ui/menu.tscn");
	}

	public override void _Notification(int what)
	{
		if (what==NotificationWMCloseRequest)
			this._OnEnd();
	}

	private void _OnEnd()
	{
		this.GetNode<Global>("/root/Global").GameOver = true;
		this._world.LogRun();
	}
}
