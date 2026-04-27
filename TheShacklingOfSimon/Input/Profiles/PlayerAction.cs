namespace TheShacklingOfSimon.Input.Profiles;

public enum PlayerAction
{
    // Movement
    MoveUp, MoveDown, MoveLeft, MoveRight,
    
    // Attacking
    PrimaryAttackUp, PrimaryAttackLeft, PrimaryAttackRight, PrimaryAttackDown,
    SecondaryAttackUp, SecondaryAttackLeft, SecondaryAttackRight, SecondaryAttackDown,
    
    // Items
    UseActiveItem,
    
    // Rotary controls
    NextPrimaryWeapon, PreviousPrimaryWeapon, NextSecondaryWeapon, PreviousSecondaryWeapon,
    PreviousActiveItem, NextActiveItem,
    
    // Miscellaneous
    Pause, Resume, Quit, Reset,
    
    // Menu controls
    MenuUp, MenuDown, MenuLeft, MenuRight, MenuConfirm, MenuCancel,
    
}