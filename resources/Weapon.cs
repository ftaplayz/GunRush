using Godot;
using System;

[GlobalClass, Tool]
public partial class Weapon : Resource
{
	//[ExportCategory("Weapon Properties")]
	[Export] public string Name { get; set; }
	[ExportCategory("Weapon Positioning")]
	[Export] public Vector3 Position { get; set; }
	[Export] public Vector3 Rotation { get; set; }
	[ExportCategory("Weapon Model")]
	[Export] public MeshTransform3D[] MeshList { get; set; }
	[Export] public Vector3 Size {get; set; }
	[ExportCategory("Weapon Stats")]
	[Export] public float Damage { get; set; }
	[Export] public int FireRate{get; set;} // bullets per minute
}