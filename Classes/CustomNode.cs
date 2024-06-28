using Godot;

namespace GunRush.Classes;

[Tool]
public partial class CustomNode : Node3D
{

	/*
	TODO: 
	ALTERAR NOME DA CLASS PARA DUNGEON
	ADICIONAR INIMIGOS
	*/
	private Aabb aabb;
	public Aabb GetAabb(){
		this.aabb = new Aabb();
		this._GetAabb(this);
		return this.aabb;
	}

	private void _GetAabb(Node3D instance){
		foreach(var member in instance.GetChildren()){
			if(member is MeshInstance3D)
				this.aabb = this.aabb.Merge((member as MeshInstance3D).GetAabb());
			if(member.GetChildCount() > 0)
				this._GetAabb(member as Node3D);
		}
	}
}
