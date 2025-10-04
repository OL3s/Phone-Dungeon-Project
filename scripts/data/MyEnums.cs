namespace MyEnums
{
	/// <summary> Enumeration for different biomes in the game. </summary>
	public enum Biomes
	{
		Woodland,
		Swamp,
		Desert,
		Mountain,
		Cave,
		IceCave,
		Volcanic,
		Ruins,
		Crypt,
		Abyss,
	}
	/// <summary> Enumeration for different mob families in the game. </summary>
	public enum MobFamilies
	{
		Wild,
		Elemental,
		Goblin,
		Bandit,
		Undead,
		Cultist,
		Abyssal
	}
	/// <summary> Enumeration for different damage types in the game. </summary>
	public enum DamageType
	{
		Physical,
		Slash,
		Pierce,
		Crush,
		Heat,
		Cold,
		Acid
	}
	/// <summary> Enumeration for different status effect types in the game. </summary>
	public enum StatusType
	{
		Hit,
		Stun,
		Slow,
		Burn,
		Freeze
	}
	public enum AttackType
	{
		Collision,
		Teleport,
		Projectile,
		Beam,
		Summon,
		AreaOfEffect,
		Throw,
		None
	}
	public enum MissionType
	{
		Rescue,     // Rescue NPC(s)
		Eliminate,  // Eliminate target(s) / Boss fight
		Rush,        // Reach end in time
		Locate,      // Locate item(s) / Collectibles
		None        // No mission

	}
}
