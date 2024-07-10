using Godot;
using System;
using GunRush.Classes;

public partial class StatsLastestPopulate : GridContainer
{
	
	private DatabaseHandler db = DatabaseHandler.Instance;
	private Label _exampleLabel; 
	public override void _Ready()
	{
		this._exampleLabel = this.GetNode<Label>("ExampleLabel").Duplicate() as Label;
		foreach (var child in this.GetChildren()) child.QueueFree();
		var runs = db.GetRuns(100);
		if (runs.Count > 0)
			foreach (var run in runs)
			{
				_AddLabel(run.Seed+"");
				_AddLabel(_GetDifficulty(run.Difficulty));
				_AddLabel(run.Duration+" s");
				_AddLabel(run.Kills+"");
				_AddLabel(run.Won?"YES_TEXT":"NO_TEXT");
			}
	}

	private void _AddLabel(string text)
	{
		var label = this._exampleLabel.Duplicate() as Label;
		label.Text = text;
		label.Visible = true;
		this.AddChild(label);
	}
	
	private string _GetDifficulty(int difficulty)
	{
		switch (difficulty)
		{
			case 0:
				return "EASY_DIFFICULTY";
			case 1:
				return "NORMAL_DIFFICULTY";
			case 2:
				return "HARD_DIFFICULTY";
		}
		return "";
	}
}
