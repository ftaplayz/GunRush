using System;
using System.Collections.Generic;
using Godot;

namespace GunRush.Classes;

public partial class Enemy : Humanoid
{
	public Player TargetPlayer { get; set; }
	public List<Node3D> Roams { get; set; } = new List<Node3D>();
	private AnimationTree _animationTree;
	public List<CollisionShape3D> Collisions = new List<CollisionShape3D>();
	public int HitChance { get; set; } = 50;
	public WeaponController WeaponController { get; set; }
	private Random _random = new Random();
	private EnemyStateMachine _stateMachine;
	
	public override void _Ready()
	{
		this._stateMachine = new EnemyStateMachine(this);
		this.WeaponController = this.GetNode<WeaponController>("Armature/Skeleton3D/RightHand/Weapon");
		this.WeaponController.AutoReload = true;
		this._animationTree = this.GetNode<AnimationTree>("AnimationTree");
		this._GetCollisionShapes(this);
		this._stateMachine.TransitionTo(this._stateMachine.Idle);
		//this._stateMachine.TransitionTo(this._stateMachine.Shooting);
		
	}

	private void _GetCollisionShapes(Node3D instance)
	{
		foreach (var child in instance.GetChildren())
		{
			if(child is CollisionShape3D)
				this.Collisions.Add(child as CollisionShape3D);
			if(child.GetChildCount() > 0)
				this._GetCollisionShapes(child as Node3D);
		}
	}

	public void SetRandom(Random random)
	{
		this._random = random;
	}

	/*private void _UpdateAnimationTree()
	{
		var shooting = this._hasTarget || this.ShootTest;
		var moving = false;
		this.SetCondition("die", this.Health <= 0);
		this.SetCondition("shoot", this._hasTarget || this.ShootTest);
		if(!this._hasTarget && !this.ShootTest){
			moving = this.Velocity != Vector3.Zero || this.WalkTest;
			this.SetCondition("walk", moving);
		}
		this.SetCondition("idle", !shooting && !moving);
		
	}*/

	public void SetCondition(string condition, bool value)
	{
		this._animationTree.Set("parameters/conditions/" + condition, value);
	}

	/*private void _StartDeath()
	{
		foreach (var collision in this._collisions)
			collision.Disabled = true;
		GD.Print("I am dying");
	}

	private void _EndDeath()
	{
		GD.Print("I am dead");
	}*/

	public override void _PhysicsProcess(double delta)
	{
		if(this.TargetPlayer != null && this._stateMachine.CurrentState != this._stateMachine.Die)
			this.LookAt(new Vector3(this.TargetPlayer.GlobalPosition.X, this.GlobalPosition.Y, this.TargetPlayer.GlobalPosition.Z), Vector3.Up);
	}

	private void _on_area_3d_damage_taken(double damage)
	{
		this.Health -= (float)damage;
		if(this.Health <= 0)
			this._stateMachine.TransitionTo(this._stateMachine.Die);
	}

	public void Aggro(Player player)
	{
		this.TargetPlayer = player;
		this._stateMachine.TransitionTo(this._stateMachine.Shooting);
	}

	public void OutOfAmmo()
	{
		this._stateMachine.TransitionTo(this._stateMachine.Die);
	}

}
