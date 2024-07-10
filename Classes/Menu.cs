using Godot;
using System;
using GunRush.Classes;

public partial class Menu : Control
{
	private Global _global;
	public override void _EnterTree()
	{
		this._global = this.GetNode<Global>("/root/Global");
		TranslationServer.SetLocale(this._global.Locale);
		DisplayServer.WindowSetMode(this._global.Fullscreen?DisplayServer.WindowMode.Fullscreen:DisplayServer.WindowMode.Windowed);
	}

	private void _OnSettings()
	{
		this.GetTree().ChangeSceneToFile("res://ui/settings.tscn");
	}


	private void _OnStats()
	{
		this.GetTree().ChangeSceneToFile("res://ui/stats.tscn");
	}


	private void _OnPlay()
	{
		this.GetTree().ChangeSceneToFile("res://ui/play.tscn");
	}
	
	private void _OnQuit()
	{
		this.GetTree().Quit();
	}

}





