using Godot;
using System;

public partial class Turret : StaticBody3D
{
	[Export] public float gainRate = 2;
	[Export] public Node3D target;
	[Export] public Node3D grabber;
	[Export] public Item item;
	// Activation rate and use stats
	[Export] public double cooldown = 1;
	[Export] public double timer = 0;
	[Export] public bool active = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}
	public void Fire() {
		if (item != null) {
			item.Activate();
		} else {
			GD.PrintErr("Unassigned Item on Turret");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (target != null) {
			grabber.LookAt(target.GlobalPosition, Vector3.Up);
		}
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
				// GD.Print("Firing!");
				item.Activate();
				timer = 0;
				// GD.Print("timer over!");
				// Deactivate();
			}
		}
	}
}
