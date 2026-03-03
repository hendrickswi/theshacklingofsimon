using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.Controllers.Gamepad;

public record struct JoystickInputRegion
{
    public Vector2 Target;
    public float MinimumDotProduct;
    public float DeadzoneSquared;

    public JoystickInputRegion(Vector2 target, float toleranceDegrees, float deadzone)
    {
        if (target.LengthSquared() > float.Epsilon)
        {
            target.Normalize();
        }
        Target = target;

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