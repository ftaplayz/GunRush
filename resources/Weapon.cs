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
	[Export] public Vector3 MuzzlePosition {get; set;}
	[ExportCategory("Weapon Model")]
	[Export] public MeshTransform3D[] MeshList { get; set; }
	[Export] public Vector3 Size {get; set; }
	
	[ExportCategory("Weapon Animation")]
	[Export] public Animation ShootAnimation {get; set;}
	[Export] public Animation ReloadAnimation {get; set;}
	[ExportCategory("Weapon Stats")]
	[Export] public float Damage { get; set; }
	[Export] public int FireRate{get; set;} // bullets per minute
	[Export] public bool Automatic {get; set;}
	[Export] public int MagazineSize {get; set;}
	[Export] public int MaxAmmo {get; set;}
	//[Export] public float ReloadTime {get; set;}

}
