using System;
using Godot;
using MyEnums;
using Combat;

namespace Items
{
    public class Item
    {
        public Texture2D Icon { get; set; }
        public string Name { get; }
        public int Cost { get; }
        public int Condition { get; set; }

        public Item(Texture2D icon, string name, int cost, int condition = 100)
        {
            Icon = icon;
            Name = name;
            Cost = cost;
            Condition = condition;
        }
    }

    public class Weapon : Item
    {
        public Attack AttackData { get; set; }

        public Weapon(Texture2D icon, string name, int cost, Attack attackData, int condition = 100)
            : base(icon, name, cost, condition)
        {
            AttackData = attackData;
        }
    }

}