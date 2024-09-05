using Godot;
using System;

public partial class CameraArm : Node3D
{
	// Debugging tools
	[Export] public MeshInstance3D ghostArm;
	[Export] public MeshInstance3D ghostCam;
	[Export] public Node3D camHost;
	[Export] public Vector3 hostOffset;
	[Export] public Vector3 offset;
	[Export] public Node3D focus;
	public float camLerp = 0.01f;
	public float armLerp = 0.005f;
	public float armDeadZone = (float)Math.PI*0.2f;
	public float camDeadZone = (float)Math.PI*0.01f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Input.MouseMode = (Input.MouseModeEnum)0;
		hostOffset = camHost.Position;
		offset = Position;
	}
	public Vector3 RayCastVector(Vector2 pos) {
		// Fix the Projection to take into account rotation and physical offset, currently the marker is off by these metrics
		var spaceState = GetWorld3D().DirectSpaceState;
		var mousePos = pos;
		var origin = GetViewport().GetCamera3D().ProjectRayOrigin(mousePos);
		var end = origin+GetViewport().GetCamera3D().ProjectRayNormal(mousePos)*1000f;
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		var result = spaceState.IntersectRay(query);
		
		if (result.Count > 0) { return (Vector3)result["position"]; }
		return new Vector3();
	}
	public float trackAngle(Node3D node, float targetAngle) {
		// Define Rotation for decision
		float currentRot = node.GlobalRotation.Y;

		// INNER BOUNDS Calculations
		// Base Assumption for Inner Bounds is that both are opposite sides of 0
		// Check for both sides being the same and recalculate innerBounds
		float diff = (float)(currentRot - targetAngle);
		// float cycledDiff = diff%(float)Math.PI;
		// Normalize to the range [-π, π]
		diff = (float)Math.Atan2(Math.Sin(diff), Math.Cos(diff));
		return -diff;
	}
	public void updateGhostCam() {
		ghostCam.Position = camHost.Position;
	}
	public void lookAtWithGhostArm(Vector3 point) {
		ghostArm.LookAt(point);
		// Manually apply rotation acccording to screen results
	}
	public void lookAtWithGhostCam(Vector3 point) {
		ghostCam.LookAt(point);
	}
	public void lookWithGhost() {
		lookAtWithGhostArm(focus.GlobalPosition);
		lookAtWithGhostCam(focus.GlobalPosition);
	}
	// Arm faces Focus
	public void lookWithArmAt() {
		// get rotation to lookAt point
		Vector3 desiredRot = ghostArm.GlobalRotation;
		// Vector3 currentRot = this.GlobalRotation;

		// compare against current rotation
		float diff = trackAngle(this, desiredRot.Y);
		if (Math.Abs(diff) > armDeadZone) {
			// lerp current rotation closer to the desired rot
			Rotate(new Vector3(0, 1, 0), diff*armLerp);
		}
	}
	// Camera faces Focus
	public void lookWithCamAt() {
		// get rotation to lookAt point
		Vector3 desiredRot = ghostCam.GlobalRotation;
		Vector3 currentRot = camHost.GlobalRotation;
		// compare against current rotation
		float diff = trackAngle(camHost, desiredRot.Y);
		// This math is fucked, still good code from Turret for this.
		if (Math.Abs(diff) > camDeadZone) {
			// lerp current rotation closer to the desired rot
			camHost.Rotate(new Vector3(0, 1, 0), diff*camLerp);
		}
	}
	public void lookWithCam() {
		lookWithArmAt();
		lookWithCamAt();
	}
	public void WarpMousePos() {
	}

	public void SetOffset(Vector3 next = new Vector3()) {
		if (next == new Vector3())
			next = hostOffset;
		// Lerp between current pos and desired
		Node3D parent = (Node3D)GetParent();
		camHost.GlobalPosition = parent.GlobalPosition.Lerp(hostOffset + parent.GlobalPosition, 0.9f);
		GlobalPosition = parent.GlobalPosition.Lerp(offset + parent.GlobalPosition, 0.9f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// update Ghosts, should check if ghost is visible
		// Input.WarpMouse()
		lookWithGhost();
		lookWithCam();
		SetOffset();
		// Rotate the camera using the arm
		// make camera face the focus
	}
	public override void _Input(InputEvent @event) {
		// Mouse in viewport coordinates.
		// if (@event is InputEventMouseMotion eventMouseButton)
			// GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
		if (@event is InputEventMouseMotion eventMouseMotion) {
			// GD.Print("Mouse Motion at: ", eventMouseMotion.Position);
			focus.GlobalPosition = RayCastVector(eventMouseMotion.Position);
		}

		// Print the size of the viewport.
		// GD.Print("Viewport Resolution is: ", GetViewport().GetVisibleRect().Size);
	}
}
