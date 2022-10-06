using BepInEx;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using Utilla;
using GorillaLocomotion;

/* Resources I used for this project */
// https://assetstore.unity.com/packages/vfx/particles/war-fx-5669
// https://www.youtube.com/watch?v=9FMquJzgDGQ&ab_channel=SoundEffects
// https://sketchfab.com/3d-models/stylized-tank-5fed1107837945d2baa535291f6ee4cb
// https://discord.com/channels/810644499763691540/810663983106621501/1010265491543183503

// tinotin is cool

namespace GorillaTank
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]

    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;

        public GameObject Tank;

        public GameObject ExplodeEffect;
        public GameObject CannonBall;

        public Transform TankTop;
        public Transform TankTopAim;
        public Transform TankSpawn;

        public AssetBundle TankBundle;

        public bool inRoom;
        public bool isHeldDown = false;

        public float Rotation = 0;
        private float FixedRotation = 0;

        public float RotationY;
        private float FixedRotationY;

        void Awake()
        {
            Instance = this;
            Events.GameInitialized += OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            try
            {
                TankBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GorillaTank.Resources.gorillatank"));
                Tank = Instantiate(TankBundle.LoadAsset<GameObject>("GorillaTank"));
                ExplodeEffect = TankBundle.LoadAsset<GameObject>("WFXMR_ExplosiveSmoke Big Alt");
                CannonBall = TankBundle.LoadAsset<GameObject>("cannonball");
            }

            catch (Exception error)
            {
                Debug.LogError(error.Message);
                return;
            }

            Tank.transform.position = new Vector3(-76.34f, 2.57f, -81.89f);
            Tank.transform.rotation = Quaternion.Euler(0, -90, 0);
            Tank.transform.localScale = Vector3.one * 1.4599f;

            TankSpawn = Tank.transform.Find("Top/Turret/SpawnPoint");
            TankTop = Tank.transform.Find("Top");
            TankTopAim = Tank.transform.Find("Top/Turret");

            MeshCollider[] tankTransforms = Tank.GetComponentsInChildren<MeshCollider>(false);
            foreach(MeshCollider tankTransform in tankTransforms)
                tankTransform.gameObject.AddComponent<GorillaSurfaceOverride>().overrideIndex = 18;

            Tank.SetActive(false);
        }

        public void AttemptShoot()
        {
            isHeldDown = true;

            GameObject projectile = Instantiate(CannonBall);

            projectile.transform.position = TankSpawn.position;
            projectile.transform.rotation = TankSpawn.rotation;

            projectile.gameObject.layer = 23;
            projectile.AddComponent<Bullet>();
        }

        void Update()
        {
            /* Code here runs every frame when the mod is enabled */

            if (Tank == null)
                return;

            if (!inRoom)
            {
                FixedRotation = Rotation;
                FixedRotationY = RotationY;

                Tank.SetActive(false);
                return;
            }

            Tank.SetActive(true);

            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis);
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool down);

            float bodyDistance = Vector3.Distance(Tank.transform.position, Player.Instance.bodyCollider.transform.position);
            if (bodyDistance <= 5f)
            {
                if (down && !isHeldDown)
                    AttemptShoot();
                else if (!down && isHeldDown)
                    isHeldDown = false;

                if (axis.x > 0.2f || axis.x < -0.2f)
                    RotationY += axis.x * 0.5f;

                if (axis.y > 0.2f || axis.y < -0.2f)
                {
                    Rotation += axis.y * -1 * 0.5f;
                }

                Rotation = Mathf.Clamp(Rotation, -45, 10);
                TankTopAim.localRotation = Quaternion.Euler(FixedRotation, TankTopAim.localRotation.y, TankTopAim.localRotation.z);
            }

            FixedRotation = Mathf.Lerp(FixedRotation, Rotation, 5f * Time.deltaTime);
            FixedRotationY = Mathf.Lerp(FixedRotationY, RotationY, 5f * Time.deltaTime);

            TankTop.localEulerAngles = new Vector3(0, FixedRotationY, 0);
        }


        [ModdedGamemodeJoin] public void OnJoin(string gamemode) => inRoom = true;
        [ModdedGamemodeLeave] public void OnLeave(string gamemode) => inRoom = false;

        class Bullet : MonoBehaviour
        {
            Vector3 lastVelocity;
            bool storeVelocity = false;
            bool hit = false;

            IEnumerator Start()
            {
                Rigidbody body = gameObject.AddComponent<Rigidbody>();
                body.AddRelativeForce(Vector3.forward * 25, ForceMode.VelocityChange);

                yield return new WaitForSeconds(0.1f);

                lastVelocity = body.velocity;
                storeVelocity = true;

                yield return new WaitForSeconds(0.15f);

                storeVelocity = false;
                yield break;
            }

            IEnumerator Explode()
            {
                Destroy(gameObject.GetComponent<Rigidbody>());
                Destroy(gameObject.GetComponent<Collider>());

                GameObject effect = Instantiate(Instance.ExplodeEffect);
                effect.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.25f, 0.5f) ;
                effect.transform.position = transform.position;

                Destroy(effect, 5);
                Destroy(gameObject, 0);

                yield break;
            }

            void FixedUpdate()
            {
                if (storeVelocity)
                    gameObject.GetComponent<Rigidbody>().velocity = lastVelocity;
            }

            void OnCollisionEnter(Collision collision)
            {
                if (hit)
                    return;

                if (collision.gameObject.TryGetComponent(out HitTargetWithScoreCounter hitTargetWithScoreCounter))
                    hitTargetWithScoreCounter.TargetHit();

                hit = true;
                StartCoroutine(Explode());
            }
        }
    }
}
