using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using IksAdminApi;

namespace FunCommands;

public class Main : AdminModule
{
    public override string ModuleName => "FunCommands";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "iks__";

    public override void Ready()
    {
        Api.MenuOpenPre += OnMenuOpen;
    }

    private HookResult OnMenuOpen(CCSPlayerController player, IDynamicMenu menu, IMenu gameMenu)
    {
        if (menu.Id != "iksadmin:menu:main") return HookResult.Continue;
        
        menu.AddMenuOption("fun_commands", Localizer["MenuTitle.FunCommands"], (p, _) =>
        {
            Menus.OpenFunMenu(p, menu);
        });
        
        return HookResult.Continue;
    }

    public override void InitializeCommands()
    {
        //Api.RegisterPermission("fun_commands.rconvar", "z");
        Api.RegisterPermission("fun_commands.noclip", "z");
        Api.RegisterPermission("fun_commands.set_money", "z");
        Api.RegisterPermission("fun_commands.slap", "z");
        
        // Api.AddNewCommand(
        //     "rconvar",
        //     "Change a replicate convar for player",
        //     "fun_commands.rconvar",
        //     "css_rconvar <#uid/#steamId/name/@...> <convar> <value>",
        //     Cmd.RConVar,
        //     minArgs: 3
        // );
        
        Api.AddNewCommand(
            "slap",
            "Slap player",
            "fun_commands.slap",
            "css_slap <#uid/#steamId/name/@...> [force(number)] [damage(number)]",
            Cmd.Slap,
            minArgs: 1
        );
        
        Api.AddNewCommand(
            "noclip",
            "Turn noclip for player",
            "fun_commands.noclip",
            "css_noclip <#uid/#steamId/name/@...> [true/false]",
            Cmd.Noclip,
            minArgs: 1
        );
        
        Api.AddNewCommand(
            "set_money",
            "Set money for player",
            "fun_commands.set_money",
            "css_set_money <#uid/#steamId/name/@...> <moneyAmount>",
            Cmd.SetMoney,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "add_money",
            "Set money for player",
            "fun_commands.set_money",
            "css_add_money <#uid/#steamId/name/@...> <moneyAmount>",
            Cmd.AddMoney,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "take_money",
            "Set money for player",
            "fun_commands.set_money",
            "css_take_money <#uid/#steamId/name/@...> <moneyAmount>",
            Cmd.TakeMoney,
            minArgs: 2
        );
    }
}
