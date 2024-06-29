using Godot;

namespace GunRush.Classes;

public partial class DamageController : Node3D
{
	[Export] public float DamageMultiplier { get; set; } = 1.0f;

	[Signal]
	public delegate void DamageTakenEventHandler(float damage);

	public void TakeDamage(float damage)
	{
		this.EmitSignal(SignalName.DamageTaken, damage * this.DamageMultiplier);
	}
}
