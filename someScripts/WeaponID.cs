/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnrealFPS.Runtime
{
    [CreateAssetMenu(fileName = "Weapon ID", menuName = UnrealFPSInfo.NAME + "/Weapon/Weapon ID", order = 123)]
    public partial class WeaponID : ScriptableObject, IWeaponID, IEquatable<WeaponID>
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private string group;
        [SerializeField] private Sprite image;
        [SerializeField] private DropProperties dropProperties;

        public string GetID()
        {
            return id;
        }

        public void SetID(string value)
        {
            id = value;
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public void SetDisplayName(string value)
        {
            displayName = value;
        }

        public string GetDescription()
        {
            return description;
        }

        public void SetDescription(string value)
        {
            description = value;
        }

        public string GetGroup()
        {
            return group;
        }

        public void SetGroup(string value)
        {
            group = value;
        }

        public Sprite GetImage()
        {
            return image;
        }

        public void SetImage(Sprite value)
        {
            image = value;
        }

        public DropProperties GetDropProperties()
        {
            return dropProperties;
        }

        public void SetDropProperties(DropProperties value)
        {
            dropProperties = value;
        }

        public static string GenerateID()
        {
            string id = System.Guid.NewGuid().ToString().ToUpper();
            id = id.Replace("-", "");
            return id;
        }

        public static bool operator ==(WeaponID left, WeaponID right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WeaponID left, WeaponID right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            return (obj is WeaponID metrics) && Equals(metrics);
        }

        public bool Equals(WeaponID other)
        {
            return (id) == (other.id);
        }

        public override int GetHashCode()
        {
            return (id, displayName, description, group, image, dropProperties).GetHashCode();
        }
    }
}