using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace CombatShields;

public class Apparel_Shield : Apparel
{
    public static readonly SoundDef SoundAbsorbDamage = SoundDef.Named("PersonalShieldAbsorbDamage");
    public static readonly SoundDef SoundBreak = SoundDef.Named("PersonalShieldBroken");

    public bool colorable;
    public Vector3 impactAngleVect;
    public Material shieldMat;

    protected bool ShouldDisplay
    {
        get
        {
            if (Wearer.Dead || Wearer.InBed() || Wearer.Downed || Wearer.IsPrisonerOfColony)
            {
                return false;
            }

            return !Wearer.IsColonist || Wearer.Drafted || Wearer.InAggroMentalState;
        }
    }


    public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
    {
        if (dinfo.Instigator == null || dinfo.Def == DamageDefOf.Extinguish || dinfo.Def == DamageDefOf.Smoke ||
            dinfo.Def == DamageDefOf.ToxGas)
        {
            return false;
        }

        var melee = Wearer.skills.GetSkill(SkillDefOf.Melee);
        var random = new Random();
        var chance = random.Next(0, 21);

        if (chance >= melee.levelInt)
        {
            var i = (int)dinfo.Amount;
            i /= 4;

            HitPoints -= i;
        }
        else
        {
            var i = (int)dinfo.Amount;
            i /= 10;

            HitPoints -= i;
        }

        return false;
    }

    [DebuggerHidden]
    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        yield return new Gizmo_ShieldStatus
        {
            shield = this
        };
    }

    public override void DrawWornExtras()
    {
        if (!ShouldDisplay)
        {
            return;
        }

        var num = 0f;
        var vector = Wearer.Drawer.DrawPos;
        vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
        var s = new Vector3(1f, 1f, 1f);
        if (Wearer.Rotation == Rot4.North)
        {
            vector.y = AltitudeLayer.Item.AltitudeFor();
            vector.x -= 0.2f;
            vector.z -= 0.2f;
        }
        else if (Wearer.Rotation == Rot4.South)
        {
            vector.y += 0.033f;
            vector.x += 0.2f;
            vector.z -= 0.2f;
        }
        else if (Wearer.Rotation == Rot4.East)
        {
            vector.z -= 0.2f;
            num = 90f;
        }
        else if (Wearer.Rotation == Rot4.West)
        {
            vector.y = AltitudeLayer.Item.AltitudeFor();
            vector.z -= 0.2f;
            num = 270f;
        }

        if (shieldMat == null)
        {
            shieldMat = MaterialPool.MatFrom(def.graphicData.texPath);
        }

        var matrix = default(Matrix4x4);
        matrix.SetTRS(vector, Quaternion.AngleAxis(num, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, shieldMat, 0);
    }

    [StaticConstructorOnStartup]
    internal class Gizmo_ShieldStatus : Gizmo
    {
        private static readonly Texture2D FullTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        private static readonly Texture2D EmptyTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public Apparel_Shield shield;

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            var rect = new Rect(topLeft.x, topLeft.y, GetWidth(140f), 75f);
            Widgets.DrawWindowBackground(rect);

            var rect2 = rect.ContractedBy(6f);
            var rect3 = rect2;

            rect3.height = rect.height / 2f;

            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, shield.LabelCap);

            var rect4 = rect2;

            rect4.yMin = rect.y + (rect.height / 2f);
            var num = (float)shield.HitPoints / shield.MaxHitPoints;

            Widgets.FillableBar(rect4, num, FullTex, EmptyTex, false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;

            Widgets.Label(rect4, $"{shield.HitPoints} / {shield.MaxHitPoints}");
            Text.Anchor = TextAnchor.UpperLeft;

            return new GizmoResult(0);
        }
    }
}