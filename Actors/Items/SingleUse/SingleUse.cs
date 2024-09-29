using Godot;
using System;

public partial class SingleUse : Item
{
	[Export] double timer = 0;
	[Export] float cooldown = 5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		active = false;
		activeMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(0f, 1f, 0f)
		};
		inactiveMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 1f, 1f)
		};
	}

	public void Remove(Node node) {
		GD.Print("Goodbye!");
		node._ExitTree();
		node.QueueFree();
	}

	public override void Activate() {
		active = true;
		GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial);
	}
	public override void Deactivate() {
		active = false;
		GetNode("MeshInstance3D").Set("surface_material_override/0", inactiveMaterial);
		Remove(this);
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
					AlbedoColor = new Color(0f, completion, 0f)
				};
				GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial); 
			} else {
				// GD.Print("timer over!");
				Deactivate();
			}
		}
	}
}
