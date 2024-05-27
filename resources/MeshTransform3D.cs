using Godot;
using System;

[GlobalClass, Tool]
public partial class MeshTransform3D : Resource
{
    [Export] public Mesh Mesh {get; set;}
    [ExportCategory("Mesh Transform")]
    [Export] public Vector3 Position { get; set; }
	[Export] public Vector3 Rotation { get; set; }
    [Export] public Vector3 Scale {get; set;}
}
