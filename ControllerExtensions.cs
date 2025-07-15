using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace IksAdmin_FunCommands;

public static class ControllerExtensions
{
    public static void SetSpeed(this CCSPlayerController target, float speed)
    {
        CCSPlayerPawn? playerPawnValue = target.PlayerPawn.Value;
        if (playerPawnValue == null) return;
        playerPawnValue.VelocityModifier = speed;
    }
    
    public static void SetGravity(this CCSPlayerController target, float gravity)
    {
        CCSPlayerPawn? playerPawnValue = target.PlayerPawn.Value;
        if (playerPawnValue == null) return;

        playerPawnValue.GravityScale = gravity;
    }
    
    public static void SetScale(this CCSPlayerController target, float value)
    {
        var playerPawnValue = target.PlayerPawn.Value;
        if (playerPawnValue == null)
            return;

        playerPawnValue.CBodyComponent!.SceneNode!.Scale = value;
        Utilities.SetStateChanged(playerPawnValue, "CBaseEntity", "m_CBodyComponent");
    }
    
}