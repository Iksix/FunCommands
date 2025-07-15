using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;

public static class Menus
{
    private static readonly IIksAdminApi Api = AdminUtils.CoreApi;
    private static readonly IStringLocalizer Localizer = Main.Instance.Localizer;
    
    public static void OpenFunMenu(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        var menu = Api.CreateMenu(
            "fun_commands",
            Localizer["MenuTitle.FunCommands"],
            backMenu: backMenu
            );
        
        // menu.AddMenuOption("rconvar", Localizer["MenuOption.RConVar"],
        //     (_, _) => { RConVar(caller, menu); },
        //      viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.rconvar"));
        
        menu.AddMenuOption("noclip", Localizer["MenuOption.Noclip"], 
            (_, _) => { Noclip(caller, menu); }, 
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.noclip"));
        
        menu.AddMenuOption("set_money", Localizer["MenuOption.SetMoney"],
            (_, _) => { SetMoney(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.set_money"));
        
        menu.AddMenuOption("add_money", Localizer["MenuOption.AddMoney"],
            (_, _) => { AddMoney(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.set_money"));
        
        menu.AddMenuOption("take_money", Localizer["MenuOption.TakeMoney"],
            (_, _) => { TakeMoney(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.set_money"));
        
        menu.AddMenuOption("set_hp", Localizer["MenuOption.SetHp"],
            (_, _) => { SetHp(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.hp"));
        
        menu.AddMenuOption("set_speed", Localizer["MenuOption.SetSpeed"],
            (_, _) => { SetSpeed(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.speed"));
        
        menu.AddMenuOption("set_scale", Localizer["MenuOption.SetScale"],
            (_, _) => { SetScale(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.scale"));
        
        menu.Open(caller);
    }

    private static void SetMoney(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        MenuUtils.OpenSelectPlayer(caller, "set_money", (target, _) =>
        {
            caller.Print(Localizer["Request.MoneyAmount"]);
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetMoney(caller, target.Controller!, int.Parse(amount));
            });
        }, backMenu: backMenu);
    }
    
    private static void SetHp(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "set_hp", backMenu, target =>
        {
            caller.Print(Localizer["Request.HpAmount"]);
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetHp(caller, target, int.Parse(amount));
            });
        });
    }
    
    private static void SetSpeed(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "set_speed", backMenu, target =>
        {
            caller.Print(Localizer["Request.Speed"]);
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetSpeed(caller, target, float.Parse(amount));
            });
        });
    }
    
    private static void SetScale(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "set_scale", backMenu, target =>
        {
            caller.Print(Localizer["Request.Scale"]);
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetScale(caller, target, float.Parse(amount));
            });
        });
    }
    
    private static void AddMoney(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        MenuUtils.OpenSelectPlayer(caller, "add_money", (target, _) =>
        {
            caller.Print(Localizer["Request.MoneyAmount"]);
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.AddMoney(caller, target.Controller!, int.Parse(amount));
            });
        }, backMenu: backMenu);
    }
    
    private static void TakeMoney(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        MenuUtils.OpenSelectPlayer(caller, "take_money", (target, _) =>
        {
            caller.Print(Localizer["Request.MoneyAmount"]);

            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.TakeMoney(caller, target.Controller!, int.Parse(amount));
            });
        }, backMenu: backMenu);
    }

    private static void Noclip(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "noclip", backMenu, target =>
        {
            FunFunctions.Noclip(caller, target);
        }, includeBots: false);
    }

    private static void RConVar(CCSPlayerController caller, IDynamicMenu backMenu)
    {
        MenuUtils.OpenSelectPlayer(caller, "rconvar", (target, _) =>
        {
            caller.Print(Localizer["Request.RConVar1"]);
            Api.HookNextPlayerMessage(caller, cvar =>
            {
                caller.Print(Localizer["Request.RConVar2"]);
                Main.Instance.AddTimer(0.2f, () =>
                {
                    Api.HookNextPlayerMessage(caller, value =>
                    {
                        FunFunctions.RConVar(caller, target.Controller!, cvar, value);
                    });
                });
            });
        }, backMenu: backMenu);
    }

    private static void OpenSelectAlivePlayer(CCSPlayerController caller, string prefix, IDynamicMenu? backMenu, Action<CCSPlayerController> action, bool includeBots = true)
    {
        var menu = Api.CreateMenu(prefix + "_select_alive_player", Api.Localizer["MenuTitle.Other.SelectPlayer"], backMenu: backMenu);

        foreach (var player in PlayersUtils.GetOnlinePlayers(includeBots))
        {
            if (!player.PawnIsAlive) continue;
            
            menu.AddMenuOption(player.Slot.ToString(), player.PlayerName, (_, _) =>
            { 
                action.Invoke(player); 
            });
        }
        
        menu.Open(caller);
    }
}