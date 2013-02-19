namespace LaddersAndSlides_GameEngine.Domain
{
    using System;

    public class Player
    {
        public string Name { get; set; }
        public Uri ImageUri { get; set; }
        public int CurrentTileNumber { get; set; }
        public int CurrentMovesRemaining { get; set; }
        public int TurnOrder { get; set; }
        public bool TurnInProcess { get; set; }
        public bool CurrentlyOnAlternateRow { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Player) obj);
        }

        protected bool Equals(Player other)
        {
            return string.Equals(Name, other.Name) && TurnOrder == other.TurnOrder;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ TurnOrder;
            }
        }

        public static bool operator ==(Player left, Player right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Player left, Player right)
        {
            return !Equals(left, right);
        }
    }
}