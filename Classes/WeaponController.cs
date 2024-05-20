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
			if(Engine.IsEditorHint())
				this._Ready();
		}
	}
	private MeshInstance3D _mesh;
	public override void _Ready()
	{
		this._mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		this._LoadWeapon();
	}

	private void _LoadWeapon()
	{
		if (this.Weapon == null)
		{
			this._mesh.Mesh = null;
			return;
		}
		this.Scale = this.Weapon.Size;
		this.Position = this.Weapon.Position;
		this.RotationDegrees = this.Weapon.Rotation;
		this._mesh.Mesh = this.Weapon.Mesh;
	}

	public void Fire()
	{
		
	}
}
