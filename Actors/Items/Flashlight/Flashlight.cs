using Godot;
using System;

public partial class Flashlight : Item
{
	[Export] public SpotLight3D lightSource;
	[Export] public float lightOn = 10f;
	[Export] public float lightOff = 0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Stat Init
		activeMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 0f, 0f)
		};
		inactiveMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 1f, 1f)
		};

		// StatForce
		if (active) {
			Activate();
		}
		else {
			Deactivate();
		}
	}

	public override void Activate() {
		// GD.Print("turning on flashlight");
		lightSource.LightEnergy = lightOn;
		active = true;
		GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial);
	}
	public override void Deactivate() {
		// GD.Print("turning off flashlight");
		lightSource.LightEnergy = lightOff;
		active = false;
		GetNode("MeshInstance3D").Set("surface_material_override/0", inactiveMaterial);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
