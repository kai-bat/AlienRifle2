using UnityEngine;

namespace AlienRifle2
{
    public class RifleTool : PlayerTool
    {
        public override string animToolName => "stasisrifle";
        public ParticleSystem chargeEffect;
        public ParticleSystem shootEffect;
        public Transform chargeMeter;
        public GameObject bulletPrefab;
        public FMOD_CustomEmitter sonarSound;
        public FMODAsset shootSound;
        public FMOD_CustomLoopingEmitter chargeSound;

        float charge = 0f;
        bool canShoot = true;
        bool aiming = false;
        float sonarTimer = 0f;
        float aimTimer = 0f;

        public void Update()
        {
            chargeMeter.localScale = Vector3.Lerp(new Vector3(0f, 1f, 0.0002f), new Vector3(0.008f, 1, 0.0002f), charge);

            if(Player.main.IsInBase() || Player.main.IsInSubmarine())
            {
                ikAimLeftArm = false;
                ikAimRightArm = false;
                useLeftAimTargetOnPlayer = false;
            }
            else
            {
                ikAimLeftArm = true;
                ikAimRightArm = true;
                useLeftAimTargetOnPlayer = true;
            }

            if(aiming)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, SNCameraRoot.main.GetAimingTransform().rotation, 0.2f);
                transform.position = Vector3.Lerp(transform.position, MainCamera.camera.transform.position, 0.2f);
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, 0.2f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.2f);
            }
        }

        public override bool OnRightHandDown()
        {
            return false;
        }

        public override bool OnRightHandHeld()
        {
            if (canShoot && energyMixin.charge > 0f && (!Player.main.IsInBase() && !Player.main.IsInSubmarine()))
            {
                if (charge < 1f)
                {
                    charge += 0.02f;
                    if (!chargeEffect.isPlaying)
                    {
                        chargeEffect.Play();
                    }
                    MainCameraControl.main.ShakeCamera(0.2f, 0.5f, MainCameraControl.ShakeMode.Sqrt, 2f);
                    chargeSound.Play();
                }
                else
                {
                    charge = 0f;
                    canShoot = false;
                    Player.main.armsController.animator.SetBool("using_tool", false);
                    chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    shootEffect.Play();
                    energyMixin.ConsumeEnergy(50f);

                    chargeSound.Stop();
                    Utils.PlayFMODAsset(shootSound, transform);

                    GameObject bulletInstance = Instantiate(bulletPrefab, bulletPrefab.transform.position, bulletPrefab.transform.rotation);
                    bulletInstance.transform.parent = null;
                    bulletInstance.SetActive(true);
                    bulletInstance.GetComponent<Rigidbody>().velocity = SNCameraRoot.main.GetAimingTransform().forward.normalized;
                    bulletInstance.AddComponent<BulletControl>().rb = bulletInstance.GetComponent<Rigidbody>();
                    Destroy(bulletInstance, 5f);
                }
            }
            return true;
        }

        public override void OnDraw(Player p)
        {
            base.OnDraw(p);
            charge = 0f;
            chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            shootEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public override void OnHolster()
        {
            base.OnHolster();
            charge = 0f;
            chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            shootEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public override bool OnRightHandUp()
        {
            chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            charge = 0f;
            canShoot = true;
            chargeSound.Stop();

            

            return true;
        }

        public override bool OnAltDown()
        {
            sonarTimer = 0.5f;
            aimTimer = 0.15f;

            return true;
        }

        public override bool OnAltHeld()
        {
            aimTimer -= Time.deltaTime;

            if (energyMixin.charge > 0f && sonarTimer <= 0f)
            {
                MainCamera.camera.GetComponent<SonarScreenFX>().Ping();
                energyMixin.ConsumeEnergy(5f);
                sonarTimer = 4f;
                sonarSound.Play();
            }

            sonarTimer -= Time.deltaTime;

            return true;
        }

        public override bool OnAltUp()
        {
            if (aimTimer >= 0f)
            {
                aiming = !aiming;
            }
            aimTimer = 0f;
            sonarTimer = 0f;

            return true;
        }
    }
}
