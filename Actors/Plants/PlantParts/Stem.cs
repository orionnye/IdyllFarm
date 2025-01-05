using Godot;
using System;
using System.Linq;

public partial class Stem : Node3D
{
	[Export] public PackedScene stemScene;
	[Export] public PackedScene leafScene;
	
	private readonly int defaultGrowthIterations = 2;
	private int growthIterations = 2;
	private LSystem lSystem;
	private PlantGrowthConfig config;
	private List<Node3D> leaves = new();

	private record struct BoundingSphere(Vector3 Center, float Radius);

	public override void _Ready() {
		GD.Print("Loading stem and leaf scenes...");
		stemScene = GD.Load<PackedScene>("res://Actors/Plants/PlantParts/Stem.tscn");
		leafScene = GD.Load<PackedScene>("res://Actors/Plants/PlantParts/Leaf.tscn");
		if (leafScene == null) {
			GD.PrintErr("Failed to load leaf scene!");
		} else {
			GD.Print("Leaf scene loaded successfully");
		}
	}

	public void Configure(PlantGrowthConfig config) {
		GD.Print("Configuring stem with growth config...");
		this.config = config;
		
		// Initialize L-system with configuration
		var rules = new PlantGrowthRules {
			Axiom = config.Axiom,
			Productions = new() {
				{ 'F', config.BranchingRule }
			},
			AngleDelta = config.AngleDelta,
			GirthFactor = config.GirthFactor,
			LengthFactor = config.LengthFactor,
			PhototropismStrength = config.PhototropismStrength,
			BranchingProbability = config.BranchingProbability,
			MinHeightGrowthFactor = config.MinHeightGrowthFactor
		};
		
		lSystem = new LSystem(rules);
		GD.Print("L-system initialized");
	}

	private BoundingSphere CalculateBoundingSphere(List<LeafInfo> leafPositions) {
		if (leafPositions.Count == 0) {
			return new BoundingSphere(Vector3.Zero, 0);
		}

		// Calculate center (centroid)
		var center = Vector3.Zero;
		foreach (var leaf in leafPositions) {
			center += leaf.Position;
		}
		center /= leafPositions.Count;

		// Calculate radius (maximum distance from center to any leaf)
		float radius = 0f;
		foreach (var leaf in leafPositions) {
			float distance = leaf.Position.DistanceTo(center);
			radius = Mathf.Max(radius, distance);
		}

		return new BoundingSphere(center, radius);
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
		node._ExitTree();
	}

	private void CullInnerLeaves()
	{
		if (leaves.Count <= 1)
			return;

		GD.Print($"Initial leaf count: {leaves.Count}");
		int leavesToKeep = leaves.Count / 2;
		GD.Print($"Culling leaves: total={leaves.Count}, keeping={leavesToKeep}");

		var leafPositions = leaves.Select(leaf => leaf.GlobalPosition).ToList();
		var scores = new List<(int index, float score)>();

		// Calculate scores for each leaf
		for (int i = 0; i < leaves.Count; i++)
		{
			var leaf = leaves[i];
			if (leaf == null || !IsInstanceValid(leaf))
				continue;

			var pos = leaf.GlobalPosition;
			float score = 0;

			// Score based on distance from other leaves
			for (int j = 0; j < leaves.Count; j++)
			{
				if (i != j && leaves[j] != null && IsInstanceValid(leaves[j]))
				{
					var otherPos = leaves[j].GlobalPosition;
					score += 1.0f / (1.0f + pos.DistanceTo(otherPos));
				}
			}

			scores.Add((i, score));
		}

		// Sort by score (higher scores are more crowded)
		scores.Sort((a, b) => b.score.CompareTo(a.score));

		// Get indices to remove (most crowded leaves)
		var indicesToRemove = scores
			.Skip(leavesToKeep)
			.Select(x => x.index)
			.OrderByDescending(x => x)
			.ToList();

		GD.Print($"Indices to remove: {string.Join(", ", indicesToRemove)}");

		// Remove leaves
		foreach (var index in indicesToRemove)
		{
			if (index < leaves.Count)
			{
				var leaf = leaves[index];
				if (leaf != null && IsInstanceValid(leaf))
				{
					GD.Print($"Removing leaf at index: {index}");
					CleanupNode(leaf);
					leaves[index] = null;
				}
			}
		}

		// Clean up the list
		leaves.RemoveAll(leaf => leaf == null || !IsInstanceValid(leaf));
		GD.Print($"Remaining leaves after culling: {leaves.Count}");
	}

	public void Reset() {
		growthIterations = defaultGrowthIterations;
		Grow();
	}

	public List<Vector3> GetLeafPositions() {
		return lSystem.GetLeafPositions().Select(l => l.Position).ToList();
	}

	public void RemoveLeaves() {
		foreach (var leaf in leaves) {
			if (leaf != null && IsInstanceValid(leaf)) {
				RemoveChild(leaf);
				leaf.QueueFree();
			}
		}
		leaves.Clear();
	}

	public void Grow() {
		if (config == null) {
			GD.PrintErr("Stem not configured! Call Configure() before growing.");
			return;
		}

		// Remove all existing nodes and free them
		foreach (var child in GetChildren()) {
			if (child != null && IsInstanceValid(child)) {
				RemoveChild(child);
				child.QueueFree();
			}
		}
		RemoveLeaves();

		// Generate new growth pattern
		lSystem.Generate(growthIterations);
		var segments = lSystem.Interpret(config.InitialLength, config.InitialGirth);

		// Create meshes for each segment
		foreach (var (start, end, girth) in segments) {
			var segmentMesh = new MeshInstance3D();
			var cylinder = new CylinderMesh {
				TopRadius = girth * config.GirthFactor,
				BottomRadius = girth,
				Height = (end - start).Length()
			};

			var material = new StandardMaterial3D {
				AlbedoColor = new Color(0.42f, 0.12f, 0.05f)
			};
			
			segmentMesh.Mesh = cylinder;
			segmentMesh.MaterialOverride = material;
			
			var midpoint = (start + end) / 2;
			segmentMesh.Position = midpoint;
			
			var direction = (end - start).Normalized();
			var rotationAxis = Vector3.Up.Cross(direction);
			var angle = Vector3.Up.AngleTo(direction);
			if (rotationAxis.LengthSquared() > 0.001f) {
				segmentMesh.RotateObjectLocal(rotationAxis.Normalized(), angle);
			}
			
			AddChild(segmentMesh);
		}

		// Get potential leaf positions and cull inner leaves
		var leafPositions = lSystem.GetLeafPositions();
		GD.Print($"Initial leaf positions: {leafPositions.Count}");
		
		var culledLeafPositions = CullInnerLeaves(leafPositions);
		GD.Print($"After culling {config.InnerLeafCullRate * 100}%: {culledLeafPositions.Count} leaves remain");

		// Add remaining leaves
		int actualLeaves = 0;
		foreach (var leafInfo in culledLeafPositions) {
			float leafChance = config.LeafProbability * 
				(0.5f + 0.5f * leafInfo.HeightFactor) *
				(leafInfo.IsTip ? 1.5f : 1.0f);
			
			if (GD.Randf() < leafChance) {
				if (leafScene == null) {
					GD.PrintErr("Leaf scene is null!");
					continue;
				}
				
				actualLeaves++;
				var leaf = leafScene.Instantiate<Node3D>();
				AddChild(leaf);
				leaves.Add(leaf);
				
				leaf.Position = leafInfo.Position;
				leaf.Scale = Vector3.One * (leafInfo.Girth * 5);
				
				var upwardBias = Mathf.Lerp(0f, 0.5f, leafInfo.HeightFactor);
				leaf.Rotation = new Vector3(
					GD.Randf() * Mathf.Pi * (1f - upwardBias),
					GD.Randf() * Mathf.Pi,
					GD.Randf() * Mathf.Pi * (1f - upwardBias)
				);
			}
		}
		GD.Print($"Final leaf count after probability: {actualLeaves}");
	}

	public void IncreaseComplexity() {
		growthIterations++;
		Grow();
	}

	public void DecreaseComplexity() {
		if (growthIterations > 1) {
			growthIterations--;
			Grow();
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		// Clean up all leaves
		foreach (var leaf in leaves.ToList())
		{
			CleanupNode(leaf);
		}
		leaves.Clear();
		
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
}
