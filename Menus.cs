using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace FunCommands;

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
        
        menu.AddMenuOption("set_money", Localizer["MenuOption.SetMoney"], (_, _) => { SetMoney(caller, menu); }, viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.set_money"));
        
        menu.AddMenuOption("add_money", Localizer["MenuOption.AddMoney"], (_, _) => { AddMoney(caller, menu); }, viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.set_money"));
        
        menu.AddMenuOption("take_money", Localizer["MenuOption.TakeMoney"], (_, _) => { TakeMoney(caller, menu); }, viewFlags: AdminUtils.GetCurrentPermissionFlags("fun_commands.set_money"));
        
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
        MenuUtils.OpenSelectPlayer(caller, "noclip", (target, _) =>
        {
            FunFunctions.Noclip(caller, target.Controller!);
        }, backMenu: backMenu);
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
}