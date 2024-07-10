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
		this.Draw();
	}

	public void Draw()
	{
		this.Toggle(this._global.Crosshair);
		this.Dot(this._global.CrosshairDot);
	}
	
	public void Dot(bool enable = false){
		if(!enable){
			this._lines["dot"].Visible = false;
			return;
		}
		this._lines["dot"].Visible = true;
		var half = this._global.CrosshairDotSize/2;
		this._lines["dot"].SetPointPosition(0, new Vector2(-half, 0));
		this._lines["dot"].SetPointPosition(1, new Vector2(half, 0));
		this._lines["dot"].Width = this._global.CrosshairDotSize;
		this._lines["dot"].DefaultColor = this._global.CrosshairColor;
	}

	public void Toggle(bool enable = false){
		foreach(var line in this._lines){
			if(line.Key.Equals("dot")) continue;
			line.Value.Visible = enable;
			if(!enable)	continue;
			line.Value.DefaultColor = this._global.CrosshairColor;
			line.Value.Width = this._global.CrosshairWidth;
			var isX = line.Key.Equals("left") || line.Key.Equals("right");
			var negative = line.Key.Equals("left") || line.Key.Equals("top");
			var val = negative?-this._global.CrosshairCenterOffset:this._global.CrosshairCenterOffset;
			var val2 = negative?val-this._global.CrosshairLength:val+this._global.CrosshairLength;
			GD.Print(val, val2);
			line.Value.SetPointPosition(0, isX?new Vector2(val, 0):new Vector2(0, val));
			line.Value.SetPointPosition(1, isX?new Vector2(val2, 0):new Vector2(0, val2));
			GD.Print(line.Value.Points[0]);
		}
	}

}
