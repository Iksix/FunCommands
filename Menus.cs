﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using IksAdmin_FunCommands.Extensions;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;

public static class Menus
{
    private static readonly IIksAdminApi Api = AdminUtils.CoreApi;
    private static readonly IStringLocalizer Localizer = Main.StringLocalizer;
    
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
        
        menu.AddMenuOption("save_position", Localizer["MenuOption.SavePosition"],
            (_, _) => { SavePosition(caller, backMenu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.tp"));
        
        menu.AddMenuOption("teleport", Localizer["MenuOption.Teleport"],
            (_, _) => { Teleport(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.tp"),
            disabled: !FunFunctions.PlayersSavedTeleportPositions.ContainsKey(caller.GetSteamId()));
        
        menu.AddMenuOption("pingtp", Localizer["MenuOption.PingTp"],
            (_, _) => { TurnPingTp(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.pingtp"));
        
        menu.AddMenuOption("shootspeed", Localizer["MenuOption.SetShootSpeed"],
            (_, _) => { SetShootSpeed(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.shootspeed"));
        
        menu.AddMenuOption("custom_damage", Localizer["MenuOption.SetCustomDamage"],
            (_, _) => { SetCustomDamage(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.set_damage"));
        
        menu.AddMenuOption("add_damage", Localizer["MenuOption.SetBonusDamage"],
            (_, _) => { SetBonusDamage(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.add_damage"));
        
        menu.AddMenuOption("max_ammo", Localizer["MenuOption.SetMaxAmmo"],
            (_, _) => { SetMaxAmmo(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.max_ammo"));
        
        menu.AddMenuOption("no_recoil", Localizer["MenuOption.SetNoRecoil"],
            (_, _) => { SetNoRecoil(caller, menu); },
            viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.no_recoil"));
        
        menu.Open(caller);
    }

    private static void SetMoney(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectPlayer(caller, "set_money", (target, _) =>
        {
            caller.Print(Localizer["Request.MoneyAmount"]);
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetMoney(caller, target.Controller!, int.Parse(amount));
            });
        }, backMenu: backMenu);
    }
    
    private static void SavePosition(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        caller.Print(Localizer["Request.PositionKey"]);
            
        Api.HookNextPlayerMessage(caller, key =>
        {
            FunFunctions.SaveTeleportPosition(caller, key);
            OpenFunMenu(caller, backMenu);
        });
    }
    
    private static void TurnPingTp(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        var menu = Api.CreateMenu("select_tp_position", Localizer["MenuTitle.SelectPosition"], backMenu: backMenu);

        foreach (var target in PlayersUtils.GetOnlinePlayers())
        {
            if (!Api.CanDoActionWithPlayer(caller.GetSteamId(), target.GetSteamId()))
            {
                continue;
            }

            var state = FunFunctions.PlayersWithTeleportOnPing.Contains(target.Slot);
            
            menu.AddMenuOption(target.GetSteamId(), target.PlayerName + (state ? " [+]" : " [-]"), (_, _) =>
            {
                FunFunctions.TurnTeleportOnPing(caller, target, !state);
                TurnPingTp(caller, backMenu);
            });
        }

        menu.Open(caller);
    }
    
    private static void SetNoRecoil(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        var menu = Api.CreateMenu("select_player", Localizer["MenuOption.SetNoRecoil"], backMenu: backMenu);

        foreach (var target in PlayersUtils.GetOnlinePlayers())
        {
            if (!Api.CanDoActionWithPlayer(caller.GetSteamId(), target.GetSteamId()))
            {
                continue;
            }

            var settings = target.GetWeaponSettings();
            
            menu.AddMenuOption(target.GetSteamId(), target.PlayerName + (settings.NoRecoil ? " [+]" : " [-]"), (_, _) =>
            {
                FunFunctions.SetNoRecoil(caller, target, !settings.NoRecoil);
                SetNoRecoil(caller, backMenu);
            });
        }

        menu.Open(caller);
    }
    
    private static void Teleport(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "teleport", backMenu, target =>
        {
            OpenSelectPositionMenu(caller, target, backMenu);
        });
    }

    private static void OpenSelectPositionMenu(CCSPlayerController caller, CCSPlayerController target, IDynamicMenu? backMenu)
    {
        var menu = Api.CreateMenu("select_tp_position", Localizer["MenuTitle.SelectPosition"]);

        menu.BackAction = _ => { Teleport(caller, backMenu); };

        var positions = FunFunctions.PlayersSavedTeleportPositions[caller.GetSteamId()];

        foreach (var position in positions)
        {
            menu.AddMenuOption(position.Key, position.Key, (_, _) =>
            {
                if (!target.PawnIsAlive)
                {
                    caller.Print(Localizer[Localizer["Error.TargetMustBeAlive"]]);
                    return;
                }
                
                FunFunctions.TeleportToSavedPos(caller, target, position.Key);
            });
        }

        menu.Open(caller);
    }
    
    private static void SetShootSpeed(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "set_shootspeed", backMenu, target =>
        {
            var weapon = caller.GetActiveWeaponName();
            
            if (weapon == null)
            {
                caller.Print(Localizer["Error.MustHandleWeapon"]);
                return;
            }
            
            caller.Print(Localizer["Request.ShootSpeed"].AReplace(["weapon"], [weapon]));
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetShootSpeed(caller, target, amount == "default" ? null : float.Parse(amount));
            });
        }, includeBots: false);
    }
    
    private static void SetCustomDamage(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "set_custom_damage", backMenu, target =>
        {
            var weapon = caller.GetActiveWeaponName();
            
            if (weapon == null)
            {
                caller.Print(Localizer["Error.MustHandleWeapon"]);
                return;
            }
            
            caller.Print(Localizer["Request.CustomDamage"].AReplace(["weapon"], [weapon]));
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetCustomDamage(caller, target, amount == "default" ? null : int.Parse(amount));
            });
        }, includeBots: false);
    }
    
    private static void SetBonusDamage(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "set_bonus_damage", backMenu, target =>
        {
            var weapon = caller.GetActiveWeaponName();
            
            if (weapon == null)
            {
                caller.Print(Localizer["Error.MustHandleWeapon"]);
                return;
            }
            
            caller.Print(Localizer["Request.BonusDamage"].AReplace(["weapon"], [weapon]));
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetBonusDamage(caller, target, amount == "default" ? null : int.Parse(amount));
            });
        }, includeBots: false);
    }
    
    private static void SetMaxAmmo(CCSPlayerController caller, IDynamicMenu? backMenu)
    {
        OpenSelectAlivePlayer(caller, "max_ammo", backMenu, target =>
        {
            var weapon = caller.GetActiveWeaponName();
            
            if (weapon == null)
            {
                caller.Print(Localizer["Error.MustHandleWeapon"]);
                return;
            }
            
            caller.Print(Localizer["Request.MaxAmmo"].AReplace(["weapon"], [weapon]));
            
            Api.HookNextPlayerMessage(caller, amount =>
            {
                FunFunctions.SetMaxAmmo(caller, target, amount == "default" ? null : int.Parse(amount));
            });
        });
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
        OpenSelectPlayer(caller, "add_money", (target, _) =>
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
        OpenSelectPlayer(caller, "take_money", (target, _) =>
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
        OpenSelectPlayer(caller, "rconvar", (target, _) =>
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
            if (!player.IsBot && !Api.CanDoActionWithPlayer(caller.GetSteamId(), player.GetSteamId()))
            {
                continue;
            }
            
            if (!player.PawnIsAlive) continue;
            
            menu.AddMenuOption(player.Slot.ToString(), player.PlayerName, (_, _) =>
            { 
                action.Invoke(player); 
            });
        }
        
        menu.Open(caller);
    }
    
    public static void OpenSelectPlayer(CCSPlayerController caller, string idPrefix, Action<PlayerInfo, IDynamicMenu> action, bool includeBots = false, IDynamicMenu? backMenu = null, string? customTitle = null)
    {
        var menu = Api.CreateMenu(
            idPrefix + "_select_player",
            customTitle ?? Api.Localizer["MenuTitle.Other.SelectPlayer"],
            titleColor: MenuColors.Gold,
            backMenu: backMenu
        );

        var players = PlayersUtils.GetOnlinePlayers(includeBots);

        foreach (var player in players)
        {
            if (!player.IsBot && !Api.CanDoActionWithPlayer(caller.GetSteamId(), player.GetSteamId()))
            {
                continue;
            }
            
            var p = new PlayerInfo(player);
            
            menu.AddMenuOption(p.SteamId!, p.PlayerName, (_, _) =>
            {
                action.Invoke(p, menu);
            });
        }

        menu.Open(caller);
    }
}