using System.Collections.Generic;
using Godot;

namespace GunRush.Classes;

[Tool]
public partial class Dungeon : Node3D
{

	/*
	TODO: 
	ALTERAR NOME DA CLASS PARA DUNGEON
	ADICIONAR INIMIGOS
	*/
	public List<Node3D> Spawns { get; set; } = new List<Node3D>();
	public List<Node3D> Roams { get; set; } = new List<Node3D>();
	private Aabb aabb;
	public override void _Ready()
	{
		this._LoadBotsZones();
	}

	private void _LoadBotsZones()
	{
		var bots = this.GetNodeOrNull<Node3D>("Bots");
		if (bots != null)
		{
			var spawns = bots.GetNodeOrNull<Node3D>("Spawns");
			var roams = bots.GetNodeOrNull<Node3D>("Roams");
			if (spawns != null) this._HasChildAdd(this.Spawns, spawns);
			if (roams != null) this._HasChildAdd(this.Roams, roams);
				
		}
	}

	public Aabb GetAabb(){
		this.aabb = new Aabb();
		this._GetAabb(this);
		return this.aabb;
	}

	private void _HasChildAdd(List<Node3D> list, Node3D instance)
	{
		if(instance.GetChildCount() > 0)
			foreach(var child in instance.GetChildren())
				if(child is Node3D)
					list.Add(child as Node3D);
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
