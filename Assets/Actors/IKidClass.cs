namespace Actors
{
    public interface IKidClass
    {
        bool IsLeader { get; set; }
        int Leadership { get; set; }
        int Lonewolf { get; set; }
        Clothing Clothing { get; set; }

        int AmbientDamage { get; set; }
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