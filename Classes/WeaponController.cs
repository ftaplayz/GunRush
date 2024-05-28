using System;
using System.Collections;
using System.Collections.Generic;
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
	private List<MeshInstance3D> _loadedMeshList = new List<MeshInstance3D>();
	private RayCast3D _ray;
	private AnimationPlayer _animationPlayer;
	private AnimationLibrary _animationLibrary;
	
	public override void _Ready()
	{
		this._ray = this.GetParent<Node3D>().GetNode<RayCast3D>("AimRay");
		this._animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
		this._animationLibrary = this._animationPlayer.GetAnimationLibrary("weapon");
		this._InitWeapon();
	}

	private void _InitWeapon(){
		this._ClearWeapon();
		this._LoadWeapon();
	}

	private void _LoadWeapon()
	{
		if(this.Weapon == null) return;
		// Weapon Transform
		this.Scale = this.Weapon.Size;
		this.Position = this.Weapon.Position;
		this.RotationDegrees = this.Weapon.Rotation;

		// Weapon Properties
		this.Magazine = this.Weapon.MagazineSize;

		// Weapon Meshes
		foreach(MeshTransform3D mesh in Weapon.MeshList)
			this._AddMeshTransform(mesh);
		
		// Weapon Privates
		this._msBetweenShot = 1000/(this.Weapon.FireRate/60);
		
	}

	public int OnReload(){
		var bulletsToReload = Mathf.Clamp(this.Ammo-this.FiredBullets, 0, this.Weapon.MagazineSize);
		this.Ammo -= bulletsToReload;
		this.Magazine = bulletsToReload;
		return bulletsToReload;
	}

	private void _ClearWeapon(){
		if(this._loadedMeshList.Count > 0){
			foreach(MeshInstance3D mesh in this._loadedMeshList){
				GD.Print(mesh);
				mesh.QueueFree();
			}
			this._loadedMeshList.Clear();
		}
		if(this._animationPlayer != null){
			if(this._animationLibrary.HasAnimation("Shoot"))
				this._animationLibrary.RemoveAnimation("Shoot");
			if(this._animationLibrary.HasAnimation("Reload"))
				this._animationLibrary.RemoveAnimation("Reload");
		}
	}

	private void _AddMeshTransform(MeshTransform3D mesh){
		var newMesh = new MeshInstance3D();
		newMesh.Mesh = mesh.Mesh;
		newMesh.Position = mesh.Position;
		newMesh.RotationDegrees = mesh.Rotation;
		newMesh.Scale = mesh.Scale;
		this.AddChild(newMesh);
		this._loadedMeshList.Add(newMesh);
	}
	public void Fire(bool firing)
	{	
		this._firing = firing;
		this._Fire();
	}

	private async void _Fire(){
		if(this.Weapon == null || this.Magazine <= 0 || !this._firing)
			return;
		this._animationPlayer.Play("Shoot");
		this._ray.ForceRaycastUpdate();
		if(this._ray.IsColliding()){
			var colliding = this._ray.GetCollider();
			this._ray.GetCollisionPoint();
			GD.Print(colliding);
		}
		this.Magazine--;
		await Task.Delay(this._msBetweenShot);
		this._Fire();
	}
}
