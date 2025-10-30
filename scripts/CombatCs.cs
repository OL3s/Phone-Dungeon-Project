using Godot;
using System;
using System.Collections.Generic;

namespace CombatCs
{
	public class CombatSystem
	{
		public enum StatusType
		{
			Hit,
			Stun,
			Slow,
			Burn,
			Freeze
		}

		public enum DamageType
		{
			Slash,
			Pierce,
			Crush,
			Heat,
			Cold,
			Acid
		}

		// Container for health and defence values on an entity
		public class CombatContainer
		{
			public float Health { get; set; }
			public CombatValues Defence { get; set; }
			public Dictionary<StatusType, float> ActiveStatusEffects { get; set; } = new Dictionary<StatusType, float>();
			public CombatContainer(float health = 100, CombatValues defence = null)
			{
				Health = health;
				Defence = defence ?? new CombatValues();
			}
			private bool ApplyDamage(CombatValues attack)
			{
				float damageTaken = CombatCalculator.CalculateDamageWithDefence(attack, Defence);
				Health -= damageTaken;
				if (Health < 0) Health = 0;
				return Health == 0;
			}
			private void ApplyStatusEffects(CombatValues attack)
			{
				var newEffects = CombatCalculator.CalculateStatusWithDefence(attack, Defence);
				ActiveStatusEffects = CombatCalculator.ApplyStatusEffects(ActiveStatusEffects, newEffects);
			}
			public void ApplyHit(CombatValues attack)
			{
				ApplyDamage(attack);
				ApplyStatusEffects(attack);
			}
		}

		// Container for attack and status effect values
		public class CombatValues
		{
			public Dictionary<StatusType, float> StatusEffects { get; set; } = new Dictionary<StatusType, float>();
			public Dictionary<DamageType, float> DamageEffects { get; set; } = new Dictionary<DamageType, float>();
			public (float damage, float status) DefaultValue { get; set; } = (1f, 1f);

			private void AddStatusEffect(StatusType statusEffect, float duration)
			{
				StatusEffects[statusEffect] = duration;
			}
			private void AddDamage(DamageType damage, float amount)
			{
				DamageEffects[damage] = amount;
			}
			public CombatValues() { }

			// Constructor, using list of tuples for easy initialization of dictionaries
			public CombatValues(
				List<(StatusType type, float duration)> statusEffects = null,
				List<(DamageType type, float value)> damageEffects = null,
				(float damage, float status)? defaultValue = null
			)
			{
				DefaultValue = defaultValue ?? (1f, 1f);
				if (statusEffects != null)
				{
					foreach (var statusEffect in statusEffects)
					{
						AddStatusEffect(statusEffect.type, statusEffect.duration);
					}
				}
				if (damageEffects != null)
				{
					foreach (var damage in damageEffects)
					{
						AddDamage(damage.type, damage.value);
					}
				}
			}
		}

		// Static class for combat calculations
		public static class CombatCalculator
		{
			public static float CalculateDamageRaw(CombatValues action)
			{
				float totalDamage = 0f;
				foreach (var damage in action.DamageEffects.Values)
				{
					totalDamage += damage;
				}
				return totalDamage;
			}

			// Defence calculation assumes that defence values are multipliers (e.g., 0.8 for 20% reduction)
			public static float CalculateDamageWithDefence(CombatValues attack, CombatValues defence)
			{
				// if no damage effects, return 0
				if (attack.DamageEffects.Count == 0)
					return 0f;

				// if no defence, return raw damage
				if (defence == null)
					return CalculateDamageRaw(attack);

				// apply flat damage multiplier to attack.FlatDamage as well
				float totalDamage = 0f;
				foreach (var damage in attack.DamageEffects)
				{
					// defence values for specific damage types are treated as multipliers; default to Defence.DefaultValue
					float defenceValue = defence.DamageEffects.TryGetValue(damage.Key, out var v) ? v : defence.DefaultValue.damage;
					totalDamage += damage.Value * defenceValue;
				}
				return totalDamage;
			}

			public static Dictionary<StatusType, float> CalculateStatusRaw(CombatValues action)
			{
				// if no status effects, return empty dictionary
				if (action.StatusEffects.Count == 0)
					return new Dictionary<StatusType, float>();

				var result = new Dictionary<StatusType, float>();
				foreach (var status in action.StatusEffects)
				{
					result[status.Key] = status.Value;
				}
				return result;
			}

			public static Dictionary<StatusType, float> CalculateStatusWithDefence(CombatValues attack, CombatValues defence)
			{
				// if no status effects, return empty dictionary
				if (attack.StatusEffects.Count == 0)
					return new Dictionary<StatusType, float>();

				// if no defence, return raw status effects
				if (defence == null)
					return CalculateStatusRaw(attack);

				var result = new Dictionary<StatusType, float>();
				foreach (var status in attack.StatusEffects)
				{
					float defenceValue = defence.StatusEffects.TryGetValue(status.Key, out var v) ? v : defence.DefaultValue.status;
					float finalDuration = status.Value * defenceValue;
					result[status.Key] = finalDuration;
				}
				return result;
			}

			public static Dictionary<StatusType, float> ApplyStatusEffects(
				Dictionary<StatusType, float> existingEffects,
				Dictionary<StatusType, float> newEffects)
			{
				var result = new Dictionary<StatusType, float>(existingEffects);
				foreach (var status in newEffects)
				{

					// refresh duration if new effect is stronger
					if (result.ContainsKey(status.Key))
					{
						if (status.Value > result[status.Key])
							result[status.Key] = status.Value;
					}

					// add new effect if not present
					else
						result[status.Key] = status.Value;

				}
				return result;
			}
		}
		
		public static class Test
		{
			public static void RunTests()
			{
				Console.WriteLine("Running Combat System Tests...");

				var ObjectDefencer = new CombatContainer(defence: new CombatValues(
					new List<(StatusType, float)> { 
						(StatusType.Stun, 0.5f) 
					},
					new List<(DamageType, float)> {
						(DamageType.Slash, 0.7f),
						(DamageType.Heat, 0.9f) 
					}
				));

				var attackerValues = new CombatValues(
					new List<(StatusType, float)> { (StatusType.Burn, 50f), (StatusType.Stun, 5f) },
					new List<(DamageType, float)> { (DamageType.Heat, 30f), (DamageType.Slash, 20f) }
				);

				Console.WriteLine($"Object Health Before Attack: {ObjectDefencer.Health}");
				ObjectDefencer.ApplyHit(attackerValues);
				Console.WriteLine($"Object Health After Attack: {ObjectDefencer.Health}");

				Console.WriteLine($"Active Status Effects:");
				foreach (var status in ObjectDefencer.ActiveStatusEffects)
				{
					Console.WriteLine($"{status.Key}: {status.Value}");
				}

				// apply ONLY effect
				var attackerValues2 = new CombatValues(
					new List<(StatusType, float)> { (StatusType.Burn, 100f) },
					null
				);

				ObjectDefencer.ApplyHit(attackerValues2);
				Console.WriteLine($"After Second Attack - Active Status Effects:");
				foreach (var status in ObjectDefencer.ActiveStatusEffects)
				{
					Console.WriteLine($"{status.Key}: {status.Value}");
				}

				// apply ONLY damage
				var attackerValues3 = new CombatValues(
					null,
					new List<(DamageType, float)> { (DamageType.Slash, 50f) }
				);

				ObjectDefencer.ApplyHit(attackerValues3);
				Console.WriteLine($"After Third Attack - Object Health: {ObjectDefencer.Health}");
				Console.WriteLine("Combat System Tests Completed.");
			}
		}
	}
}
