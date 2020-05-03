namespace Mafia.NET.Players.Votes
{
    public class AccuseChangeEventArgs
    {
        public IPlayer Voter { get; }
        public IPlayer OldAccuse { get; }
        public IPlayer NewAccuse { get; }

        public AccuseChangeEventArgs(IPlayer voter, IPlayer oldAccuse, IPlayer newAccuse)
        {
            Voter = voter;
            OldAccuse = oldAccuse;
            NewAccuse = newAccuse;
        }
    }
}
