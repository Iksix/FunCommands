using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace FunCommands;

public static class FunFunctions
{
    private static readonly IIksAdminApi Api = AdminUtils.CoreApi;
    private static readonly IStringLocalizer Localizer = Main.Instance.Localizer;
    
    public static void RConVar(
        CCSPlayerController caller, 
        CCSPlayerController target, 
        string convar, 
        string value, 
        IdentityType identityType = IdentityType.SteamId
    )
    {
        if (!ValidateTarget(caller, target, identityType))
            return;

        target.ReplicateConVar(convar, value);
        
        caller.Print(Localizer["Message.RConVar"].AReplace(["target"], [target.PlayerName]));
    }

    public static void Noclip(
        CCSPlayerController caller, 
        CCSPlayerController target, 
        IdentityType identityType = IdentityType.SteamId,
        bool? state = null
    )
    {
        if (!ValidateAliveTarget(caller, target, identityType))
            return;
        
        if (state == null)
            target.MoveType = target.MoveType == MoveType_t.MOVETYPE_NOCLIP ? MoveType_t.MOVETYPE_WALK : MoveType_t.MOVETYPE_NOCLIP;
        else target.MoveType = (bool)state ? MoveType_t.MOVETYPE_NOCLIP : MoveType_t.MOVETYPE_WALK;
        
        caller.Print(Localizer["Message.Noclip"].AReplace(
            ["target", "value"], 
            [target.PlayerName, target.MoveType == MoveType_t.MOVETYPE_NOCLIP]
            ));
    }
    
    public static void SetMoney(
        CCSPlayerController caller, 
        CCSPlayerController target, 
        int moneyAmount,
        IdentityType identityType = IdentityType.SteamId
    )
    {
        if (!ValidateTarget(caller, target, identityType))
            return;

        if (target.InGameMoneyServices == null)
        {
            caller.Print($"For some reason, set money for player {target.PlayerName} is impossible");
            return;
        }

        target.InGameMoneyServices.Account = moneyAmount;
        
        Utilities.SetStateChanged(target, "CCSPlayerController", "m_pInGameMoneyServices");
        
        caller.Print(Localizer["Message.SetMoney"].AReplace(
            ["target", "value"], 
            [target.PlayerName, moneyAmount]
        ));
    }
    
    public static void AddMoney(
        CCSPlayerController caller, 
        CCSPlayerController target, 
        int moneyAmount,
        IdentityType identityType = IdentityType.SteamId
    )
    {
        if (!ValidateTarget(caller, target, identityType))
            return;

        if (target.InGameMoneyServices == null)
        {
            caller.Print($"For some reason, set money for player {target.PlayerName} is impossible");
            return;
        }

        target.InGameMoneyServices.Account += Math.Abs(moneyAmount);
        
        Utilities.SetStateChanged(target, "CCSPlayerController", "m_pInGameMoneyServices");
        
        caller.Print(Localizer["Message.AddMoney"].AReplace(
            ["target", "value"], 
            [target.PlayerName, moneyAmount]
        ));
    }
    
    public static void TakeMoney(
        CCSPlayerController caller, 
        CCSPlayerController target, 
        int moneyAmount,
        IdentityType identityType = IdentityType.SteamId
    )
    {
        if (!ValidateTarget(caller, target, identityType))
            return;

        if (target.InGameMoneyServices == null)
        {
            caller.Print($"For some reason, set money for player {target.PlayerName} is impossible");
            return;
        }

        target.InGameMoneyServices.Account -= Math.Abs(moneyAmount);
        
        Utilities.SetStateChanged(target, "CCSPlayerController", "m_pInGameMoneyServices");
        
        caller.Print(Localizer["Message.TakeMoney"].AReplace(
            ["target", "value"], 
            [target.PlayerName, moneyAmount]
        ));
    }
    
    public static void Slap(
        CCSPlayerController caller, 
        CCSPlayerController target,
        IdentityType identityType = IdentityType.SteamId,
        int force = 1, int damage = 0
    )
    {
        if (!ValidateAliveTarget(caller, target, identityType))
            return;
        
        var pawn = target.PlayerPawn.Value;
        if (pawn!.LifeState != (int)LifeState_t.LIFE_ALIVE)
            return;

        /* Teleport in a random direction - thank you, Mani!*/
        /* Thank you AM & al!*/
        var random = new Random();
        var vel = new Vector(pawn.AbsVelocity.X, pawn.AbsVelocity.Y, pawn.AbsVelocity.Z);

        vel.X += ((random.Next(180) + 50) * ((random.Next(2) == 1) ? -1 : 1));
        vel.Y += ((random.Next(180) + 50) * ((random.Next(2) == 1) ? -1 : 1));
        vel.Z += random.Next(200) + 100;

        pawn.AbsVelocity.X = vel.X * force;
        pawn.AbsVelocity.Y = vel.Y * force;
        pawn.AbsVelocity.Z = vel.Z * force;

        if (damage <= 0)
            return;

        pawn.Health -= damage;
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");

        if (pawn.Health <= 0)
            pawn.CommitSuicide(true, true);

        caller.Print(Localizer["Message.Slap"].AReplace(
            ["target"], 
            [target.PlayerName]
        ));
    }
    
    private static bool ValidateTarget(
        CCSPlayerController caller, 
        CCSPlayerController? target, 
        IdentityType identityType
    )
    {
        if (target == null || !target.IsValid)
        {
            if (identityType is IdentityType.Name or IdentityType.UserId or IdentityType.SteamId)
                caller.Print(Localizer["Error.TargetNotFound"]);
            
            return false;
        }

        return true;
    }
    
    private static bool ValidateAliveTarget(
        CCSPlayerController caller, 
        CCSPlayerController? target, 
        IdentityType identityType
    )
    {
        if (!ValidateTarget(caller, target, identityType))
            return false;

        if (!target!.PawnIsAlive)
        {
            if (identityType is IdentityType.Name or IdentityType.UserId or IdentityType.SteamId)
                caller.Print(Localizer["Error.TargetMustBeAlive"]);
            
            return false;
        }

        return true;
    }
}