using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace AlienRifle2
{
    public class RiflePrefab : ModPrefab
    {
        public RiflePrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = MainPatch.bundle.LoadAsset<GameObject>("Assets/AlienRifle/Alien Rifle.prefab");
            obj.GetOrAddComponent<PrefabIdentifier>().ClassId = ClassID;
            obj.GetOrAddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            obj.GetOrAddComponent<TechTag>().type = TechType;
            obj.GetOrAddComponent<Pickupable>().isPickupable = true;
            SkyApplier sky = obj.GetOrAddComponent<SkyApplier>();
            sky.renderers = obj.GetComponentsInChildren<MeshRenderer>();
            sky.anchorSky = Skies.Auto;

            GameObject originalRifle = Resources.Load<GameObject>("WorldEntities/Doodads/Precursor/Prison/Relics/Alien_relic_07");
            Material newMat = originalRifle.GetComponentInChildren<MeshRenderer>().material;
            sky.renderers[0].materials = new Material[] { newMat, newMat };

            sky.renderers[0].GetComponent<MeshFilter>().mesh = originalRifle.GetComponentInChildren<MeshFilter>().mesh;

            VFXFabricating vfx = obj.transform.Find("RifleMesh").gameObject.GetOrAddComponent<VFXFabricating>();
            vfx.localMinY = -0.4f;
            vfx.localMaxY = 0.2f;
            vfx.posOffset = new Vector3(-0.054f, 0.1f, -0.06f);
            vfx.eulerOffset = new Vector3(0f, 0f, 90f);
            vfx.scaleFactor = 1f;

            WorldForces forces = obj.GetOrAddComponent<WorldForces>();

            forces.underwaterGravity = 0f;
            forces.useRigidbody = obj.GetOrAddComponent<Rigidbody>();
            forces.useRigidbody.useGravity = false;

            RifleTool rifle = obj.GetOrAddComponent<RifleTool>();

            rifle.mainCollider = obj.GetComponentInChildren<Collider>();
            rifle.ikAimRightArm = true;
            rifle.ikAimLeftArm = true;
            rifle.useLeftAimTargetOnPlayer = true;
            rifle.chargeEffect = rifle.transform.Find("chargeparticles").GetComponent<ParticleSystem>();
            rifle.shootEffect = rifle.transform.Find("shooteffect").GetComponent<ParticleSystem>();
            rifle.chargeMeter = obj.transform.Find("HUD/ChargeBar");
            rifle.bulletPrefab = obj.transform.Find("BulletPrefab").gameObject;

            rifle.energyMixin = obj.GetOrAddComponent<EnergyMixin>();
            rifle.energyMixin.allowBatteryReplacement = true;
            rifle.energyMixin.compatibleBatteries = new List<TechType>()
            {
                TechType.PrecursorIonBattery
            };

            rifle.energyMixin.defaultBattery = TechType.None;
            rifle.energyMixin.batteryModels = new EnergyMixin.BatteryModels[]
            {
                new EnergyMixin.BatteryModels()
                {
                    model = obj.transform.Find("Battery").gameObject,
                    techType = TechType.PrecursorIonBattery
                }
            };
            rifle.energyMixin.storageRoot = obj.transform.Find("Battery").gameObject.GetOrAddComponent<ChildObjectIdentifier>();

            GameObject seamoth = CraftData.GetPrefabForTechType(TechType.Seamoth);
            rifle.sonarSound = obj.GetOrAddComponent<FMOD_CustomEmitter>();
            rifle.sonarSound.asset = seamoth.GetComponent<SeaMoth>().sonarSound.asset;
            rifle.sonarSound.playOnAwake = false;
            rifle.sonarSound.followParent = true;
            rifle.sonarSound.restartOnPlay = true;

            rifle.shootSound = CraftData.GetPrefabForTechType(TechType.RepulsionCannon).GetComponent<RepulsionCannon>().shootSound;
            rifle.chargeSound = obj.GetOrAddComponent<FMOD_CustomLoopingEmitter>();
            rifle.chargeSound.asset = seamoth.GetComponent<SeaMoth>().pulseChargeSound.asset;
            rifle.chargeSound.followParent = true;
            rifle.sonarSound.restartOnPlay = true;

            rifle.Awake();

            return obj;
        }
    }
}
