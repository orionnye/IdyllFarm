using Godot;
using System;

public partial class Spawner : Item
{
	[Export] public double timer = 0;
	[Export] public double cooldown = 3;
	[Export] public Node3D spawnNode;
	[Export] public PackedScene item = GD.Load<PackedScene>("res://Actors/Items/SingleUse/SingleUse.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

		activeMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 0f, 1f)
		};
		inactiveMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 1f, 1f)
		};
	}

	public override void Activate() {
		if (timer >= cooldown || timer == 0) {
			timer = 0;
			active = true;
			GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial);
			Node3D scene = (Node3D)item.Instantiate();
            GetTree().Root.AddChild(scene);
			scene.GlobalPosition = spawnNode.GlobalPosition;
		}
	}
	public override void Deactivate() {
		if (cooldown <= timer) {
			active = false;
			GetNode("MeshInstance3D").Set("surface_material_override/0", inactiveMaterial);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (active == true) {
			if (timer <= cooldown) {
				timer += delta;
				float baseValue = 2;
				float completion = baseValue - (float)(timer / cooldown);
				// GD.Print(completion);
				activeMaterial = new StandardMaterial3D{
					AlbedoColor = new Color(completion, 0f, completion)
				};
				GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial);
			} else {
				// GD.Print("timer over!");
				Deactivate();
			}
		}
	}
}