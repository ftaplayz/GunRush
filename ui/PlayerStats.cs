using Godot;
using System;
using GunRush.Classes;

public partial class PlayerStats : VBoxContainer
{
	private DatabaseHandler _db = DatabaseHandler.Instance;
	public override void _Ready()
	{
		this.GetNode<LabelFormat>("RunsLabel").FormatText(_db.CountRuns(),_db.CountRuns(true));
		this.GetNode<LabelFormat>("KillsLabel").FormatText(_db.GetKills());
		this.GetNode<LabelFormat>("DeathsLabel").FormatText(_db.CountRuns()-_db.CountRuns(true));
		var shortest = _db.GetShortest();
		var longest = _db.GetLongest();
		this.GetNode<LabelFormat>("ShortestLabel").FormatText( shortest.Duration, shortest.Seed);
		this.GetNode<LabelFormat>("LongestLabel").FormatText(longest.Duration, longest.Seed);
	}
	
}
