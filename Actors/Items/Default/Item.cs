using Godot;
using System;

public partial class Item : Node3D
{
	// Storage and Translation
	[Export] public Vector3 heldPosition = new Vector3(0, 0, 0);
	[Export] public Vector3 heldRotation = Vector3.Zero;
	public Boolean active = false;
	double timer = 0;
	double cooldown = 0.5;

	// UI
	// [Export] public NodeTracker UIMarker;

	public StandardMaterial3D activeMaterial;
	public StandardMaterial3D inactiveMaterial;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		activeMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 0f, 0f)
		};
		inactiveMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 1f, 1f)
		};
	}
	
	// Base Activate function
	public virtual void Activate() {
		// GD.Print("Activating: ", this);
		active = true;
		GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial);
	}
	// Base Inactive function
	public virtual void Deactivate() {
		// GD.Print("Deactivating: ", this);
		active = false;
		GetNode("MeshInstance3D").Set("surface_material_override/0", inactiveMaterial);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// if (active) {
			// timer += delta;
			// if (timer >= cooldown) {
			// 	timer = 0;
			// 	Deactivate();
			// }
		// } else {
			// timer += delta;
			// if (timer > cooldown) {
			// 	timer = 0;
			// 	Activate();
			// }
		// }

	}
}