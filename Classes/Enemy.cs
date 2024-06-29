using System.Collections.Generic;
using Godot;

namespace GunRush.Classes;

public partial class Enemy : Humanoid
{
	[Export] public bool WalkTest { get; set; }
	[Export] public bool ShootTest { get; set; }
	private AnimationTree _animationTree;
	private List<CollisionShape3D> _collisions = new List<CollisionShape3D>();
	private bool _hasTarget = false;
	
	public override void _Ready()
	{
		this._animationTree = this.GetNode<AnimationTree>("AnimationTree");
		this._GetCollisionShapes(this);
	}

	public override void _Process(double delta)
	{
		this._UpdateAnimationTree();
	}

	private void _GetCollisionShapes(Node3D instance)
	{
			foreach (var child in instance.GetChildren())
			{
				if(child is CollisionShape3D)
					this._collisions.Add(child as CollisionShape3D);
				if(child.GetChildCount() > 0)
					this._GetCollisionShapes(child as Node3D);
			}
	}

	private void _UpdateAnimationTree()
	{
		
		this._SetCondition("die", this.Health <= 0);
		this._SetCondition("shoot", this._hasTarget || this.ShootTest);
		if(!this._hasTarget && !this.ShootTest){
			var moving = this.Velocity != Vector3.Zero || this.WalkTest;
			this._SetCondition("idle", !moving);
			this._SetCondition("walk", moving);
		}
		
	}

	private void _SetCondition(string condition, bool value)
	{
		this._animationTree.Set("parameters/conditions/" + condition, value);
	}

	private void _StartDeath()
	{
		foreach (var collision in this._collisions)
			collision.Disabled = true;
		GD.Print("I am dying");
	}

	private void _EndDeath()
	{
		GD.Print("I am dead");
	}
	
	private void _on_area_3d_damage_taken(double damage)
	{
		//GD.Print("And I sucked his cock in poland");
		GD.Print(damage);
		
		this.Health -= (float)damage;
		GD.Print(this.Health);
	}

}
