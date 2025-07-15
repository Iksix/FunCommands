using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using IksAdminApi;

namespace IksAdmin_FunCommands;

public class Main : AdminModule
{
    public override string ModuleName => "IksAdmin_FunCommands";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "iks__";

    public static PluginConfig Config = null!;

    public override void Ready()
    {
        Api.MenuOpenPre += OnMenuOpen;
        
        Config = new PluginConfig().ReadOrCreate(AdminUtils.ConfigsDir + "/IksAdmin_Modules/FunCommands.json", new PluginConfig());
        
        //Api.RegisterPermission("fun_commands.rconvar", "z");

        var defaultFlag = Config.DefaultFlag;
        
        Api.RegisterPermission("fun_commands.noclip", defaultFlag);
        Api.RegisterPermission("fun_commands.set_money", defaultFlag);
        Api.RegisterPermission("fun_commands.slap", defaultFlag);
        Api.RegisterPermission("fun_commands.hp", defaultFlag);
        Api.RegisterPermission("fun_commands.speed", defaultFlag);
        Api.RegisterPermission("fun_commands.scale", defaultFlag);
        
        RegisterEventHandler<EventPlayerHurt>(FunFunctions.OnPlayerHurt);
        RegisterEventHandler<EventRoundEnd>(FunFunctions.OnRoundEnd);
    }

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
        
        DeregisterEventHandler<EventPlayerHurt>(FunFunctions.OnPlayerHurt);
        DeregisterEventHandler<EventRoundEnd>(FunFunctions.OnRoundEnd);
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
            "hp",
            "Set hp for player",
            "fun_commands.hp",
            "css_hp <#uid/#steamId/name/@...> <hp amount>",
            Cmd.SetHp,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "speed",
            "Set speed for player",
            "fun_commands.speed",
            "css_speed <#uid/#steamId/name/@...> <speed>",
            Cmd.SetSpeed,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "scale",
            "Set scale for player",
            "fun_commands.scale",
            "css_scale <#uid/#steamId/name/@...> <scale>",
            Cmd.SetScale,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "set_money",
            "Set money for player",
            "fun_commands.set_money",
            "css_set_money <#uid/#steamId/name/@...> <money amount>",
            Cmd.SetMoney,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "add_money",
            "Set money for player",
            "fun_commands.set_money",
            "css_add_money <#uid/#steamId/name/@...> <money amount>",
            Cmd.AddMoney,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "take_money",
            "Set money for player",
            "fun_commands.set_money",
            "css_take_money <#uid/#steamId/name/@...> <money amount>",
            Cmd.TakeMoney,
            minArgs: 2
        );
    }
}
