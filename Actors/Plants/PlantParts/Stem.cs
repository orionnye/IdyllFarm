using Godot;
using System;

public partial class Stem : Node3D
{
	[Export] public float height = 1f;
	[Export] public float girth = 0.05f;
	[Export] public Vector3 radialDisposition = new Vector3(0, 0, 0.5f);
	[Export] public Vector3 noiseDeviation = new Vector3(0.5f, (float)Math.PI, 0.5f);
	private Vector3 radialNoise = new Vector3(GD.Randf(), GD.Randf(), GD.Randf());
	[Export] public float dependentScalar = 0.1f;
	[Export] public PackedScene stemScene;
	[Export] public PackedScene leafScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		stemScene = GD.Load<PackedScene>("res://Actors/Plants/PlantParts/Stem.tscn");
		leafScene = GD.Load<PackedScene>("res://Actors/Plants/PlantParts/Leaf.tscn");
	}
	// add child to the branch
	public void growChild() {
		// GD.Print("Growing!");
		Stem stem = (Stem)stemScene.Instantiate();
		AddChild(stem);
		// need to fix rotation to be random
		// store rotation as a local variable then mutate Y rotation
		Vector3 randRot = new Vector3(
			(float)GD.Randfn(0, noiseDeviation.X),
			(float)GD.Randfn(0, noiseDeviation.Y),
			(float)GD.Randfn(0, noiseDeviation.Z));
		stem.Rotation = randRot;
		stem.Position = new Vector3(0, GD.Randf(), 0);
	}
	public void growLeaf() {
		Node3D leaf = (Node3D)leafScene.Instantiate();
		AddChild(leaf);
		// need to fix rotation to be random
		// store rotation as a local variable then mutate Y rotation
		Vector3 randRot = new Vector3(
			(float)GD.Randfn(0, noiseDeviation.X),
			(float)GD.Randfn(0, noiseDeviation.Y),
			(float)GD.Randfn(0, noiseDeviation.Z));
		leaf.Rotation = randRot;
		leaf.Position = new Vector3(0, GD.Randf(), 0);
	}
	public int getDependantCount() {
		int count = 0;
		Godot.Collections.Array<Stem> children = getChildStems();
		if (children.Count > 0) {
			foreach (Stem child in children) {
				count += child.getDependantCount();
			}
		}
		return count;
	}
	public void recursiveChildScale() {
		Godot.Collections.Array<Stem> children = getChildStems();
		int count = getDependantCount();
		float scalar = 2;
		Scale = new Vector3(0.5f, 0.5f, 0.5f);
		if (children.Count > 0) {
			foreach (Stem child in children) {
				// child.Scale = new Vector3();
				child.recursiveChildScale();
			}
		}
	}
	public void recursiveChildGrowth(int depth, int complexity) {
		Godot.Collections.Array<Stem> children = getChildStems();
		if (depth > 0) {
			for (int i = 0; i < complexity; i++) {
				growChild();
			}
			children = getChildStems();
			foreach (Stem child in children) {
				child.recursiveChildGrowth(depth-1, complexity);
			}
		} else {
			// GD.Print("thats deep enough bro, I'm making leaves");
			for (int i = 0; i < complexity*2; i++) {
				growLeaf();
			}
		}
	}
	public void growGrandChildren() {
		Godot.Collections.Array<Stem> stems = getChildStems();
		if (stems.Count > 0) {
			foreach(Stem child in stems) {
				child.growChild();
			}
		}
	}
	public Godot.Collections.Array<Stem> getChildStems() {
		Stem stem = (Stem)stemScene.Instantiate();
		int childCount = GetChildCount();
		// GD.Print("Getting child stems");
		// GD.Print("Getting child count:", childCount);
		Godot.Collections.Array<Node> children = GetChildren();
		// GD.Print("Getting children:", children);
		Godot.Collections.Array<Stem> childStems = new Godot.Collections.Array<Stem>();
		for (var i = 0; i < childCount; i++) {
			// GD.Print("children:", children[i]);
			// GD.Print("children:", children[i].GetType());
			if (children[i].GetType() == stem.GetType()) {
				childStems.Add((Stem)children[i]);
				// GD.Print("found a childStem!:", children[i]);
			}
		}
		return childStems;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
