using Godot;
using System;

[Tool]
public partial class MultiItem : Node3D
{
    [Export] public MeshInstance3D meshSource;
    [Export] public double timer = 10;
    [Export] public MultiMeshInstance3D multiMeshInstance;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        if (multiMeshInstance == null) {
            multiMeshInstance = new MultiMeshInstance3D();
            AddChild(multiMeshInstance);
        }
        RepopulateMultiMesh();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
		// Rotation shows tool modifier is working
        RotationDegrees += new Vector3(0, 1, 0);

		// Timer recreates the MultiMesh every 10 seconds
        timer -= delta;
        if (timer < 0) {
            timer = 10;
            RepopulateMultiMesh();
        }
    }

    private void RepopulateMultiMesh() {
        if (meshSource == null) return;
		GD.Print("Repopulating the scene");
        MultiMesh multiMesh = new MultiMesh();
        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        multiMesh.InstanceCount = 10; // Example instance count

        for (int i = 0; i < multiMesh.InstanceCount; i++) {
            Transform3D transform = new Transform3D();
            transform.Origin = new Vector3(i * 2, 0, 0); // Example positioning
            multiMesh.SetInstanceTransform(i, transform);
        }

        multiMeshInstance.Multimesh = multiMesh;
        multiMeshInstance.Multimesh.Mesh = meshSource.Mesh;
    }
}
