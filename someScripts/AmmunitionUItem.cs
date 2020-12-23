/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine;
using UnrealFPS.Runtime;
using static UnrealFPS.Editor.UProjectManager;

namespace UnrealFPS.Editor
{
    [UPMItem("Ammunition", 5, ItemType.General)]
    public sealed class AmmunitionUItem
    {
        internal static class ContentProperties
        {
            public readonly static GUIContent BaseOptions = new GUIContent("Base Options");
            public readonly static GUIContent BulletType = new GUIContent("Bullet Type", "Type of bullet.");
            public readonly static GUIContent Model = new GUIContent("Model", "Bullet model.");
            public readonly static GUIContent Damage = new GUIContent("Damage", "Bullet hit damage.");
            public readonly static GUIContent Variance = new GUIContent("Variance", "Bullet variance (for shotgun bullet).");
            public readonly static GUIContent NumberBullet = new GUIContent("Number Bullet", "Number bullet (for shotgun bullet).");
            public readonly static GUIContent B_ExplosionRadius = new GUIContent("Explosion Radius", "Bullet explosion radius when hit on some object.");
            public readonly static GUIContent B_ExplosionPower = new GUIContent("Explosion Power", "Bullet explosion power when hit on some object in explosion radius.");
            public readonly static GUIContent B_ExplosionEffect = new GUIContent("Explosion Effect", "Bullet explosion effect will be played when bullet hit on some object.");
            public readonly static GUIContent DecalProperties = new GUIContent("Decal Properties", "Decal properties asset.");
            public readonly static GUIContent ExplosionProperties = new GUIContent("Explosion Properties", "ExplosionProperties asset.");
            public readonly static GUIContent G_ExplosionRadius = new GUIContent("Radius", "Radius for detect gameobjects.");
            public readonly static GUIContent G_ExplosionEffect = new GUIContent("Effect", "Explosion effect.");
            public readonly static GUIContent ExplosionSound = new GUIContent("Sound", "Explosion sound.");
            public readonly static GUIContent Delay = new GUIContent("Delay", "Grenade life time.");
        }

        internal sealed class PhysicsBulletProperties
        {
            private PhysicsBullet.BulletType bulletType;
            private GameObject bullet;
            private string model;
            private int damage;
            private float delay;
            private int numberBullet = 1;
            private float variance;
            private ParticleSystem explosionEffect;
            private AudioClip explosionSound;
            private float explosionRadius;
            private float explosionPower;
            private DecalProperties decalProperties;


            public PhysicsBullet.BulletType GetBulletType()
            {
                return bulletType;
            }

            public void SetBulletType(PhysicsBullet.BulletType value)
            {
                bulletType = value;
            }

            public GameObject GetBullet()
            {
                return bullet;
            }

            public void SetBullet(GameObject value)
            {
                bullet = value;
            }

            public string GetModel()
            {
                return model;
            }

            public void SetModel(string value)
            {
                model = value;
            }

            public int GetDamage()
            {
                return damage;
            }

            public void SetDamage(int value)
            {
                damage = value;
            }

            public float GetDelay()
            {
                return delay;
            }

            public void SetDelay(float value)
            {
                delay = value;
            }

            public int GetNumberBullet()
            {
                return numberBullet;
            }

            public void SetNumberBullet(int value)
            {
                numberBullet = value;
            }

            public float GetVariance()
            {
                return variance;
            }

            public void SetVariance(float value)
            {
                variance = value;
            }

            public ParticleSystem GetExplosionEffect()
            {
                return explosionEffect;
            }

            public void SetExplosionEffect(ParticleSystem value)
            {
                explosionEffect = value;
            }

            public AudioClip GetExplosionSound()
            {
                return explosionSound;
            }

            public void SetExplosionSound(AudioClip value)
            {
                explosionSound = value;
            }

            public float GetExplosionRadius()
            {
                return explosionRadius;
            }

            public void SetExplosionRadius(float value)
            {
                explosionRadius = value;
            }

            public float GetExplosionPower()
            {
                return explosionPower;
            }

            public void SetExplosionPower(float value)
            {
                explosionPower = value;
            }

            public DecalProperties GetDecalProperties()
            {
                return decalProperties;
            }

            public void SetDecalProperties(DecalProperties value)
            {
                decalProperties = value;
            }
        }

        internal sealed class GrenadeProperties
        {
            private GameObject grenadeObject;
            private string model;
            private float radius = 1.25f;
            private ParticleSystem explosionEffect;
            private AudioClip explosionSound;
            private ExplosionProperties explosionProperties;
            private DecalProperties decalProperties;
            private float delay = 5.0f;

            public GameObject GetGrenadeObject()
            {
                return grenadeObject;
            }

            public void SetGrenadeObject(GameObject value)
            {
                grenadeObject = value;
            }

            public string GetModel()
            {
                return model;
            }

            public void SetModel(string value)
            {
                model = value;
            }

            public float GetRadius()
            {
                return radius;
            }

            public void SetRadius(float value)
            {
                radius = value;
            }

            public ParticleSystem GetExplosionEffect()
            {
                return explosionEffect;
            }

            public void SetExplosionEffect(ParticleSystem value)
            {
                explosionEffect = value;
            }

            public AudioClip GetExplosionSound()
            {
                return explosionSound;
            }

            public void SetExplosionSound(AudioClip value)
            {
                explosionSound = value;
            }

            public ExplosionProperties GetExplosionProperties()
            {
                return explosionProperties;
            }

            public void SetExplosionProperties(ExplosionProperties value)
            {
                explosionProperties = value;
            }

            public DecalProperties GetDecalProperties()
            {
                return decalProperties;
            }

            public void SetDecalProperties(DecalProperties value)
            {
                decalProperties = value;
            }

            public float GetDelay()
            {
                return delay;
            }

            public void SetDelay(float value)
            {
                delay = value;
            }
        }

        private static RayBullet rayBullet;
        private static PhysicsBulletProperties physicsBulletProperties;
        private static GrenadeProperties grenadeProperties;
        private static IEditorDelay editorDelay;
        private static string[] toolbarItems;
        private static int toolbarActiveIndex;
        private static GameObject createdObject;

        public static void OnEnable()
        {
            CreateInstances();
            editorDelay = new EditorDelay(0.1f);
            toolbarItems = new string[3] { "Ray Bullet", "Physics Bullet", "Grenade" };
        }

        public static void OnGUI()
        {
            toolbarActiveIndex = GUILayout.Toolbar(toolbarActiveIndex, toolbarItems);
            GUILayout.Space(10);
            switch (toolbarActiveIndex)
            {
                case 0:
                    RayBulletGUI();
                    break;
                case 1:
                    PhysicsBulletGUI();
                    break;
                case 2:
                    GrenadeGUI();
                    break;
                default:
                    RayBulletGUI();
                    break;
            }
        }

        private static void RayBulletGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label(ContentProperties.BaseOptions, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            rayBullet.SetModel(EditorGUILayout.TextField(ContentProperties.Model, rayBullet.GetModel()));
            rayBullet.SetDamage(EditorGUILayout.IntField(ContentProperties.Damage, rayBullet.GetDamage()));
            rayBullet.SetVariance(EditorGUILayout.FloatField(ContentProperties.Variance, rayBullet.GetVariance()));
            rayBullet.SetNumberBullet(EditorGUILayout.IntField(ContentProperties.NumberBullet, rayBullet.GetNumberBullet()));
            rayBullet.SetDecalProperties(UEditor.ObjectField<DecalProperties>(ContentProperties.DecalProperties, rayBullet.GetDecalProperties(), false));

            GUILayout.Space(5);
            UEditor.HorizontalLine();
            GUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(rayBullet.GetModel()));
            if (UEditor.Button("Create", "Right", GUILayout.Width(70)))
            {
                string path = EditorUtility.SaveFilePanelInProject("Create new RayBullet", rayBullet.GetModel(), "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    string name = System.IO.Path.GetFileName(path);
                    path = path.Replace(name, "");
                    ScriptableObjectUtility.CreateAsset(rayBullet, path, name);
                    CreateInstances();
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private static void PhysicsBulletGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label(ContentProperties.BaseOptions, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);

            physicsBulletProperties.SetBulletType((PhysicsBullet.BulletType) EditorGUILayout.EnumPopup(ContentProperties.BulletType, physicsBulletProperties.GetBulletType()));
            physicsBulletProperties.SetBullet(UEditor.ObjectField<GameObject>(new GUIContent("Bullet", "Bullet gameobject."), physicsBulletProperties.GetBullet(), true));
            if (physicsBulletProperties.GetBullet() == null)
            {
                UEditorHelpBoxMessages.Error("Bullet object cannot be empty!", "Add bullet gameobject.");
            }
            physicsBulletProperties.SetModel(EditorGUILayout.TextField(ContentProperties.Model, physicsBulletProperties.GetModel()));
            physicsBulletProperties.SetDamage(EditorGUILayout.IntField(ContentProperties.Damage, physicsBulletProperties.GetDamage()));
            physicsBulletProperties.SetDelay(EditorGUILayout.FloatField(ContentProperties.Delay, physicsBulletProperties.GetDelay()));

            if (physicsBulletProperties.GetBulletType() == PhysicsBullet.BulletType.Standard)
            {
                physicsBulletProperties.SetVariance(EditorGUILayout.FloatField(ContentProperties.Variance, physicsBulletProperties.GetVariance()));
                physicsBulletProperties.SetNumberBullet(EditorGUILayout.IntField(ContentProperties.NumberBullet, physicsBulletProperties.GetNumberBullet()));
            }
            else if (physicsBulletProperties.GetBulletType() == PhysicsBullet.BulletType.Rocket)
            {
                physicsBulletProperties.SetExplosionRadius(EditorGUILayout.FloatField(ContentProperties.B_ExplosionRadius, physicsBulletProperties.GetExplosionRadius()));
                physicsBulletProperties.SetExplosionPower(EditorGUILayout.FloatField(ContentProperties.B_ExplosionPower, physicsBulletProperties.GetExplosionPower()));
                physicsBulletProperties.SetExplosionEffect(UEditor.ObjectField<ParticleSystem>(ContentProperties.B_ExplosionEffect, physicsBulletProperties.GetExplosionEffect(), true));
                physicsBulletProperties.SetExplosionSound(UEditor.ObjectField<AudioClip>(ContentProperties.ExplosionSound, physicsBulletProperties.GetExplosionSound(), true));
            }
            physicsBulletProperties.SetDecalProperties(UEditor.ObjectField<DecalProperties>(ContentProperties.DecalProperties, physicsBulletProperties.GetDecalProperties(), false));

            GUILayout.Space(5);
            UEditor.HorizontalLine();
            GUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(physicsBulletProperties.GetModel()) || physicsBulletProperties.GetBullet() == null);
            if (UEditor.Button("Create", "Right", GUILayout.Width(70)))
            {
                createdObject = CreatePhysicsBullet(physicsBulletProperties);
            }
            EditorGUI.EndDisabledGroup();

            if (createdObject != null && editorDelay.WaitForSeconds())
            {
                if (UDisplayDialogs.Message("Create Successful", "Physics bullet was created on scene!\nSetup bullet components before start play.", "Select", "Ok"))
                {
                    Selection.activeGameObject = createdObject;
                }
                createdObject = null;
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private static void GrenadeGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label(ContentProperties.BaseOptions, UEditorStyles.SectionHeaderLabel);
            GUILayout.Space(7);
            grenadeProperties.SetGrenadeObject((GameObject) EditorGUILayout.ObjectField(new GUIContent("Grenade", "Grenade gameobject."), grenadeProperties.GetGrenadeObject(), typeof(GameObject), true));
            if (grenadeProperties.GetGrenadeObject() == null)
            {
                UEditorHelpBoxMessages.Error("Grenade object cannot be empty!", "Add grenade gameobject.");
            }
            grenadeProperties.SetModel(EditorGUILayout.TextField(ContentProperties.Model, grenadeProperties.GetModel()));
            grenadeProperties.SetRadius(EditorGUILayout.FloatField(ContentProperties.G_ExplosionRadius, grenadeProperties.GetRadius()));
            grenadeProperties.SetDelay(EditorGUILayout.FloatField(ContentProperties.Delay, grenadeProperties.GetDelay()));
            grenadeProperties.SetExplosionEffect((ParticleSystem) EditorGUILayout.ObjectField(ContentProperties.G_ExplosionEffect, grenadeProperties.GetExplosionEffect(), typeof(ParticleSystem), true));
            grenadeProperties.SetExplosionSound((AudioClip) EditorGUILayout.ObjectField(ContentProperties.ExplosionSound, grenadeProperties.GetExplosionSound(), typeof(AudioClip), true));
            grenadeProperties.SetExplosionProperties((ExplosionProperties) EditorGUILayout.ObjectField(ContentProperties.ExplosionProperties, grenadeProperties.GetExplosionProperties(), typeof(ExplosionProperties), true));
            grenadeProperties.SetDecalProperties((DecalProperties) EditorGUILayout.ObjectField(ContentProperties.DecalProperties, grenadeProperties.GetDecalProperties(), typeof(DecalProperties), true));

            GUILayout.Space(5);
            UEditor.HorizontalLine();
            GUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(grenadeProperties.GetModel()) || grenadeProperties.GetGrenadeObject() == null);
            if (UEditor.Button("Create", "Right", GUILayout.Width(70)))
            {
                createdObject = CreateGreanade(grenadeProperties);
            }
            EditorGUI.EndDisabledGroup();

            if (createdObject != null && editorDelay.WaitForSeconds())
            {
                if (UDisplayDialogs.Message("Create Successful", "Greanade was created on scene!\nSetup grenade components before start play.", "Select", "Ok"))
                {
                    Selection.activeGameObject = createdObject;
                }
                createdObject = null;
            }
            GUILayout.Space(5);

            GUILayout.EndVertical();
        }

        private static void CreateInstances()
        {
            rayBullet = ScriptableObject.CreateInstance<RayBullet>();
            physicsBulletProperties = new PhysicsBulletProperties();
            grenadeProperties = new GrenadeProperties();
        }

        private static GameObject CreatePhysicsBullet(PhysicsBulletProperties properties)
        {
            // Initialize gameobjects.
            GameObject bullet = new GameObject(properties.GetModel());
            GameObject bulletModel = GameObject.Instantiate(properties.GetBullet(), Vector3.zero, Quaternion.identity, bullet.transform);
            ParticleSystem explosionEffect = null;
            if (properties.GetBulletType() == PhysicsBullet.BulletType.Rocket || properties.GetExplosionEffect() != null)
            {
                explosionEffect = GameObject.Instantiate(properties.GetExplosionEffect(), Vector3.zero, Quaternion.identity, bullet.transform);
            }

            // Initialize components
            BoxCollider boxCollider = UEditorInternal.AddComponent<BoxCollider>(bullet);
            PhysicsBullet physicsBulletComponent = UEditorInternal.AddComponent<PhysicsBullet>(bullet);
            Rigidbody rigidbody = UEditorInternal.AddComponent<Rigidbody>(bullet);
            AudioSource audioSource = UEditorInternal.AddComponent<AudioSource>(bullet);

            // Setup PhysicsBullet component.
            physicsBulletComponent.SetBulletType(properties.GetBulletType());
            physicsBulletComponent.SetBullet(properties.GetBullet());
            physicsBulletComponent.SetModel(properties.GetModel());
            physicsBulletComponent.SetDamage(properties.GetDamage());
            physicsBulletComponent.SetVariance(properties.GetVariance());
            physicsBulletComponent.SetNumberBullet(properties.GetNumberBullet());
            physicsBulletComponent.SetExplosionRadius(properties.GetExplosionRadius());
            physicsBulletComponent.SetExplosionPower(properties.GetExplosionPower());
            if (properties.GetBulletType() == PhysicsBullet.BulletType.Rocket || explosionEffect != null)
            {
                physicsBulletComponent.SetExplosionEffect(explosionEffect);
            }
            physicsBulletProperties.SetExplosionSound(properties.GetExplosionSound());
            physicsBulletComponent.SetDecalProperties(properties.GetDecalProperties());

            // Setup BoxCollider component.
            Renderer renderer = bulletModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                boxCollider.center = renderer.bounds.center;
                boxCollider.size = renderer.bounds.size;
            }

            // Apply components position.
            UEditorInternal.MoveComponentBottom<PhysicsBullet>(bullet.transform);
            UEditorInternal.MoveComponentBottom<Rigidbody>(bullet.transform);
            UEditorInternal.MoveComponentBottom<BoxCollider>(bullet.transform);
            UEditorInternal.MoveComponentBottom<AudioSource>(bullet.transform);

            return bullet;
        }

        private static GameObject CreateGreanade(GrenadeProperties properties)
        {
            // Initialize gameobjects.
            GameObject grenade = new GameObject(properties.GetModel());
            GameObject grenadeModel = GameObject.Instantiate(properties.GetGrenadeObject(), Vector3.zero, Quaternion.identity, grenade.transform);

            // Initialize components
            Grenade grenadeComponent = UEditorInternal.AddComponent<Grenade>(grenade);
            Rigidbody rigidbody = UEditorInternal.AddComponent<Rigidbody>(grenade);
            SphereCollider sphereCollider = UEditorInternal.AddComponent<SphereCollider>(grenade);
            AudioSource audioSource = UEditorInternal.AddComponent<AudioSource>(grenade);

            // Setup Grenade component.
            grenadeComponent.SetGrenadeObject(properties.GetGrenadeObject());
            grenadeComponent.SetRadius(properties.GetRadius());
            grenadeComponent.SetExplosionEffect(properties.GetExplosionEffect());
            grenadeComponent.SetExplosionSound(properties.GetExplosionSound());
            grenadeComponent.SetExplosionProperties(properties.GetExplosionProperties());
            grenadeComponent.SetDecalProperties(properties.GetDecalProperties());
            grenadeComponent.SetDelay(properties.GetDelay());

            // Setup SphereCollider component.
            Renderer renderer = grenadeModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                sphereCollider.center = renderer.bounds.center;
                sphereCollider.radius = renderer.bounds.size.x;
            }

            // Apply components position.
            UEditorInternal.MoveComponentBottom<Grenade>(grenade.transform);
            UEditorInternal.MoveComponentBottom<Rigidbody>(grenade.transform);
            UEditorInternal.MoveComponentBottom<SphereCollider>(grenade.transform);
            UEditorInternal.MoveComponentBottom<AudioSource>(grenade.transform);

            return grenade;
        }
    }
}