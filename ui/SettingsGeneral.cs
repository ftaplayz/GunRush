using Godot;
using System;
using GunRush.Classes;

public partial class SettingsGeneral : VBoxContainer
{
	private Global _global;
	private SpinBox _sensitivity;
	private OptionButton _language;
	private CheckButton _fullscreen;
	public override void _Ready()
	{
		this._global = this.GetNode<Global>("/root/Global");
		this._sensitivity = this.GetNode<SpinBox>("HBoxContainer/SensitivityInput");
		this._language = this.GetNode<OptionButton>("HBoxContainer2/LanguageInput");
		this._fullscreen = this.GetNode<CheckButton>("FullscreenButton");
		this._SetLanguage();
		this._SetSensitivity();
		this._SetFullscreen();
	}

	private void _SetFullscreen()
	{
		this._fullscreen.ButtonPressed = this._global.Fullscreen;
	}
	private void _SetSensitivity()
	{
		this._sensitivity.Value = this._global.Sensitivity;
	}
	private void _SetLanguage()
	{
		for (int i = 0; i < this._language.ItemCount; i++)
		{
			if (this._language.GetItemText(i) == this._global.Locale)
			{
				this._language.Select(i);
				break;
			}
		}
	}

	private void _LanguageChanged(long index)
	{
		var locale = this._language.GetItemText((int)index);
		this._global.Locale = locale;
		TranslationServer.SetLocale(locale);
	}
	
	private void _Fullscreen(bool toggled)
	{
		this._global.Fullscreen = toggled;
		DisplayServer.WindowSetMode(this._global.Fullscreen?DisplayServer.WindowMode.Fullscreen:DisplayServer.WindowMode.Windowed);
	}


	private void _Sensitivity(double value)
	{
		this._global.Sensitivity = (float)value;
	}
}
