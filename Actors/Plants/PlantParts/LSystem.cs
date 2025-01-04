using Godot;
using System;
using System.Collections.Generic;

// Represents a single state in the L-system growth
public interface IGrowthState {
    Vector3 Position { get; }
    Vector3 Direction { get; }
    float Girth { get; }
    float Height { get; }  // Added height for light calculation
    float DistanceFromTip { get; }  // Distance from the current branch tip
}

// Immutable growth state implementation
public record GrowthState(Vector3 Position, Vector3 Direction, float Girth, float Height, float DistanceFromTip) : IGrowthState;

// Rules for the L-system
public interface IGrowthRules {
    string Axiom { get; }
    Dictionary<char, string> Productions { get; }
    float AngleDelta { get; }
    float GirthFactor { get; }
    float LengthFactor { get; }
    float PhototropismStrength { get; }  // How strongly branches grow towards light
    float BranchingProbability { get; }  // Base probability of branching
    float MinHeightGrowthFactor { get; } // Minimum height required for growth (0.2 = 20%)
}

// Immutable growth rules implementation
public record PlantGrowthRules : IGrowthRules {
    public string Axiom { get; init; } = "F";
    public Dictionary<char, string> Productions { get; init; } = new() {
        { 'F', "F[+F][-F][F]" }
    };
    public float AngleDelta { get; init; } = Mathf.Pi / 7;
    public float GirthFactor { get; init; } = 0.8f;
    public float LengthFactor { get; init; } = 0.9f;
    public float PhototropismStrength { get; init; } = 0.3f;  // 30% bias towards vertical
    public float BranchingProbability { get; init; } = 0.7f;  // 70% base chance to branch
    public float MinHeightGrowthFactor { get; init; } = 0.2f; // No growth below 20% of max height
}

// The main L-system implementation
public class LSystem {
    private readonly IGrowthRules rules;
    private string currentState;
    private readonly Stack<IGrowthState> stateStack = new();
    private readonly List<(Vector3 start, Vector3 end, float girth)> segments = new();
    private float maxHeight = 0f;  // Track maximum height for light calculation

    public LSystem(IGrowthRules rules) {
        this.rules = rules;
        this.currentState = rules.Axiom;
    }

    public void Generate(int iterations) {
        // Apply production rules
        for (int i = 0; i < iterations; i++) {
            var nextState = "";
            foreach (char c in currentState) {
                if (c == 'F') {
                    var currentState = stateStack.Peek();
                    float heightFactor = maxHeight > 0 ? currentState.Height / maxHeight : 1f;
                    
                    // No growth below minimum height threshold
                    if (heightFactor < rules.MinHeightGrowthFactor) {
                        nextState += "F";  // Just continue existing branch
                        continue;
                    }

                    // Calculate growth probability based on height and distance from tip
                    float tipFactor = Mathf.Max(0f, 1f - currentState.DistanceFromTip);
                    float branchChance = rules.BranchingProbability * 
                        (0.5f + 0.5f * heightFactor) * // Height factor
                        (0.3f + 0.7f * tipFactor);     // Tip factor - more growth near tips
                    
                    if (GD.Randf() < branchChance) {
                        nextState += rules.Productions['F'];
                    } else {
                        nextState += "F";  // Just grow forward without branching
                    }
                } else {
                    nextState += c;
                }
            }
            currentState = nextState;
        }
    }

    public List<(Vector3 start, Vector3 end, float girth)> Interpret(float initialLength = 1.0f, float initialGirth = 0.1f) {
        segments.Clear();
        stateStack.Clear();
        maxHeight = 0f;
        
        var currentPos = Vector3.Zero;
        var currentDir = Vector3.Up;
        var currentGirth = initialGirth;
        var length = initialLength;
        
        // Initialize with root state
        stateStack.Push(new GrowthState(currentPos, currentDir, currentGirth, 0f, 0f));

        foreach (char c in currentState) {
            var currentState = stateStack.Peek();
            float heightFactor = maxHeight > 0 ? currentState.Height / maxHeight : 1f;
            
            switch (c) {
                case 'F':
                    // Apply phototropism - bias towards vertical based on height
                    var targetDir = Vector3.Up;
                    var phototropicDir = currentState.Direction.Lerp(targetDir, rules.PhototropismStrength * heightFactor);
                    phototropicDir = phototropicDir.Normalized();
                    
                    var newPos = currentState.Position + phototropicDir * length;
                    segments.Add((currentState.Position, newPos, currentState.Girth));
                    
                    // Update maximum height and current state
                    maxHeight = Mathf.Max(maxHeight, newPos.Y);
                    stateStack.Pop();
                    stateStack.Push(new GrowthState(newPos, phototropicDir, currentState.Girth, newPos.Y, 0f));
                    break;
                    
                case '+':
                    var rotatedDirPlus = currentState.Direction.Rotated(
                        Vector3.Right, 
                        rules.AngleDelta * (1f - heightFactor * 0.5f)  // Reduce angle at higher points
                    );
                    stateStack.Pop();
                    stateStack.Push(new GrowthState(
                        currentState.Position, 
                        rotatedDirPlus, 
                        currentState.Girth, 
                        currentState.Height,
                        currentState.DistanceFromTip + length
                    ));
                    break;
                    
                case '-':
                    var rotatedDirMinus = currentState.Direction.Rotated(
                        Vector3.Right, 
                        -rules.AngleDelta * (1f - heightFactor * 0.5f)  // Reduce angle at higher points
                    );
                    stateStack.Pop();
                    stateStack.Push(new GrowthState(
                        currentState.Position, 
                        rotatedDirMinus, 
                        currentState.Girth, 
                        currentState.Height,
                        currentState.DistanceFromTip + length
                    ));
                    break;
                    
                case '[':
                    var currentStateSnapshot = stateStack.Peek();
                    stateStack.Push(new GrowthState(
                        currentStateSnapshot.Position,
                        currentStateSnapshot.Direction,
                        currentStateSnapshot.Girth * rules.GirthFactor,
                        currentStateSnapshot.Height,
                        0f  // Reset distance from tip for new branch
                    ));
                    length *= rules.LengthFactor;
                    break;
                    
                case ']':
                    if (stateStack.Count > 1) {  // Keep at least root state
                        stateStack.Pop();
                        length /= rules.LengthFactor;
                    }
                    break;
            }
        }

        return segments;
    }
} 