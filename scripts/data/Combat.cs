using System;
using Godot;
using MyEnums;

namespace Combat
{
    public abstract class Attack
    {
        public abstract AttackType Type { get; }

    }

    public class MeleeAttack : Attack
    {
        public override AttackType Type => AttackType.Collision;
        public float Radius { get; set; }
        public float Duration { get; set; }
        public CollisionContainer CollisionData { get; set; }
        public Attack[] NextAttacks { get; set; }
        public MeleeAttack(float radius, float duration, CollisionContainer collisionData, Attack[] nextAttacks)
        {
            Radius = radius;
            Duration = duration;
            CollisionData = collisionData;
            NextAttacks = nextAttacks;
        }
    }

    public class RangedAttack : Attack
    {
        public override AttackType Type => AttackType.Projectile;
        public CollisionContainer CollisionData { get; set; }
        public float Speed { get; set; }
        public float Range { get; set; }
        public Attack[] NextAttacks { get; set; }

        public RangedAttack(float speed, float range, CollisionContainer collisionData, Attack[] nextAttacks)
        {
            Speed = speed;
            Range = range;
            CollisionData = collisionData;
            NextAttacks = nextAttacks;
        }
    }

    public class BeamAttack : Attack
    {
        public override AttackType Type => AttackType.Beam;
        public CollisionContainer CollisionData { get; set; }
        public float Duration { get; set; }
        public float Width { get; set; }
        public float Range { get; set; }
        public Attack[] NextAttacks { get; set; }

        public BeamAttack(float duration, float width, float range, CollisionContainer collisionData, Attack[] nextAttacks)
        {
            Duration = duration;
            Width = width;
            Range = range;
            CollisionData = collisionData;
            NextAttacks = nextAttacks;
        }

    }

    public class AreaOfEffectAttack : Attack
    {
        public override AttackType Type => AttackType.AreaOfEffect;
        public CollisionContainer CollisionData { get; set; }
        public float Radius { get; set; }
        public float Duration { get; set; }
        public Attack[] NextAttacks { get; set; }

        public AreaOfEffectAttack(float radius, float duration, CollisionContainer collisionData, Attack[] nextAttacks)
        {
            Radius = radius;
            Duration = duration;
            CollisionData = collisionData;
            NextAttacks = nextAttacks;
        }
    }

    public class SummonAttack : Attack
    {
        public override AttackType Type => AttackType.Summon;
        public string SummonedEntity { get; set; }
        public int Duration { get; set; } // Duration in seconds, 0 for permanent
        public Attack[] NextAttacks { get; set; }

        public SummonAttack(string summonedEntity, int duration, Attack[] nextAttacks)
        {
            SummonedEntity = summonedEntity;
            Duration = duration;
            NextAttacks = nextAttacks;
        }
    }

    public class ThrowAttack : Attack
    {
        public override AttackType Type => AttackType.Throw;
        public CollisionContainer CollisionData { get; set; }
        public float Range { get; set; }
        public Attack[] NextAttacks { get; set; }

        public ThrowAttack(float range, CollisionContainer collisionData, Attack[] nextAttacks)
        {
            Range = range;
            CollisionData = collisionData;
            NextAttacks = nextAttacks;
        }
    }
    public class TeleportAttack : Attack
    {
        public override AttackType Type => AttackType.Teleport;
        public float Distance { get; set; }
        public Attack[] NextAttacks { get; set; }

        public TeleportAttack(float distance, Attack[] nextAttacks)
        {
            Distance = distance;
            NextAttacks = nextAttacks;
        }
    }

    public class NoAttack : Attack
    {
        public override AttackType Type => AttackType.None;
    }

    public struct Damage
    {
        public DamageType Type { get; }
        public int Amount { get; }

        public Damage(DamageType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }

    public struct StatusEffect
    {
        public StatusType Type { get; }
        public float Duration { get; }

        public StatusEffect(StatusType type, float duration)
        {
            Type = type;
            Duration = duration;
        }
    }

    public class CollisionContainer
    {
        public Damage[] Damages { get; set; }
        public StatusEffect[] StatusEffects { get; set; }

        public CollisionContainer(Damage[] damages, StatusEffect[] statusEffects)
        {
            Damages = damages;
            StatusEffects = statusEffects;
        }

        public Damage[] DamageConstructor(DamageType[] DamageTypes, int[] Values)
        {
            if (DamageTypes.Length != Values.Length)
            {
                throw new ArgumentException("DamageTypes and Values arrays must have the same length.");
            }

            Damage[] damages = new Damage[DamageTypes.Length];
            for (int i = 0; i < DamageTypes.Length; i++)
            {
                damages[i] = new Damage(DamageTypes[i], Values[i]);
            }
            return damages;
        }

        public StatusEffect[] StatusEffectConstructor(StatusType[] StatusTypes, float[] Durations)
        {
            if (StatusTypes.Length != Durations.Length)
            {
                throw new ArgumentException("StatusTypes and Durations arrays must have the same length.");
            }

            StatusEffect[] statusEffects = new StatusEffect[StatusTypes.Length];
            for (int i = 0; i < StatusTypes.Length; i++)
            {
                statusEffects[i] = new StatusEffect(StatusTypes[i], Durations[i]);
            }
            return statusEffects;
        }

    }
}