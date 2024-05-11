using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace MapKey;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private static ConfigEntry<bool> _modEnabled = null!;
    private static ConfigEntry<KeyCode> _keyBind = null!;

    private static bool _showWindow;
    private static bool _wasWindowShown;
    private void Awake()
    {
        _modEnabled = Config.Bind("1 - General", "Mod Enabled", true, "");
        _keyBind = Config.Bind("2 - Keybinds", "HotKey", KeyCode.M, "Hotkey to Open Map");
    }

    public void Update()
    {
        if (!_modEnabled.Value) return;

        if (Input.GetKeyDown(_keyBind.Value)) _showWindow = !_showWindow;

        if (!(Input.GetKeyDown(KeyCode.Escape) && _showWindow)) return;

        _showWindow = false;
        _wasWindowShown = true;
        
    }

    public void OnGUI()
    {
        if (_showWindow)
        {
            if (GUIManager.instance.IsLoading() || (LoadingScreen.playerRespawningFromCliff && LoadingScreen.timeSinceLoaded < 2f) || GUIManager.instance.pauseBlocked) return;
            OpenMap();
        }
        else
        {
            if (!_wasWindowShown) return;
            CloseMap();
        }
    }

    private static void OpenMap()
    {
        var pauseMenu = GUIManager.instance.pauseMenu;
        pauseMenu.currentTabIndex = 4;
        pauseMenu.SetTab(pauseMenu.currentTabIndex);
        pauseMenu.showMouseWhileOpen = false;
        pauseMenu.preventManualClosing = true;
        pauseMenu.SetEnabled(true);
        pauseMenu.windowActive = true;
        GUIManager.instance.PutWindowOnTop(pauseMenu);
        pauseMenu.PlayOpenSound();
        GameManager.events.TriggerWindowOpened(pauseMenu);
        pauseMenu.OnWindowOpened?.Invoke();
        pauseMenu.closing = false;
        _wasWindowShown = true;
    }

    private static void CloseMap()
    {
        var pauseMenu = GUIManager.instance.pauseMenu;
        pauseMenu.Close();
        pauseMenu.previousSelectedObject = null;
        _wasWindowShown = false;
    }
}

