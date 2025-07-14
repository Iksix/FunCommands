using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using IksAdminApi;

namespace FunCommands;

public static class Cmd
{
    private static readonly IIksAdminApi Api = AdminUtils.CoreApi;

    private static string[] GetBlockedIdentifiers(string key)
    {
        if (Api.Config.BlockedIdentifiers.TryGetValue(key, out var arr))
            return arr;
        return [];
    }
    
    public static void RConVar(CCSPlayerController caller, List<string> args, CommandInfo _)
    {
        Api.DoActionWithIdentity(caller, args[0], (target, identityType) =>
        {
            FunFunctions.RConVar(caller, target!, args[1], args[2], identityType);
        }, blockedArgs: GetBlockedIdentifiers("rconvar"));
    }
    
    public static void Noclip(CCSPlayerController caller, List<string> args, CommandInfo _)
    {
        bool? state = args.Count > 1 ? bool.Parse(args[1]) : null;
        
        Api.DoActionWithIdentity(caller, args[0], (target, identityType) =>
        {
            FunFunctions.Noclip(caller, target!, identityType, state);
        }, blockedArgs: GetBlockedIdentifiers("noclip"));
    }
    
    public static void Slap(CCSPlayerController caller, List<string> args, CommandInfo _)
    {
        int force = args.Count > 1 ? int.Parse(args[1]) : 1;
        int damage = args.Count > 2 ? int.Parse(args[2]) : 0;
        
        Api.DoActionWithIdentity(caller, args[0], (target, identityType) =>
        {
            FunFunctions.Slap(caller, target!, identityType, force, damage);
        }, blockedArgs: GetBlockedIdentifiers("slap"));
    }
    
    public static void SetMoney(CCSPlayerController caller, List<string> args, CommandInfo _)
    {
        Api.DoActionWithIdentity(caller, args[0], (target, identityType) =>
        {
            FunFunctions.SetMoney(caller, target!, int.Parse(args[1]), identityType);
        }, blockedArgs: GetBlockedIdentifiers("set_money"));
    }
    
    public static void AddMoney(CCSPlayerController caller, List<string> args, CommandInfo _)
    {
        Api.DoActionWithIdentity(caller, args[0], (target, identityType) =>
        {
            FunFunctions.AddMoney(caller, target!, int.Parse(args[1]), identityType);
        }, blockedArgs: GetBlockedIdentifiers("add_money"));
    }
    
    public static void TakeMoney(CCSPlayerController caller, List<string> args, CommandInfo _)
    {
        Api.DoActionWithIdentity(caller, args[0], (target, identityType) =>
        {
            FunFunctions.TakeMoney(caller, target!, int.Parse(args[1]), identityType);
        }, blockedArgs: GetBlockedIdentifiers("take_money"));
    }
}