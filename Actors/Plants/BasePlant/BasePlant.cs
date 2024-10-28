using Godot;
using System;

public partial class BasePlant : StaticBody3D
{
	[Export] public int depth = 3;
	[Export] public int complexity = 2;
	[Export] public int dependantCap = 3;
	[Export] public float moistCap = 5;
	[Export] public float moistness = 0;

	[Export] public Node3D leaf;
	[Export] public Stem branch;
	[Export] public PackedScene stemScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		stemScene = GD.Load<PackedScene>("res://Actors/Plants/PlantParts/Stem.tscn");
	}
	// define a function for retrieving the scale and height of the stem
	// public void Retireve() {
		
	// }
	// define a generation function with a complexity and depth parameters

	// define a growth and reinforcement algorithm, this can start as random

	public void Grow() {
		GD.Print("growing!");
		Scale = Scale * 1.1f;
	}
	public void Shrink() {
		GD.Print("shrinking!");
		Scale = Scale * 0.9f;
	}
	public void QueueGrowth(int deep, int complex) {
		if (branch.getDependantCount() < dependantCap) {
			branch.recursiveChildGrowth(deep, complex);
		}
		branch.recursiveChildScale();
		Grow();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("trigger")) {
			// branch.growChild();
			// Godot.Collections.Array<Node> childStems = branch.getChildStems();
			// if (childStems.Count > 0) {
			// 	Stem stemChild = (Stem)childStems[0];
			// 	stemChild.growChild();
			// }
			// Generate tree here
			branch.recursiveChildGrowth(depth, complexity);
			branch.recursiveChildScale();
		}
		if (Input.IsActionPressed("increment")) {
			Grow();
		}
		if (Input.IsActionPressed("reduce")) {
			Shrink();
		}
	}
	public void _on_body_entered(Node body) {
		// GD.Print("collision class:", body.GetType() == typeof(Bullet));
		if (body.GetType() == typeof(Water)) {
			GD.Print("Water COLLISION!!!");
			// Bullet damager = (Bullet)body;
			// Water droplet = (Water)body;
			body._ExitTree();
			body.QueueFree();
			moistness += 1;
			if (moistness > moistCap) {
				QueueGrowth(depth, complexity);
			}
		}
	}
}
