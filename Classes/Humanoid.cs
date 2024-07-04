using Godot;

namespace GunRush.Classes;

public partial class Humanoid : CharacterBody3D
{
	[Export] public float MaxHealth { get; set; } = 100;
	private float health = 100;
	[Export] public float Health {
		get
		{
			return this.health;
		}
		set
		{
			this.health = Mathf.Clamp(value, 0, MaxHealth);
		}
	}
	[Export] public float Speed = 5.0f;
	[Export] public float JumpPower= 4.5f;
	[Export] public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	[Export] public float FallStrength = 10;
	
	public void TakeDamage(float damage)
	{
		this.Health -= damage;
	}
}
