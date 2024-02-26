using System;
using System.Diagnostics;
using Godot;

public partial class Player : CharacterBody3D
{
    [Export] public float WalkSpeed { get; set; } = 40;
    [Export] public float Gravity { get; set; } = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    [Export] public float Sensitivity = 0.5f;
    private Vector3 _targetVelocity = Vector3.Zero;
    private Node3D _camera;

    public override void _Ready()
    {
        _camera = GetNode<Node3D>("CameraRoot");
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Debug.Print("Gravity: "+Gravity);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * Sensitivity));
            _camera.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * Sensitivity));
            Vector3 euler = _camera.Transform.Basis.GetEuler();
            euler.X = Mathf.Clamp(euler.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
            _camera.Basis = Basis.FromEuler(euler);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 direction = Vector3.Zero;
        if (!IsOnFloor())
            direction.Y -= Gravity * (float)delta;
        if (Input.IsActionPressed("move_forward"))
            direction.Z += 1.0f;
        if (Input.IsActionPressed("move_backward"))
            direction.Z -= 1.0f;
        if (Input.IsActionPressed("move_left"))
            direction.X -= 1.0f;
        if (Input.IsActionPressed("move_right"))
            direction.X += 1.0f;
    }
}