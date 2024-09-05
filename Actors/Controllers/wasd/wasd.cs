using Godot;
using System;

public partial class wasd : RigidBody3D
{
	[Export] public int speedCap = 15;
	[Export] public float speedGain = 5f;
	[Export] public Node3D trajectoryMarker;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}
	public Vector3 UserInput() {
		Vector3 direction = new Vector3(0, 0, 0);
		bool lockDirection = false;
		// Mutate var if key pressed
		if (Input.IsActionPressed("w")) {
			direction += new Vector3(0, 0, -1);
		}
		if (Input.IsActionPressed("a")) {
			direction += new Vector3(-1, 0, 0);
		}
		if (Input.IsActionPressed("s")) {
			direction += new Vector3(0, 0, 1);
		}
		if (Input.IsActionPressed("d")) {
			direction += new Vector3(1, 0, 0);
		}
		// return direction
		if (!lockDirection) {
			direction = direction.Rotated(new Vector3(0, 1, 0), Rotation.Y);
		}
		return direction.Normalized();
	}
	public void updateTrajectoryMarker() {
		Vector3 force = UserInput() * (float)speedGain;
		Vector3 trajectory = GlobalPosition+force;
		// Find desired rotation and then lerp towards it(steal code from camera)
		trajectoryMarker.GlobalPosition = trajectory;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		updateTrajectoryMarker();
	}
    public override void _IntegrateForces(PhysicsDirectBodyState3D state) {
        base._IntegrateForces(state);
    }
    public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		// Find the input force depending on current speed
		// Max out the speed or speed gain depending on current velocity

		// Find the current speed and then compare it to the desired max speed
		Vector3 force = UserInput() * (float)speedGain;
		if (LinearVelocity.Length() <= speedCap) {
			ApplyCentralForce(force);
		}
	}
}
