using Godot;

// Immutable configuration for plant growth
public record PlantGrowthConfig {
    // Initial dimensions
    public float InitialGirth { get; init; } = 0.1f;
    public float InitialLength { get; init; } = 1.0f;
    
    // Growth behavior
    public float PhototropismStrength { get; init; } = 0.3f;
    public float BranchingProbability { get; init; } = 0.7f;
    public float MinHeightGrowthFactor { get; init; } = 0.25f;
    public float GirthFactor { get; init; } = 0.8f;
    public float LengthFactor { get; init; } = 0.9f;
    public float AngleDelta { get; init; } = Mathf.Pi / 7;
    
    // Leaf parameters
    public float LeafProbability { get; init; } = 0.3f;
    public float InnerLeafCullRate { get; init; } = 0.85f;
    
    // L-system rules
    public string Axiom { get; init; } = "F";
    public string BranchingRule { get; init; } = "F[+F]F[-F][F]";

    // Preset configurations
    public static PlantGrowthConfig Bushy => new() {
        InitialGirth = 0.15f,
        InitialLength = 0.8f,
        PhototropismStrength = 0.2f,
        BranchingProbability = 0.8f,
        MinHeightGrowthFactor = 0.25f,
        GirthFactor = 0.85f,
        LengthFactor = 0.85f,
        AngleDelta = Mathf.Pi / 5,
        LeafProbability = 0.4f,
        InnerLeafCullRate = 0.98f
    };

    public static PlantGrowthConfig TallSparse => new() {
        InitialGirth = 0.08f,
        InitialLength = 1.2f,
        PhototropismStrength = 0.4f,
        BranchingProbability = 0.5f,
        MinHeightGrowthFactor = 0.3f,
        GirthFactor = 0.75f,
        LengthFactor = 0.95f,
        AngleDelta = Mathf.Pi / 8,
        LeafProbability = 0.2f,
        InnerLeafCullRate = 0.98f
    };

    public static PlantGrowthConfig Vine => new() {
        InitialGirth = 0.05f,
        InitialLength = 1.5f,
        PhototropismStrength = 0.1f,
        BranchingProbability = 0.4f,
        MinHeightGrowthFactor = 0.2f,
        GirthFactor = 0.9f,
        LengthFactor = 0.98f,
        AngleDelta = Mathf.Pi / 4,
        LeafProbability = 0.35f,
        InnerLeafCullRate = 0.98f,
        BranchingRule = "F[+F][-F]F"
    };
} 