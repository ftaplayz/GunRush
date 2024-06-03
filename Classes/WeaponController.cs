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
			if(this._weapon != null){
				this._GetEssentialNodes();
				this._InitWeapon();
			}else
				this._ClearWeapon();
		}
	}

	[Export] public int Magazine {get; set;}
	[Export] public int Ammo {get; set;}
	[Export] public int FiredBullets {get; set;}
	private bool _fireCooldown = false;
	private float _msBetweenShot;
	private bool _firing = false;
	private bool _ready = false;
	private Node3D _weaponNode;
	private List<MeshInstance3D> _loadedMeshList = new List<MeshInstance3D>();
	private RayCast3D _ray;
	private AnimationPlayer _animationPlayer;
	private AnimationLibrary _animationLibrary;
	private Node3D _muzzle;
	private string _animationLibraryName = "weapon";
	
	public override void _Ready()
	{
		this._ready = true;
		this._ray = this.GetParent<Node3D>().GetNode<RayCast3D>("AimRay");
		this._GetEssentialNodes();
		this._InitWeapon();
		
	}

	private void _GetEssentialNodes(){
		this._weaponNode = this.GetNode<Node3D>("InnerWeapon");
		this._muzzle = this._weaponNode.GetNode<Node3D>("Muzzle");
		this._animationPlayer = this._weaponNode.GetNode<AnimationPlayer>("AnimationPlayer");
		this._animationLibrary = this._animationPlayer.GetAnimationLibrary(this._animationLibraryName);
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
		this.Ammo = this.Weapon.MaxAmmo;
		this._muzzle.Position = this.Weapon.MuzzlePosition;

		// Weapon Meshes
		foreach(MeshTransform3D mesh in Weapon.MeshList)
			this._AddMeshTransform(mesh);

		// Weapon Privates
		this._msBetweenShot = 1000.0f/(this.Weapon.FireRate/60.0f);
		GD.Print(this._msBetweenShot);
		
		// Weapon Animations
		if(this._ready){
			GD.Print("Adding Animations");
			if(this.Weapon.ShootAnimation != null)
				this._animationLibrary.AddAnimation("Shoot", this.Weapon.ShootAnimation);
			if(this.Weapon.ReloadAnimation != null)
				this._animationLibrary.AddAnimation("Reload", this.Weapon.ReloadAnimation);
		}
		
	}

	public int OnReload(){
		var bulletsToReload = Mathf.Clamp(this.Ammo-(this.Weapon.MagazineSize-this.Magazine), 0, this.Weapon.MagazineSize);
		// 120 - 3 = 117
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
		if(this._animationPlayer != null && this._animationLibrary != null){
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
		this._weaponNode.AddChild(newMesh);
		this._loadedMeshList.Add(newMesh);
	}
	public void Fire(bool firing)
	{	
		if(this._weapon == null) return;
		GD.Print(firing?"Started firing":"Stopped firing");
		this._firing = firing;
		this._Fire();
	}

	private async void _Fire(){
		GD.Print("Trying to fire");	
		if(this.Weapon == null || this.Magazine <= 0 || !this._firing || this._fireCooldown)
			return;
		GD.Print("Firing");
		if(this._animationLibrary.HasAnimation("Shoot")){
			GD.Print("Has Animation");
			this._animationPlayer.Stop();
			this._animationPlayer.Play(this._animationLibraryName+"/Shoot");
		}
		this._ray.ForceRaycastUpdate();
		if(this._ray.IsColliding()){
			var colliding = this._ray.GetCollider();
			this._ray.GetCollisionPoint();
			GD.Print(colliding);
		}
		this.FiredBullets++;
		this.Magazine--;
		this._fireCooldown = true;
		await Task.Delay((int)Mathf.Round(this._msBetweenShot));
		this._fireCooldown = false;
		if(this._weapon.Automatic)
			this._Fire();
	}
}
