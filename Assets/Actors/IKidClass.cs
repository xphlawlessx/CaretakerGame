namespace Actors
{
    public interface IKidClass
    {
        GroupBehaviour GroupBehaviour { get; set; }
        Clothing Clothing { get; set; }

        int AmbientDamage { get; set; }
    }

    public enum GroupBehaviour
    {
        LoneWolf,
        CoDependant
    }

    public enum DestructionBehaviour
    {
        Shortest,
        Longest
    }

    public enum Clothing
    {
        SchoolClothes,
        Sneakers
    }
}