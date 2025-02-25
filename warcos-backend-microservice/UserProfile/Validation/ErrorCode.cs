namespace UserProfile.Validation;

public enum ErrorCode
{
    // User 
    UnknownUser,
    UserAlreadyExists,
    
    // Level 
    UnknownLevel,
    LevelAlreadyExists,
    CannotDeleteOwnedLevel,
    
    // Item
    ItemAlreadyExists,
    UnknownItem,
    CannotDeleteOwnedItem
}