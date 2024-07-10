using Godot;
using System;
using GunRush.Classes;

public partial class SettingsCrosshair : VBoxContainer
{
	private Global _global;
	private CheckButton _crosshair;
	private SpinBox _crosshairWidth;
	private SpinBox _crosshairLength;
	private SpinBox _crosshairCenterOffset;
	private ColorPickerButton _crosshairColor;
	private CheckButton _crosshairDot;
	private SpinBox _crosshairDotSize;
	private Crosshair _crosshairObj;

	public override void _Ready()
	{
		this._crosshairObj = this.GetParent().GetNode<Crosshair>("CenterContainer");
		this._global = this.GetNode<Global>("/root/Global");
		this._crosshair = this.GetNode<CheckButton>("CrosshairSwitch");
		this._crosshairWidth = this.GetNode<SpinBox>("HBoxContainer/CrosshairWidthInput");
		this._crosshairLength = this.GetNode<SpinBox>("HBoxContainer2/CrosshairLengthInput");
		this._crosshairCenterOffset = this.GetNode<SpinBox>("HBoxContainer3/CrosshairCenterOffsetInput");
		this._crosshairColor = this.GetNode<ColorPickerButton>("HBoxContainer4/CrosshairColorInput");
		this._crosshairDot = this.GetNode<CheckButton>("CrosshairDotSwitch");
		this._crosshairDotSize = this.GetNode<SpinBox>("HBoxContainer5/CrosshairDotSizeInput");
		this._SetCrosshair();
		this._SetCrosshairWidth();
		this._SetCrosshairLength();
		this._SetCrosshairCenterOffset();
		this._SetCrosshairColor();
		this._SetCrosshairDot();
		this._SetCrosshairDotSize();
	}

	private void _SetCrosshair()
	{
		this._crosshair.ButtonPressed = this._global.Crosshair;
	}

	private void _SetCrosshairWidth()
	{
		this._crosshairWidth.Value = this._global.CrosshairWidth;
	}

	private void _SetCrosshairLength()
	{
		this._crosshairLength.Value = this._global.CrosshairLength;
	}

	private void _SetCrosshairCenterOffset()
	{
		this._crosshairCenterOffset.Value = this._global.CrosshairCenterOffset;
	}

	private void _SetCrosshairColor()
	{
		this._crosshairColor.Color = this._global.CrosshairColor;
	}

	private void _SetCrosshairDot()
	{
		this._crosshairDot.ButtonPressed = this._global.CrosshairDot;
	}

	private void _SetCrosshairDotSize()
	{
		this._crosshairDotSize.Value = this._global.CrosshairDotSize;
	}

	private void _Crosshair(bool toggled)
	{
		this._global.Crosshair = toggled;
		this._crosshairObj.Draw();
	}

	private void _CrosshairWidth(double value)
	{
		this._global.CrosshairWidth = (int)value;
		this._crosshairObj.Draw();
	}

	private void _CrosshairLength(double value)
	{
		this._global.CrosshairLength = (int)value;
		this._crosshairObj.Draw();
	}

	private void _CrosshairCenterOffset(double value)
	{
		this._global.CrosshairCenterOffset = (int)value;
		this._crosshairObj.Draw();
	}

	private void _CrosshairColor(Color value)
	{
		this._global.CrosshairColor = value;
		this._crosshairObj.Draw();
	}

	private void _CrosshairDot(bool toggled)
	{
		this._global.CrosshairDot = toggled;
		this._crosshairObj.Draw();
	}

	private void _CrosshairDotSize(double value)
	{
		this._global.CrosshairDotSize = (int)value;
		this._crosshairObj.Draw();
	}
}
