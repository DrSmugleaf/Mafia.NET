namespace Mafia.NET.Registries
{
    public interface IRegistrable<out T>
    {
        T Id { get; }
    }

    public interface IRegistrable : IRegistrable<string>
    {
    }
}