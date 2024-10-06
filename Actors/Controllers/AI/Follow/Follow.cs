using Godot;
using System;

public partial class Follow : Node3D
{
	[Export] public Node3D target;
	[Export] public float targetDist = 10f;
	[Export] public float speed = 5;
	[Export] public float rotSpeed = 2f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	public void lerpAtSpeedToTarget() {
		// lerp at speed to target
	}
	public void lookAtTarget() {
		// look at target at rotation speed
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// lerp to target at speed
		// look at target at speed
	}
}
