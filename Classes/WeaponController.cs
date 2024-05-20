using System;
using Godot;
namespace GunRush.Classes;
[Tool]
public partial class WeaponController : Node3D
{
	private Weapon _weapon;
	[Export] public int FireRate {get; set;}
	[Export] public Weapon Weapon
	{
		get
		{
			return this._weapon;
		}
		set
		{
			this._weapon = value;
			if(Engine.IsEditorHint())
				this._InitWeapon();
		}
	}

	private long _lastShootTime;
	private long _msBetweenShot;
	private MeshInstance3D _mesh;
	private RayCast3D _ray;
	public override void _Ready()
	{
		this._ray = this.GetParent<Node3D>().GetNode<RayCast3D>("AimRay");
		this._InitWeapon();
	}

	private void _InitWeapon(){
		this._mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		this._LoadWeapon();
	}

	private void _LoadWeapon()
	{
		if (this.Weapon == null)
		{
			this._mesh.Mesh = null;
			return;
		}
		this.Scale = this.Weapon.Size;
		this.Position = this.Weapon.Position;
		this.RotationDegrees = this.Weapon.Rotation;
		this._mesh.Mesh = this.Weapon.Mesh;
		this.FireRate = this.Weapon.FireRate;
		this._msBetweenShot = 1000/(this.Weapon.FireRate/60);
		
	}

	public bool Fire()
	{
		var currentMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		if(this._lastShootTime+this._msBetweenShot <= currentMs){
			this._ray.ForceRaycastUpdate();
			if(this._ray.IsColliding()){
				var colliding = this._ray.GetCollider();
				this._ray.GetCollisionPoint();
				GD.Print(colliding);
				return true;
			}
			this._lastShootTime = currentMs;
		}
		
		return false;
	}
}
