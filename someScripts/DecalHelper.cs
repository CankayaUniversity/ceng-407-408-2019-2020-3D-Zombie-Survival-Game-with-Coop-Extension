/* ================================================================
   ---------------------------------------------------
   Project   :    Unreal FPS
   Publisher :    Renowned Studio
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2019 All rights reserved.
   ================================================================ */

using UnityEngine;
using UnrealFPS.Utility;

namespace UnrealFPS.Runtime
{
    public static class DecalHelper
    {
        /// <summary>
        /// Get decal property from DecalProperties in compliance RayCastHit surface.
        /// </summary>
        public static DecalProperty GetDecalPropertyBySurface(DecalProperties decalProperties, RaycastHit rayCastHit)
        {
            if (decalProperties == null)
                return DecalProperty.Empty;

            Object surfaceInfo = null;
            for (int i = 0, length = decalProperties.GetLength(); i < length; i++)
            {
                DecalProperty decalProperty = decalProperties.GetProperty(i);
                if (decalProperty.GetTexture() != null)
                {
                    surfaceInfo = SurfaceHelper.GetSurfaceTexture(rayCastHit.collider, rayCastHit.point);
                    if (surfaceInfo == decalProperty.GetTexture())
                    {
                        return decalProperties.GetProperty(i);
                    }
                }
                
                if (decalProperty.GetPhysicMaterial() != null)
                {
                    surfaceInfo = SurfaceHelper.GetSurfaceMaterial(rayCastHit.collider, rayCastHit.point);
                    if (surfaceInfo == decalProperty.GetPhysicMaterial())
                    {
                        return decalProperties.GetProperty(i);
                    }
                }
            }
            return DecalProperty.Empty;
        }

        /// <summary>
        /// Get decal property from DecalProperties in compliance ContactPoint surface.
        /// </summary>
        public static DecalProperty GetDecalPropertyBySurface(DecalProperties decalProperties, ContactPoint contactPoint)
        {
            if (decalProperties == null)
                return DecalProperty.Empty;

            Object surfaceInfo = null;
            for (int i = 0, length = decalProperties.GetLength(); i < length; i++)
            {
                DecalProperty decalProperty = decalProperties.GetProperty(i);

                if (decalProperty.GetTexture() != null)
                {
                    surfaceInfo = SurfaceHelper.GetSurfaceTexture(contactPoint.thisCollider, contactPoint.point);
                    if (surfaceInfo == decalProperty.GetTexture())
                    {
                        return decalProperties.GetProperty(i);
                    }
                }
                
                if (decalProperty.GetPhysicMaterial() != null)
                {
                    surfaceInfo = SurfaceHelper.GetSurfaceMaterial(contactPoint.thisCollider, contactPoint.point);
                    if (surfaceInfo == decalProperty.GetPhysicMaterial())
                    {
                        return decalProperties.GetProperty(i);
                    }
                }
            }
            return DecalProperty.Empty;
        }
    }
}