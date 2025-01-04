using Godot;
using System;

public partial class BasePlant : StaticBody3D
{
	[Export] public int initialComplexity = 2;
	[Export] public float moistCap = 5;
	[Export] public float moistness = 0;
	[Export] public Node3D branch;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (branch is Stem stem) {
			stem.Grow();
		}
	}

	public void Grow() {
		if (branch is Stem stem) {
			stem.IncreaseComplexity();
		}
	}

	public void Shrink() {
		if (branch is Stem stem) {
			stem.DecreaseComplexity();
		}
	}

	public void Reset() {
		if (branch is Stem stem) {
			stem.Reset();
		}
		moistness = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("trigger")) {
			Grow();
		}
		if (Input.IsActionPressed("increment")) {
			Grow();
		}
		if (Input.IsActionPressed("reduce")) {
			Shrink();
		}
		if (Input.IsActionJustPressed("reset_plant")) {
			Reset();
		}
	}

	public void _on_body_entered(Node body) {
		if (body.GetType() == typeof(Water)) {
			GD.Print("Water COLLISION!!!");
			body._ExitTree();
			body.QueueFree();
			moistness += 1;
			if (moistness > moistCap) {
				Grow();
				moistness = 0;
			}
		}
	}
}
