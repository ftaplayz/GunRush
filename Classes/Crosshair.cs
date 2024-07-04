using Godot;
using GunRush.Classes;
using System;
using System.Collections.Generic;

public partial class Crosshair : CenterContainer
{
	private IDictionary<string, Line2D> _lines = new Dictionary<string, Line2D>();
	private Global _global;
	public override void _Ready()
	{
		this._lines.Add("top", this.GetNode<Line2D>("Top"));
		this._lines.Add("bottom", this.GetNode<Line2D>("Bottom"));
		this._lines.Add("left", this.GetNode<Line2D>("Left"));
		this._lines.Add("right", this.GetNode<Line2D>("Right"));
		this._lines.Add("dot", this.GetNode<Line2D>("Dot"));
		this._global = this.GetNode<Global>("/root/Global");
	}
	
	public void Dot(bool enable = false){
		if(!enable){
			this._lines["dot"].Visible = false;
			return;
		}
		this._lines["dot"].Visible = true;
		var half = this._global.CrosshairDotSize/2;
		this._lines["dot"].Points[0].X = -half;
		this._lines["dot"].Points[1].X = half;
		this._lines["dot"].Width = this._global.CrosshairDotSize;
	}

}
