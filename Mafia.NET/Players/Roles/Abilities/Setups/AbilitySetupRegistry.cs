using System.Collections.Concurrent;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Abilities.Setups;

public class AbilitySetupRegistry : WritableRegistry<AbilitySetupEntry>
{
    public AbilitySetupRegistry()
    {
        Roles = new ConcurrentDictionary<string, RoleSetupEntry>();
    }

    protected ConcurrentDictionary<string, RoleSetupEntry> Roles { get; }

    public RoleSetupEntry this[IRole role]
    {
        get => Role(role.Id);
        set => Roles[role.Id] = value;
    }

    public RoleSetupEntry Role(string role)
    {
        return Roles.GetOrAdd(role, new RoleSetupEntry(role));
    }

    public void Replace(params IAbilitySetup[] setups)
    {
        foreach (var setup in setups)
        foreach (var ability in AbilityRegistry.Default.Entries(setup))
        {
            var id = ability.Id;

            var entry = this[id];
            entry.Setup = setup;
            Ids[entry.Id] = entry;
        }
    }

    public override AbilitySetupEntry Create(string id)
    {
        var ability = AbilityRegistry.Default[id];
        return new AbilitySetupEntry(ability);
    }
}