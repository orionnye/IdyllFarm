using Godot;
using System;

public partial class Rock : Item
{
	[Export] public float mass = 0.1f;
	[Export] public float force = 1;
	[Export] public bool isMagic = true;
	[Export] public double timer = 0;
	[Export] public double cooldown = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Mass = mass;
	}

    public override void Activate() {
		GD.Print("Thowing Rock");
		active = true;
		// Unfreeze
		Set("freeze", false);
		Freeze = false;
        // Correct rotation
		Rotation = new Vector3(0, 0, 0);
		// Reparent(GetTree().Root, true);
		// Assign direction
		Vector3 forceDir = new Vector3(0, 0, -1).Rotated(Vector3.Up, GlobalRotation.Y);
		Vector3 impulse = force*forceDir;
		// Apply Impulse in Direction
		ApplyCentralImpulse(impulse);
		GD.Print("impulse: ", impulse);
		// Reparent
    }
    public override void Deactivate() {
		GD.Print("Storing Rock");
		active = false;
		timer = 0;
		// _ExitTree();
		// QueueFree();
		Rotation = heldRotation;
		Position = heldPosition;
		Set("freeze", true);
		Freeze = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
		if (active == true) {
			if (timer <= cooldown) {
				timer += delta;
				// float baseValue = 2;
				// float completion = baseValue - (float)(timer / cooldown);
				// GD.Print(completion);
				// activeMaterial = new StandardMaterial3D{
				// 	AlbedoColor = new Color(0f, completion, 0f)
				// };
				// GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial); 
			} else {
				// GD.Print("timer over!");
				Deactivate();
			}
		}
	}
}
