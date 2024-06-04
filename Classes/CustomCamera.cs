using System.Collections.Generic;
using Godot;
using Microsoft.VisualBasic;

namespace GunRush.Classes;

public partial class CustomCamera : Node3D
{
	[Export] public float MaxUpwardsRotation {get; set;} = 89;
	[Export] public float MaxDownwardsRotation {get; set;} = -89;
	private IDictionary<string, bool> _lerping = new Dictionary<string, bool>(){{"X", false},{"Y", false},{"Z", false}};
	private Vector3 _newRotation = new Vector3();
	private Vector3 _lerpSpeed = new Vector3();

	public new void RotateX(float angle){
		var rotation = this.RotationDegrees.X+angle;
		rotation = Mathf.Clamp(rotation, this.MaxDownwardsRotation, this.MaxUpwardsRotation);
		this.RotationDegrees = new Vector3(rotation, this.RotationDegrees.Y, this.RotationDegrees.Z);
	}

	public void RotateX(float angle, float speed){
		this._lerping["X"] = true;
		this._newRotation.X = Mathf.Clamp(this.RotationDegrees.X+angle, this.MaxDownwardsRotation, this.MaxUpwardsRotation);
		this._lerpSpeed.X = speed;
	}
}
