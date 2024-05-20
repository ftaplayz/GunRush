using Godot;

namespace GunRush.Classes;

public partial class Player : Humanoid
{
	[Export] public float Sensitivity = 0.5f;
	[Export] public int State = 0;
	public static int StateIdle = 0;
	public static int StateWalking = 1;
	public static int StateJumping = 2;
	public static int StateCrouching = 3;
	public static int StateFalling = 4;
	private Node3D _camera;
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
	private bool _firing = false;
	
	public override void _Ready()
	{
		this._camera = GetNode<Node3D>("CameraRoot");
		this._weapon = _camera.GetNode<WeaponController>("Weapon");
		this._raycastStand = GetNode<RayCast3D>("StandingRaycast");
		this._standingCollision = GetNode<CollisionShape3D>("StandingCollision");
		this._crouchingCollission = GetNode<CollisionShape3D>("CrouchingCollision");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			this.RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * Sensitivity));
			this._camera.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * Sensitivity));
			var euler = this._camera.Transform.Basis.GetEuler();
			euler.X = Mathf.Clamp(euler.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
			this._camera.Basis = Basis.FromEuler(euler);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if(Input.IsActionJustPressed("pause"))
			GetTree().Quit();
		if(Input.IsActionJustPressed("fire"))
			this._firing = true;
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

	public override void _Process(double delta)
	{
		if (this._scheduledStand && !this._raycastStand.IsColliding())
		{
			this._scheduledStand = false;
			this._Crouch(false, this._standingCollision, this._crouchingCollission);
		}
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
		
		if (Input.IsActionJustPressed("jump") && this.IsOnFloor())
		{
			velocity.Y = this.JumpPower;
			this._jumping = true;
			this._fallTime = 0;
		}

		if(this._firing){
			this._weapon.Fire();
			this._firing = Input.IsActionPressed("fire");
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

	private void _Crouch(bool crouchState, CollisionShape3D shapeEnable, CollisionShape3D shapeDisable)
	{
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
}
