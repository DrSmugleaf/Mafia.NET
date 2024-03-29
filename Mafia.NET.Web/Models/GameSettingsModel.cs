﻿using System;
using System.Collections.Generic;
using Mafia.NET.Players.Roles.Selectors;

namespace Mafia.NET.Web.Models;

public class GameSettingsModel
{
    public int DayLength { get; set; }
    public string DayType { get; set; } = "Majority";
    public int NightLength { get; set; }
    public bool LastWillAllowed { get; set; }
    public int DiscussionTime { get; set; }
    public string StartGameAt { get; set; } = "Day";
    public string NightType { get; set; } = "Night Sequence";
    public bool PmAllowed { get; set; }
    public bool Discussion { get; set; }
    public bool TrialPausesDay { get; set; }
    public bool TrialDefense { get; set; }
    public bool ChooseNames { get; set; }
    public int TrialLength { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();

    public List<IRoleSelector> RoleEntries()
    {
        var selectors = new List<IRoleSelector>();

        foreach (var group in SelectorGroup.Default())
        {
            var subSelectors = group.Get(Roles);
            selectors.AddRange(subSelectors);
        }

        return selectors;
    }
}