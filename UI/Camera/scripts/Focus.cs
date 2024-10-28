using Godot;
using System;

public partial class Focus : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float rotSpeed = 0.1f;
		Vector3 rot = new Vector3(0, 0, 0);
		if (Input.IsActionPressed("a")) {
			rot.Y += -rotSpeed;
		}
		if (Input.IsActionPressed("d")) {
			rot.Y += rotSpeed;
		}
		Rotation += rot;
	}
}
