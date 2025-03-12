using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAdventage
{
    private static string[] base_defense = { "base_attack" };
    private static string[] strong_attack = { "base_attack", "base_defense" };
    private static string[] base_dodge = { "strong_attack", "smash" };
    private static string[] smash = { "base_attack", "base_defense", "strong_attack" };


    public Dictionary<string, string[]> Adventages = new Dictionary<string, string[]>()
    {
        {"base_defense", base_defense },
        {"strong_attack", strong_attack },
        {"base_dodge", base_dodge },
        {"smash", smash }
    };

   
}
