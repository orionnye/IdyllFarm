using Godot;
using System;

public partial class Gun : Spawner
{
	[Export] public int bulletCount = 1;
	[Export] public float force = 15;
	[Export] public float noise = 0.2f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		cooldown = 0.5;
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
			for (int i = 0; i < bulletCount; i++) {
				Projectile scene = (Projectile)item.Instantiate();
				Vector3 randNoise = new Vector3(GD.Randf()*noise - (noise/2), GD.Randf()*noise - (noise/2), GD.Randf()*noise - (noise/2));
				GetTree().Root.AddChild(scene);
				scene.Position = spawnNode.GlobalPosition;
				// Assign direction
				Vector3 forceDir = -spawnNode.GlobalTransform.Basis.Z.Normalized();
				Vector3 impulse = force*forceDir+randNoise;
				// Apply Impulse in Direction
				scene.ApplyCentralImpulse(impulse);
				// GD.Print("impulse:", impulse);
				// GD.Print("impulse length:", impulse.Length());
			}
		}
	}
	public override void Deactivate() {
		if (cooldown <= timer) {
			active = false;
			// GetNode("MeshInstance3D").Set("surface_material_override/0", inactiveMaterial);
		}
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
				// 	AlbedoColor = new Color(completion, 0f, completion)
				// };
				// GetNode("MeshInstance3D").Set("surface_material_override/0", activeMaterial);
			} else {
				// GD.Print("timer over!");
				Deactivate();
			}
		}
	}
}