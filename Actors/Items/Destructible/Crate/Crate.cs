using Godot;
using System;

public partial class Crate : Item
{
	[Export] public int health = 2;
	[Export] public Node3D contained;

	public void freezeContained(bool freeze) {
		// iterate through and freeze children, then abstract into a function with a freeze toggle parameter
		Godot.Collections.Array<Node> children = contained.GetChildren();
		
		// find item count
		int childCount = children.Count;

		// iterate through item count
		for (int i = 0; i < childCount; i++) {
			// GD.Print(children[i]);
			RigidBody3D child = (RigidBody3D)children[i];
			child.Freeze = freeze;
			child.Sleeping = freeze;
			if (freeze) {
				child.CollisionLayer = 2;
				child.CollisionMask = 2;
			}
		}
	}
	public void dropChildren() {
		Godot.Collections.Array<Node> children = contained.GetChildren();
		// find item count
		int childCount = children.Count;
		// iterate through item count
		for (int i = 0; i < childCount; i++) {
			// GD.Print(children[i]);
			RigidBody3D child = (RigidBody3D)children[i];
			child.Freeze = false;
			child.Sleeping = false;
			child.CollisionLayer = 1;
			child.CollisionMask = 1;
			// reparent child
			child.Reparent(GetTree().Root, true);
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		CollisionMask = 1;
		activeMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 0f, 0f)
		};
		inactiveMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1f, 1f, 1f)
		};
		freezeContained(true);
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
	public void takeDamage(int damage) {
		GD.Print("health at:", health);
		health -= damage;
		GD.Print("health at:", health);

		float scalar = 0.1f;
		// float completion = baseValue - (float)(timer / cooldown);
		// GD.Print(completion);
		activeMaterial = new StandardMaterial3D{
			AlbedoColor = new Color(1 - health*scalar, 0f, 0f)
		};
		GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial); 
		if (health <= 0) {
			dropChildren();
			GD.Print("Goodbye World");
			_ExitTree();
			QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	public void _on_body_entered(Node body) {
		GD.Print("collision class:", body.GetType() == typeof(Bullet));
		if (body.GetType() == typeof(Bullet)) {
			GD.Print("Bullet COLLISION!!!");
			Bullet damager = (Bullet)body;
			takeDamage((int)Math.Ceiling(damager.Mass));
		}
	}
}
