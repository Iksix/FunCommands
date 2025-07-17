using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;

public class Main : AdminModule
{
    public override string ModuleName => "IksAdmin_FunCommands";
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "iks__";

    public static PluginConfig Config = null!;

    public static IStringLocalizer StringLocalizer = null!;
    
    /*
    Для теста в будущем
     
    sub_1342910(&unk_1A2F120, &v10); ===== Player Ping
    qword_1A2F108 = (__int64)"CHandle< CBaseEntity>";
    qword_1A2F100 = (__int64)"m_hPlayerPing";
    
     __int64 __fastcall sub_1342910(__m128i *a1, __int64 a2)
     
    \x48\xC7\x47\x08\x00\x00\x00\x00\x48\xC7\x47\x10\x00\x00\x00\x00\x48\xC7\x47\x18\x00\x00\x00\x00\x48\x8B\x06
    */

    public override void Ready()
    {
        Api.MenuOpenPre += OnMenuOpen;
        
        StringLocalizer = Localizer;
        
        Config = new PluginConfig().ReadOrCreate(AdminUtils.ConfigsDir + "/IksAdmin_Modules/FunCommands.json", new PluginConfig());
        
        //Api.RegisterPermission("fun_commands.rconvar", "z");

        var defaultFlag = Config.DefaultFlag;
        
        Api.RegisterPermission("fun_commands.noclip", defaultFlag);
        Api.RegisterPermission("fun_commands.set_money", defaultFlag);
        Api.RegisterPermission("fun_commands.slap", defaultFlag);
        Api.RegisterPermission("fun_commands.hp", defaultFlag);
        Api.RegisterPermission("fun_commands.speed", defaultFlag);
        Api.RegisterPermission("fun_commands.scale", defaultFlag);
        Api.RegisterPermission("fun_commands.tp", defaultFlag);
        Api.RegisterPermission("fun_commands.pingtp", defaultFlag);
        
        RegisterEventHandler<EventPlayerHurt>(FunFunctions.OnPlayerHurt);
        RegisterEventHandler<EventRoundEnd>(FunFunctions.OnRoundEnd);
        RegisterEventHandler<EventPlayerPing>(FunFunctions.OnPlayerPing, HookMode.Pre);
        
        RegisterListener<Listeners.OnClientDisconnect>(FunFunctions.OnClientDisconnect);
    }
    
    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);
        Api.MenuOpenPre -= OnMenuOpen;
        DeregisterEventHandler<EventPlayerHurt>(FunFunctions.OnPlayerHurt);
        DeregisterEventHandler<EventRoundEnd>(FunFunctions.OnRoundEnd);
        DeregisterEventHandler<EventPlayerPing>(FunFunctions.OnPlayerPing, HookMode.Pre);
    }

    private HookResult OnMenuOpen(CCSPlayerController player, IDynamicMenu menu, IMenu gameMenu)
    {
        if (menu.Id != "iksadmin:menu:main") return HookResult.Continue;
        
        menu.AddMenuOption("fun_commands", Localizer["MenuTitle.FunCommands"], (p, _) =>
        {
            Menus.OpenFunMenu(p, menu);
        }, viewFlags: AdminUtils.GetAllPermissionGroupFlags("fun_commands"));
        
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
        
        // [1.0.1]
        
        Api.AddNewCommand(
            "pingtp",
            "Set money for player",
            "fun_commands.pingtp",
            "css_pingtp <#uid/#steamId/name/@...> <true/false>",
            Cmd.TurnTeleportOnPing,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "tp",
            "Set money for player",
            "fun_commands.tp",
            "css_tp <#uid/#steamId/name/@...> <position key>",
            Cmd.Teleport,
            minArgs: 2
        );
        
        Api.AddNewCommand(
            "savepos",
            "Set money for player",
            "fun_commands.tp",
            "css_savepos <position key>",
            Cmd.SavePos,
            minArgs: 1
        );
    }
}
