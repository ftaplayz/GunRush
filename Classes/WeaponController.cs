using System;
using System.Collections;
using System.Threading.Tasks;
using Godot;
namespace GunRush.Classes;
[Tool]
public partial class WeaponController : Node3D
{
	private Weapon _weapon;
	[Export] public Weapon Weapon
	{
		get
		{
			return this._weapon;
		}
		set
		{
			this._weapon = value;
			//if(Engine.IsEditorHint())
			this._InitWeapon();
		}
	}

	[Export] public int Magazine {get; set;}

	[Export] public int Ammo {get; set;}
	[Export] public int FiredBullets {get; set;}
	private int _msBetweenShot;
	private bool _firing = false;
	private Node3D _meshFolder;
	private RayCast3D _ray;
	public override void _Ready()
	{
		this._ray = this.GetParent<Node3D>().GetNode<RayCast3D>("AimRay");
		this._InitWeapon();
	}

	private void _InitWeapon(){
		this._LoadWeapon();
	}

	private void _LoadWeapon()
	{
		if (this.Weapon == null)
		{
			if(this._meshFolder != null)
				this.RemoveChild(this._meshFolder);
			this.Scale = Vector3.Zero;
			return;
		}
		this._meshFolder = new Node3D();
		this.Scale = this.Weapon.Size;
		this.Position = this.Weapon.Position;
		this.RotationDegrees = this.Weapon.Rotation;
		foreach(MeshTransform3D mesh in Weapon.MeshList){
			var newMesh = new MeshInstance3D();
			newMesh.Mesh = mesh.Mesh;
			newMesh.Position = mesh.Position;
			newMesh.RotationDegrees = mesh.Rotation;
			newMesh.Scale = mesh.Scale;
			this._meshFolder.AddChild(newMesh);
		}
		this.AddChild(this._meshFolder);
		this._msBetweenShot = 1000/(this.Weapon.FireRate/60);
	}

	public int OnReload(){
		var bulletsToReload = Mathf.Clamp(this.Ammo-this.FiredBullets, 0, this.Weapon.MagazineSize);
		this.Ammo -= bulletsToReload;
		this.Magazine = bulletsToReload;
		return bulletsToReload;
	}

	public async void Fire(bool firing, bool functionEvoked = false)
	{	
		if(!functionEvoked)
			this._firing = firing;
		if(this.Weapon == null || this.Magazine <= 0 || !this._firing)
			return;
		this._ray.ForceRaycastUpdate();
		if(this._ray.IsColliding()){
			var colliding = this._ray.GetCollider();
			this._ray.GetCollisionPoint();
			GD.Print(colliding);
		}
		this.Magazine--;
		await Task.Delay(this._msBetweenShot);
		Fire(this._firing, true);
	}
}
