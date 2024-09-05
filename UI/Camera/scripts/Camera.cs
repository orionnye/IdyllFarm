using Godot;
using System;

public partial class Camera : Node3D
{
	public Vector3 fixedPos;
	public Vector3 fixedRot;
	[Export] Camera3D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		fixedPos = Position;
		fixedRot = Rotation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Position = fixedPos;
		Rotation = fixedRot;
	}
}
