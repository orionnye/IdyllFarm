using Godot;
using System;

public partial class Hands : Node3D
{
	// PlayerCam cam;
	[Export] public Node3D grabber;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// starting values

		// reference values
		// cam = GetParent<OldController>().cam;

		// starting mutations
		foreach (var item in grabber.GetChildren()) {
			item.Set("freeze", true);
		}
	}
	// Reference function
	public Boolean isHolding() { return grabber.GetChildCount() > 0; }

	//                    |
	//                    |
	// Utility Functions \|/
	// 					  v


	// Uses the object
	public void Use(Item item) {
		if (isHolding()) {
			if (item.active) {
				item.Deactivate();
			} else {
				item.Activate();
			}
		}
	}
	public void HoldLerp(Item item) {
		float lerpWeight = 0.9f;
		item.Position = item.Position.Lerp(item.heldPosition, lerpWeight);
	}
	
	// Grab Object
	public void Grab(Item item) {
		if (!isHolding()) {
			item.Reparent(grabber, true);
			item.Position = item.heldPosition;
			item.Rotation = item.heldRotation;
			item.Set("freeze", true);
			// item.held = true;
		}
	}
	// Drop Object Held in hands
	public void Drop() {
		// GD.Print("Trying to drop");
		if (isHolding()) {
			Node3D item = (Node3D)grabber.GetChild(0);
			item.Set("freeze", false);
			item.Reparent(GetTree().Root, true);
		}
	}

	public Item getTargetItem() {
		foreach (Item item in GetTree().GetNodesInGroup("Item")) {
			if ((item.GlobalPosition - GlobalPosition).Length() < 2) {
				return item;
			}
		}
		return null;
	}
	public void toggleHold() {
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}