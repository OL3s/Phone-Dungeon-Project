using MyStructs;

namespace MyClasses {
	public class GameData {
		public int Gold {get; private set;}
		public int Wave {get; private set;}
		public Biomes biome {get; set;}
		public int Kills {get; private set;}
		public int KillsHeavy {get; private set;}
		
		void AddGold(int amount) {
			Gold += amount;
		}
		
		void AddWave() {
			Wave ++;
		}
		
		void AddKill(bool isHeavy) {
			Kills++;
			KillsHeavy += (isHeavy) ? 1 : 0;
		}
	}

	public class PermData {
		public int[] Gems {get; set;}
	}
	
	public class InventoryData {
		
	}

	public class Item {
		public string Name {get;}
		public int Cost {get;}
		public int Condition {get; set;}
	}
}
