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
	[Export] public RayCast3D WeaponRaycast { get; set; }
	public bool AutoReload { get; set; } = false;
	private bool _fireCooldown = false;
	private float _msBetweenShot;
	private bool _firing = false;
	private bool _reloading = false;
	private bool _ready = false;
	private Node3D _weaponNode;
	private List<MeshInstance3D> _loadedMeshList = new List<MeshInstance3D>();
	private Random _random;
	private AnimationPlayer _animationPlayer;
	private AnimationLibrary _animationLibrary;
	private Node3D _muzzle;
	private CustomCamera _camera;
	private GpuParticles3D _muzzleFlash;
	private string _animationLibraryName = "weapon";
	
	public override void _Ready()
	{
		this._ready = true;
		this._camera = this.GetParentOrNull<CustomCamera>();
		//this.WeaponRaycast = this.GetParent<Node3D>().GetNode<RayCast3D>("AimRay");
		this._GetEssentialNodes();
		this._muzzleFlash = this._muzzle.GetNode<GpuParticles3D>("GPUParticles3D");
		this._InitWeapon();
		this._random = new Random();

	}

	private void _GetEssentialNodes(){
		this._weaponNode = this.GetNode<Node3D>("InnerWeapon");
		this._muzzle = this._weaponNode.GetNode<Node3D>("Muzzle");
		this._animationPlayer = this._weaponNode.GetNode<AnimationPlayer>("AnimationPlayer");
		this._animationLibrary = this._animationPlayer.GetAnimationLibrary(this._animationLibraryName);
	}

	private void _InitWeapon(){
		GD.Print("Weapon init");
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
		if(Weapon.MeshList != null && Weapon.MeshList.Length > 0)
			foreach(MeshTransform3D mesh in Weapon.MeshList)
				this._AddMeshTransform(mesh);

		// Weapon Privates
		this._msBetweenShot = 1000.0f/(this.Weapon.FireRate/60.0f);
		GD.Print(this._msBetweenShot);
		
		// Weapon Animations
		if(this._ready){
			GD.Print("Adding Animations");
			if(this.Weapon.ShootAnimation != null)
			{
				this._animationLibrary.AddAnimation("Shoot", this.Weapon.ShootAnimation);
				GD.Print("Added shoot animation");
			}
			if(this.Weapon.ReloadAnimation != null)
			{
				this._animationLibrary.AddAnimation("Reload", this.Weapon.ReloadAnimation);
				GD.Print("Added reload animation");
			}
		}
	}

	public int Reload(){
		if(this.Magazine == this.Weapon.MagazineSize || this.Ammo == 0 || this._reloading) return 0;
		this._reloading = true;
		var bulletsToReload = Mathf.Clamp(this.Weapon.MagazineSize-this.Magazine, 0, this.Ammo);
		this.Ammo -= bulletsToReload;
		this.Magazine += bulletsToReload;
		GD.Print("Reloading", bulletsToReload, "bullet(s)");
		if(!this._animationLibrary.HasAnimation("Reload")){
			GD.Print("No Reload Animation");
			this._reloading = false;
		}else
			this._animationPlayer.Play(this._animationLibraryName+"/Reload");
		if(this.AutoReload && this._firing)
			this._Fire();
		return bulletsToReload;
	}

	private void _ClearWeapon(){
		GD.Print("Clearing my weapon");
		if(this._loadedMeshList.Count > 0){
			foreach(MeshInstance3D mesh in this._loadedMeshList){
				GD.Print(mesh);
				mesh.QueueFree();
			}
			this._loadedMeshList.Clear();
		}
		if(this._animationPlayer != null && this._animationLibrary != null){
			GD.Print("Clearing animations");
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
		if(this._firing) this._Fire();
	}

	private async void _Fire(){
		GD.Print("Trying to fire");
		
		if(this.Weapon == null || (this.Magazine <= 0 && !this.AutoReload) || !this._firing || this._fireCooldown || this._reloading)
			return;
		if(this.AutoReload && this.Magazine <= 0)
		{
			if (this.Ammo == 0)
			{
				if(this.Owner is Enemy)
					((Enemy)this.Owner).OutOfAmmo();
				GD.Print("Out of ammo");
			}
			else
				this.Reload();
			return;
		}
		GD.Print("Firing");
		
		//this._animationPlayer.Play(this._animationLibraryName + "/Shoot");
		if(this._animationLibrary.HasAnimation("Shoot")){
			GD.Print("Has Animation");
			this._animationPlayer.Stop();
			this._animationPlayer.Play(this._animationLibraryName+"/Shoot");
		}		
		this._muzzleFlash.Emitting = true;
		if (this.Owner is Enemy && ((Enemy)this.Owner).TargetPlayer != null)
		{
			
			var enemy = this.Owner as Enemy;
			var rand = this._random.Next(0, Mathf.CeilToInt(100 / enemy.HitChance) + 1);
			//GD.Print(enemy.TargetPlayer.GlobalPosition);
			//GD.Print(enemy.TargetPlayer.GlobalPosition - this.GlobalPosition);
			//GD.Print(this.WeaponRaycast.ToLocal(enemy.TargetPlayer.GlobalPosition));
			if (rand == 1)
			{
				GD.Print("Shooting towards player!");
				this.WeaponRaycast.TargetPosition = this.WeaponRaycast.ToLocal(enemy.TargetPlayer.GlobalPosition);
			}
			else
			{
				var randOffset = 1000;
				this.WeaponRaycast.TargetPosition = new Vector3(this._random.Next(-randOffset, randOffset), this._random.Next(-randOffset, randOffset), this._random.Next(-randOffset, randOffset));
			}
		}
		this.WeaponRaycast.ForceRaycastUpdate();
		if(this.WeaponRaycast.IsColliding())
			this._OnHit(this.WeaponRaycast.GetCollider() as Node3D);
		if(this._camera!= null)this._camera.RotateX(this.Weapon.Recoil);
		this.FiredBullets++;
		this.Magazine--;
		this._fireCooldown = true;
		await Task.Delay((int)Mathf.Round(this._msBetweenShot));
		this._fireCooldown = false;
		this._muzzleFlash.Emitting = false;
		if(this._weapon.Automatic)
			this._Fire();
	}

	private void _OnHit(Node3D node)
	{
		GD.Print(node.Name);
		if (node.IsInGroup("enemy") && node is DamageController)
		{
			GD.Print("hit enemy");
			var damageController = node as DamageController;
			damageController.TakeDamage(this.Weapon.Damage);
		}

		if (node.IsInGroup("player") && node is Player)
		{
			var player = node as Player;
			player.TakeDamage(this._weapon.Damage);
		}
	}
	private void _on_animation_player_animation_finished(StringName animationName)
	{
		GD.Print(animationName);
		if(animationName.Equals(this._animationLibraryName+"/Reload") && this._reloading)
		{
			this._reloading = false;
			if(this.AutoReload && this._firing)
				this._Fire();
		}
		
	}
}
