using System;
using Godot;

namespace GunRush.Classes;

public partial class Player : Humanoid
{
	
	[Export] public int State = 0;
	public static int StateIdle = 0;
	public static int StateWalking = 1;
	public static int StateJumping = 2;
	public static int StateCrouching = 3;
	public static int StateFalling = 4;
	public bool JustPaused = false;
	private float _sensitivity = 0.5f;
	private CustomCamera _camera;
	private WeaponController _weapon;
	private RayCast3D _raycastStand;
	private CollisionShape3D _standingCollision;
	private CollisionShape3D _crouchingCollission;
	private float _crouchLerp = 0.05f;
	private float _fallTime = 0;
	private bool _jumping = false;
	private Vector3 _lastPosition;
	private float _oldCameraY;
	private bool _scheduledStand = false;
	private bool _crouching = false;
	private Label _killed;
	private Label _total;
	private Global _global;
	private Label _healthLabel;
	private Label _ammo;
	private Label _maxAmmo;
	
	public override void _Ready()
	{
		this._global = this.GetNode<Global>("/root/Global");
		//GD.Print(this.GetNode<Global>("/root/Global").Difficulty);
		this._sensitivity = _global.Sensitivity;
		this._camera = GetNode<CustomCamera>("CameraRoot");
		this._weapon = _camera.GetNode<WeaponController>("Weapon");
		this._raycastStand = GetNode<RayCast3D>("StandingRaycast");
		this._standingCollision = GetNode<CollisionShape3D>("StandingCollision");
		this._crouchingCollission = GetNode<CollisionShape3D>("CrouchingCollision");
		this._killed = this.GetNode<Label>("KillsCounter/HBoxContainer/Killed");
		this._total = this.GetNode<Label>("KillsCounter/HBoxContainer/Total");
		this._healthLabel = this.GetNode<Label>("Health/MarginContainer/HBoxContainer/HealthLabel");
		this._ammo = this.GetNode<Label>("Ammo/MarginContainer/HBoxContainer/Ammo");
		this._maxAmmo = this.GetNode<Label>("Ammo/MarginContainer/HBoxContainer/TotalAmmo");
		this._healthLabel.Text = this.Health+"";
		this._ammo.Text = this._weapon.Magazine + "";
		this._maxAmmo.Text = this._weapon.Ammo+"";
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			this.RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * _sensitivity));
			this._camera.RotateX(-mouseMotion.Relative.Y * _sensitivity);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			if(!JustPaused)
				this.GetNode<Pause>("pause").State(!GetTree().Paused);
			JustPaused = false;
		}
		
		if(Input.IsActionJustPressed("fire"))
			this._weapon.Fire(true);
		if(Input.IsActionJustReleased("fire"))
			this._weapon.Fire(false);
		if (Input.IsActionJustPressed("reload"))
			this._weapon.Reload();
		if (Input.IsActionJustPressed("crouch"))
		{
			this._Crouch(true, this._crouchingCollission, this._standingCollision);
			this._scheduledStand = false;
		}
		if(!this._scheduledStand)
			if (this._crouching && Input.IsActionJustReleased("crouch"))
				if (!this._raycastStand.IsColliding())
					this._Crouch(false, this._standingCollision, this._crouchingCollission);
				else
					this._scheduledStand = true;
	}
	

	public override void _PhysicsProcess(double delta)
	{		
		//GD.Print(this.State);
		var velocity = Velocity;
		if (this._jumping && this._lastPosition.Y > this.Position.Y)
		{
			this._jumping = false;
			//GD.Print("Reached jump peak, now falling");
		}
		if (!this.IsOnFloor())
		{
			if (this._jumping)
			{
				velocity.Y -= this.Gravity * (float)delta;
				//GD.Print("Jump slowing down");
			}
			else
			{
				//GD.Print("Falling");
				this._fallTime += (float)delta;
				velocity.Y = -(this.Gravity * Mathf.Pow(this._fallTime, 2));
			}
		}
		if (this._scheduledStand && !this._raycastStand.IsColliding())
		{
			this._scheduledStand = false;
			this._Crouch(false, this._standingCollision, this._crouchingCollission);
		}
		if (Input.IsActionJustPressed("jump") && this.IsOnFloor())
		{
			this._Jump(ref velocity);
		}
			
		var inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		var direction = (this.Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();
		if(direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		this._lastPosition = this.Position;
		this.Velocity = velocity;
		MoveAndSlide();
		this.State = this._CalculateState();
	}

	private int _CalculateState()
	{
		if (this.Velocity != Vector3.Zero)
		{
			if (Velocity.Y != 0)
				return Velocity.Y > 0 ? Player.StateJumping : Player.StateFalling;
			if (this._crouching)
				return Player.StateCrouching;
			return Player.StateWalking;
		}
		return Player.StateIdle;
	}

	private void _Jump(ref Vector3 velocity){
		//this._weapon.Weapon = GD.Load<Weapon>("res://weapons/m4/m4.tres");
		velocity.Y = this.JumpPower;
		this._jumping = true;
		this._fallTime = 0;
	}
	private void _Crouch(bool crouchState, CollisionShape3D shapeEnable, CollisionShape3D shapeDisable)
	{
		//this._weapon.Weapon = null;
		if (this._scheduledStand) return;
		var camPos = this._camera.Position;
		this._crouching = crouchState;
		if (crouchState)
		{
			this._oldCameraY = camPos.Y;
			camPos.Y -= 0.5f;
		}
		else
			camPos.Y = this._oldCameraY;
		shapeEnable.Disabled = false;
		shapeDisable.Disabled = true;
		this._camera.Position = camPos;
	}

	public override void _ExitTree()
	{
		this.GetTree().Paused = false;
	}

	private void _on_enemy_died()
	{
		this._global.EnemyKills++;
		this._killed.Text = this._global.EnemyKills+"";
	}
	
	private void _on_dungeon_dungeon_generated()
	{
		var totalEnemies = this.Owner.GetNode<DungeonGenerate>("Dungeon").TotalEnemies;
		this._total.Text = totalEnemies + "";
		this._weapon.Ammo = totalEnemies * this._weapon.Weapon.MagazineSize/ (this._global.Difficulty + 1);
		this._maxAmmo.Text = this._weapon.Ammo+"";

	}

	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);
		this._healthLabel.Text = this.Health+"";
		if (this.Health <= 0)
		{
			var world = (World)this.Owner;
			_global.GameOver = true;
			world.LogRun();
			world.QueueFree();
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GetTree().ChangeSceneToFile("res://ui/menu.tscn");
		}
	}
	
	private void _on_weapon_fired()
	{
		GD.Print("Fired");
		this._ammo.Text = this._weapon.Magazine + "";
	}


	private void _on_weapon_reloaded()
	{
		this._ammo.Text = this._weapon.Magazine + "";
		this._maxAmmo.Text = this._weapon.Ammo+"";
	}

}








