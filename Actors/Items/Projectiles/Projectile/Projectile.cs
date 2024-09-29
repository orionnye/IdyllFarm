using Godot;
using System;

public partial class Projectile : Item
{
	[Export] public Vector3 impulseDir = new Vector3(0, 0, 0);
	[Export] float mass = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// initializing values
		Mass = mass;
		active = true;
		activeMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(0f, 1f, 0f)
		};
		inactiveMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 1f, 1f)
		};
		// ApplyCentralImpulse(impulseDir);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
    // public override void _PhysicsProcess(double delta)
    // {
    //     base._PhysicsProcess(delta);
    // }

}
