using Godot;
using System;
public partial class LabelFormat : Label
{
	public void FormatText(params object[] args)
	{
		this.Text = String.Format(Tr(this.Text), args);
	}
}
