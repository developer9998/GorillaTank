using BepInEx;
using System;
using UnityEngine;
using VmodMonkeMapLoader;

namespace GorillaTankMapLoader
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    [BepInDependency("vadix.gorillatag.maploader", "1.2.1")]
    [BepInDependency("com.dev9998.gorillatag.gorillatank", "1.0.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        void Awake() => Events.OnMapEnter += MapUpdate;

        void MapUpdate(bool Entered)
        {
            if (!Entered) // when you leave the map
            {
                GorillaTank.Plugin.Instance.Tank.transform.parent = null;
                GorillaTank.Plugin.Instance.Tank.transform.position = new Vector3(-76.34f, 2.57f, -81.89f);
                GorillaTank.Plugin.Instance.Tank.transform.rotation = Quaternion.Euler(0, -90, 0);
                return;
            }

            if (GameObject.Find("TankArea") == null)
            {
                Debug.LogError("Failed to place tank (Object not found in map or doesn't exist.)");
                return;
            }

            try
            {
                GameObject tankArea = GameObject.Find("TankArea");

                GorillaTank.Plugin.Instance.Tank.transform.parent = tankArea.transform;
                GorillaTank.Plugin.Instance.Tank.transform.localPosition = Vector3.zero;
                GorillaTank.Plugin.Instance.Tank.transform.localEulerAngles = Vector3.zero;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to place tank ({e})");
                return;
            }
        }
    }
}
