using Godot;
using System;

public partial class User : RigidBody3D
{
	[Export] public int speedCap = 15;
	[Export] public float speedGain = 5f;
	[Export] public Node3D spawnAnchor;
	[Export] public Vector3 spawnPoint = new Vector3(0, 1, 0);
	[Export] public bool spawnControl = false;
	[Export] public Node3D inputMarker;
	[Export] public Node3D velocityMarker;
	[Export] public Node3D Meshes;
	[Export] public Hands hands;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	public void UserControls() {
		Vector3 yBump = new Vector3(0, 0.1f, 0);
		if (Input.IsActionJustReleased("Space")) {
			if (spawnControl) {
				GlobalPosition = spawnAnchor.GlobalPosition + yBump;
			}
			// Teleport
			// GD.Print("respawn");
		}
		if (Input.IsActionJustReleased("r")) {
			spawnAnchor.GlobalPosition = GlobalPosition;
			// Reset teleport
			// GD.Print("reset spawn");
		}
		userHands();
	}
	public void userHands() {
		Item target = hands.getTargetItem();
		// if (target != null) {
		// 	cam.TextOnNode(target);
		// }
		if (hands.isHolding() && Input.IsActionJustReleased("Space")) {
			hands.Use((Item)hands.grabber.GetChild(0));
		}
		if (Input.IsActionJustReleased("Shift")) {
			// GD.Print("isHolding: ", isHolding());
			if (hands.isHolding()) {
				// GD.Print("trying to drop");
				hands.Drop();
			} else {
				if (target != null) {
					// GD.Print("trying to grab");
					hands.Grab(target);
				}
				// Restructure this design to return the object closest and add a UI element in the viewport over it
			}
		}
	}
	public Vector3 UserWASD() {
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

	public void checkBounds() {
		if (GlobalPosition.Y <= -2) {
			GlobalPosition = spawnPoint;
		}
	}
	public void updateInputMarker() {
		Vector3 force = UserWASD();
		// GD.Print(force);
		Vector3 trajectory = GlobalPosition+force;
		// Find desired rotation and then lerp towards it(steal code from camera)
		inputMarker.GlobalPosition = inputMarker.GlobalPosition.Lerp(trajectory, 0.8f);
	}
	public void updateVelocityMarker() {
		// GD.Print(force);
		Vector3 trajectory = GlobalPosition+LinearVelocity;
		// Find desired rotation and then lerp towards it(steal code from camera)
		velocityMarker.GlobalPosition = velocityMarker.GlobalPosition.Lerp(trajectory, 0.8f);
	}
	public void updateSpawnAnchor() {
		// GD.Print(force);
		Vector3 trajectory = GlobalPosition+LinearVelocity;
		// Find desired rotation and then lerp towards it(steal code from camera)
		velocityMarker.GlobalPosition = velocityMarker.GlobalPosition.Lerp(trajectory, 0.8f);
	}
	public void updateMeshLook() {
		;
		float diff = (Meshes.Position - inputMarker.Position).Length();
		float diffMin = 0.1f;
		if (diff > diffMin) {
			Meshes.LookAt(inputMarker.GlobalPosition);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		checkBounds();
		UserControls();
		updateInputMarker();
		updateVelocityMarker();
		updateMeshLook();
	}
    public override void _IntegrateForces(PhysicsDirectBodyState3D state) {
        base._IntegrateForces(state);
    }
    public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		// Find the input force depending on current speed
		// Max out the speed or speed gain depending on current velocity

		// Find the current speed and then compare it to the desired max speed
		Vector3 force = UserWASD() * (float)speedGain;
		if (LinearVelocity.Length() <= speedCap) {
			ApplyCentralForce(force);
		}
	}
}
