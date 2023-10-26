namespace AmeisenBotX.Wow.Objects.Enums
{
    /// <summary>
    /// Represents the different types of click-to-move actions in World of Warcraft.
    /// </summary>
    public enum WowClickToMoveType
    {
        FaceTarget = 1,      // LeftClick??
        FaceDestination = 2,
        Stop = 3,            // results in UI exception
        Move = 4,
        InteractNpc = 5,
        Loot = 6,
        InteractObject = 7,
        FaceOther = 8,
        Skin = 9,
        AttackPosition = 10,
        AttackGuid = 11,
        ConstantFace = 12,

        // ----- Values below may be wrong ---->
        Halted = 13,

        None = 14,

        // 15
        Attack = 16,

        // 17 18
        WalkAndRotate = 19
    }
}