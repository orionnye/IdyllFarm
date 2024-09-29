using Godot;
using System;

public partial class Bullet : Projectile
{
	// // Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// GD.Print("Entering the scene");
		// ApplyCentralImpulse(impulseDir);
		Reparent(GetTree().Root, true);
	}

	// // Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }
	private void _on_timer_timeout() {
		// Replace with function body.
		_ExitTree();
		QueueFree();
	}
}
