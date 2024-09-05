using Godot;
using System;

public partial class Mouse : RigidBody3D
{
	// Mouse Marker
	[Export] public MeshInstance3D mouseMarker;
	[Export] public Node3D mouseRotation;

	// lerpSpeed
	[Export] public float lerpRate = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}
	// public Vector3 GetMouseRotation() {
	// 	// Get player rotation based on mouse
	// 	Vector2 mouseRelative = GetMouseRelative();
	// 	Vector3 mouseInSpace = new Vector3(mouseRelative.X, GlobalPosition.Y, mouseRelative.Y);

	// 	return mouseInSpace;
	// }
	// public Vector2 GetMouseRelative() {
	// 	// Gets the mouse position on screen
	// 	Vector2 mouseInViewport = GetViewport().GetMousePosition();
	// 	// Gets the player Position in ViewPort
	// 	Vector2 userInViewport = GetViewport().GetCamera3D().UnprojectPosition(GlobalPosition);
	// 	// Compares mouse position in viewport compared to playerPos in viewport
	// 	Vector2 mouseRelative = userInViewport - mouseInViewport;
	// 	// return the comparison
	// 	return mouseRelative;
	// }
	public Vector3 MouseRayCast() {
		// Fix the Projection to take into account rotation and physical offset, currently the marker is off by these metrics
		var spaceState = GetWorld3D().DirectSpaceState;
		var mousePos = GetViewport().GetMousePosition();
		var origin = GetViewport().GetCamera3D().ProjectRayOrigin(mousePos);
		var end = origin+GetViewport().GetCamera3D().ProjectRayNormal(mousePos)*1000f;
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		var result = spaceState.IntersectRay(query);
		
		if (result.Count > 0) { return (Vector3)result["position"]; }
		return new Vector3();
	}
	// public void trackMouse() {
	// 	Vector3 mousePos = MouseRayCast();
	// 	marker1.GlobalPosition = mousePos;
	// 	aimGhost.LookAt(mousePos);
	// 	trackAngle(aimGhost.GlobalRotation.Y);
	// }
	public void LookAt() {
		// return mouse direction and ray cast from center to mouse
		mouseRotation.LookAt(mouseMarker.GlobalPosition);
		// Lerp Rate needs to be applied because 3rd Person controllers snapping look odd
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// Assign marker to the mouse direction
		// mouseMarker.GlobalPosition = MouseRayCast();
		// LookAt();
	}
}
