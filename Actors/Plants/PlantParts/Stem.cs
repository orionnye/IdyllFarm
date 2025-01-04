using Godot;
using System;

public partial class Stem : Node3D
{
	[Export] public float initialGirth = 0.1f;
	[Export] public float initialLength = 1.0f;
	[Export] public int growthIterations = 2;
	[Export] public float phototropismStrength = 0.3f;
	[Export] public float branchingProbability = 0.7f;
	[Export] public float minHeightGrowthFactor = 0.2f;  // No growth below 20% of max height
	[Export] public PackedScene stemScene;
	[Export] public PackedScene leafScene;
	
	private readonly int defaultGrowthIterations = 2;
	private LSystem lSystem;
	private IGrowthRules growthRules;

	public override void _Ready() {
		stemScene = GD.Load<PackedScene>("res://Actors/Plants/PlantParts/Stem.tscn");
		leafScene = GD.Load<PackedScene>("res://Actors/Plants/PlantParts/Leaf.tscn");
		
		// Initialize L-system with default rules
		growthRules = new PlantGrowthRules {
			Axiom = "F",
			Productions = new() {
				{ 'F', "F[+F]F[-F][F]" }
			},
			AngleDelta = Mathf.Pi / 7,
			GirthFactor = 0.8f,
			LengthFactor = 0.9f,
			PhototropismStrength = phototropismStrength,
			BranchingProbability = branchingProbability,
			MinHeightGrowthFactor = minHeightGrowthFactor
		};
		
		lSystem = new LSystem(growthRules);
		Grow();
	}

	public void Reset() {
		growthIterations = defaultGrowthIterations;
		Grow();
	}

	public void Grow() {
		// Clear existing children
		foreach (var child in GetChildren()) {
			if (child is not MeshInstance3D) {
				RemoveChild(child);
				child.QueueFree();
			}
		}

		// Generate new growth pattern
		lSystem.Generate(growthIterations);
		var segments = lSystem.Interpret(initialLength, initialGirth);

		// Create meshes for each segment
		foreach (var (start, end, girth) in segments) {
			var segmentMesh = new MeshInstance3D();
			var cylinder = new CylinderMesh {
				TopRadius = girth * growthRules.GirthFactor,
				BottomRadius = girth,
				Height = (end - start).Length()
			};

			// Set material
			var material = new StandardMaterial3D {
				AlbedoColor = new Color(0.42f, 0.12f, 0.05f) // Woody brown color
			};
			
			segmentMesh.Mesh = cylinder;
			segmentMesh.MaterialOverride = material;
			
			// Position and rotate the segment
			var midpoint = (start + end) / 2;
			segmentMesh.Position = midpoint;
			
			// Calculate rotation to point from start to end
			var direction = (end - start).Normalized();
			var rotationAxis = Vector3.Up.Cross(direction);
			var angle = Vector3.Up.AngleTo(direction);
			if (rotationAxis.LengthSquared() > 0.001f) {
				segmentMesh.RotateObjectLocal(rotationAxis.Normalized(), angle);
			}
			
			AddChild(segmentMesh);
		}

		// Add leaves, more likely at higher points and branch tips
		foreach (var (start, end, girth) in segments) {
			float heightFactor = end.Y / 10f; // Assuming 10 units as max height
			
			// No leaves below minimum height threshold
			if (heightFactor < minHeightGrowthFactor) {
				continue;
			}
			
			if (GD.Randf() < 0.3f * (0.5f + 0.5f * heightFactor)) { // More leaves higher up
				var leaf = leafScene.Instantiate<Node3D>();
				AddChild(leaf);
				leaf.Position = end;
				leaf.Scale = Vector3.One * (girth * 5); // Scale leaves based on girth
				
				// Rotate leaves to face more upward at higher points
				var upwardBias = Mathf.Lerp(0f, 0.5f, heightFactor);
				leaf.Rotation = new Vector3(
					GD.Randf() * Mathf.Pi * (1f - upwardBias),
					GD.Randf() * Mathf.Pi,
					GD.Randf() * Mathf.Pi * (1f - upwardBias)
				);
			}
		}
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
}
