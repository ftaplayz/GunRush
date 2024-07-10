using Godot;
using System;

public partial class ReturnToMenu : Button
{
	public override void _Pressed()
	{
		this.GetTree().ChangeSceneToFile("res://ui/menu.tscn");
	}
}
