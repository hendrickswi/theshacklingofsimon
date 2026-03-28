using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Players;

public class PlayerInputBuffer
{
    public Vector2 Movement { get; private set; }
    public Vector2 PrimaryAttack { get; private set; }
    public Vector2 SecondaryAttack { get; private set; }

    public PlayerInputBuffer()
    {
        Movement = Vector2.Zero;
        PrimaryAttack = Vector2.Zero;
        SecondaryAttack = Vector2.Zero;
    }

    public void AddMovement(Vector2 direction)
    {
        Movement += direction;
    }

    public void AddPrimaryAttack(Vector2 direction)
    {
        PrimaryAttack += direction;
    }

    public void AddSecondaryAttack(Vector2 direction)
    {
        SecondaryAttack += direction;
    }
    
    public Vector2 ConsumeMovement()
    {
        if (Movement.LengthSquared() <= float.Epsilon)
        {
            return Vector2.Zero;
        }

        Vector2 result = Movement;
        Movement = Vector2.Zero;
        return result;
    }

    public Vector2 ConsumePrimaryAttack()
    {
        if (PrimaryAttack.LengthSquared() <= float.Epsilon)
        {
            return Vector2.Zero;
        }

        Vector2 result = PrimaryAttack;
        PrimaryAttack = Vector2.Zero;
        return result;
    }

    public Vector2 ConsumeSecondaryAttack()
    {
        if (SecondaryAttack.LengthSquared() <= float.Epsilon)
        {
            return Vector2.Zero;
        }

        Vector2 result = SecondaryAttack;
        SecondaryAttack = Vector2.Zero;
        return result;
    }
}