using Godot;
using System;

public partial class BasePlant : StaticBody3D
{
	[Export] public int initialComplexity = 2;
	[Export] public float moistCap = 5;
	[Export] public float moistness = 0;
	[Export] public Node3D branch;

	// Growth configuration
	private PlantGrowthConfig growthConfig;
	
	[Export] public string plantType = "Default";  // Can be "Default", "Bushy", "TallSparse", or "Vine"

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Set growth configuration based on plant type
		growthConfig = plantType switch {
			"Bushy" => PlantGrowthConfig.Bushy,
			"TallSparse" => PlantGrowthConfig.TallSparse,
			"Vine" => PlantGrowthConfig.Vine,
			_ => new PlantGrowthConfig()
		};

		if (branch is Stem stem) {
			stem.Configure(growthConfig);
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

	private void CleanupNode(Node node)
	{
		if (node == null || !IsInstanceValid(node))
			return;

		// Disconnect any signals
		node.GetSignalConnectionList().ForEach(connection => {
			node.Disconnect(connection["signal"].AsString(), connection["callable"].AsCallable());
		});

		// Remove from parent if it has one
		if (node.GetParent() != null)
			node.GetParent().RemoveChild(node);

		// Free any resources
		if (node is MeshInstance3D meshInstance)
		{
			var mesh = meshInstance.Mesh;
			if (mesh != null)
			{
				meshInstance.Mesh = null;
				if (mesh.GetRid().IsValid())
				{
					mesh.ClearSurfaces();
					mesh.ResourceLocalToScene = false;
				}
			}
			
			var material = meshInstance.GetSurfaceOverrideMaterial(0);
			if (material != null)
			{
				meshInstance.SetSurfaceOverrideMaterial(0, null);
				if (material.GetRid().IsValid())
				{
					material.ResourceLocalToScene = false;
				}
			}
		}

		// Queue the node for deletion
		node.QueueFree();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		// Clean up all stems
		foreach (var stem in GetAllStems().ToList())
		{
			CleanupNode(stem);
		}

		// Clean up any remaining children
		foreach (var child in GetChildren().ToList())
		{
			CleanupNode(child);
		}

		// Clean up mesh instance if we have one
		if (this is MeshInstance3D meshInstance)
		{
			var mesh = meshInstance.Mesh;
			if (mesh != null)
			{
				meshInstance.Mesh = null;
				if (mesh.GetRid().IsValid())
				{
					mesh.ClearSurfaces();
					mesh.ResourceLocalToScene = false;
				}
			}
			
			var material = meshInstance.GetSurfaceOverrideMaterial(0);
			if (material != null)
			{
				meshInstance.SetSurfaceOverrideMaterial(0, null);
				if (material.GetRid().IsValid())
				{
					material.ResourceLocalToScene = false;
				}
			}
		}
	}

	public override void _Notification(int what)
	{
		if (what == NotificationPredelete)
		{
			// Ensure cleanup when the node is about to be deleted
			foreach (var stem in GetAllStems().ToList())
			{
				CleanupNode(stem);
			}

			foreach (var child in GetChildren().ToList())
			{
				CleanupNode(child);
			}
		}
	}
}
