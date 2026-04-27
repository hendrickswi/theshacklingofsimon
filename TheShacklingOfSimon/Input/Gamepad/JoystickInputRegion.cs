#region

using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

#endregion

namespace TheShacklingOfSimon.Input.Gamepad;

public readonly record struct JoystickInputRegion
{
    // The actual properties used
    public Vector2 Target { get; }
    [JsonIgnore]
    public float MinimumDotProduct { get; }
    [JsonIgnore]
    public float DeadzoneSquared { get; }
    
    // Exposed properties for JSON deserialization (should remain unused)
    public float ToleranceDegrees { get; init; }
    public float Deadzone { get; init; }

    [JsonConstructor]
    public JoystickInputRegion(Vector2 target, float toleranceDegrees, float deadzone)
    {
        if (target.LengthSquared() > float.Epsilon)
        {
            target.Normalize();
        }
        
        Target = target;
        ToleranceDegrees = toleranceDegrees;
        Deadzone = deadzone;

        double halfAngleInRadians = MathHelper.ToRadians(toleranceDegrees * 0.5f);
        MinimumDotProduct = (float)Math.Cos(halfAngleInRadians);
        DeadzoneSquared = deadzone * deadzone;
    }

    public bool Contains(Vector2 input)
    {
        if (input.LengthSquared() < DeadzoneSquared)
        {
            return false;
        }

        Vector2 normalizedInput = Vector2.Normalize(input);
        
        // The dot product measures how aligned the vectors are
        float dotProduct = Vector2.Dot(Target, normalizedInput);
        
        // The vectors must be aligned enough to return true
        return dotProduct >= MinimumDotProduct;
    }
}