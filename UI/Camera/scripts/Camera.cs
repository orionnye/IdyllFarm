using Godot;
using System;

public partial class Camera : Node3D
{
	[Export] public Vector3 fixedPos;
	[Export] public Vector3 fixedRot;
	[Export] Camera3D camera;
	[Export] private bool useWorldDiff = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (!useWorldDiff) {
			fixedPos = Position;
			fixedRot = Rotation;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Position = fixedPos;
		Rotation = fixedRot;
	}
}
