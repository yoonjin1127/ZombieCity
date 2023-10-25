using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 여러 객체가 참조할 고유 데이터를 생성하여 보관
// 고유 데이터를 참조하여 사용하기에 불러오는 시간을 줄임

namespace DesignPattern
{
    public class FlyWeight
    {
        // 공공 저장소
        public class MonsterDataContainer
        {
            private static MonsterDataContainer instance;
            public static MonsterDataContainer Instance { get { return instance; } }

            Dictionary<string, MonsterData> dictionary;

            public MonsterDataContainer()
            {
                instance = this;
                dictionary = new Dictionary<string, MonsterData>();

                dictionary.Add("오크", new MonsterData("오크", 100, 20, 10, "무자비한 몬스터입니다."));
                dictionary.Add("드래곤", new MonsterData("드래곤", 500, 120, 30, "브레스를 피해야 합니다."));
                dictionary.Add("슬라임", new MonsterData("슬라임", 3, 10, 5, "분열하는 산성 몬스터입니다."));
            }

            public MonsterData GetData(string name)
            {
                return dictionary[name];
            }
        }

        public class MonsterData
        {
            public string Name { get; private set; }
            public int MaxHP { get; private set; }
            public int Damage { get; private set; }
            public int Shield { get; private set; }
            public string Desription { get; private set; }

            public MonsterData(string name, int maxHP, int damage, int shield, string desription)
            {
                this.Name = name;
                this.MaxHP = maxHP;
                this.Damage = damage;
                this.Shield = shield;
                this.Desription = desription;
            }
        }

        public class Monster
        {
            protected MonsterData data;
        }

        public class Orc : Monster
        {
            public Orc()
            {
                this.data = MonsterDataContainer.Instance.GetData("오크");
            }
        }

        public class Dragon : Monster
        {
            public Dragon()
            {
                this.data = MonsterDataContainer.Instance.GetData("드래곤");
            }
        }

        public class Slime : Monster
        {
            public Slime()
            {
                this.data = MonsterDataContainer.Instance.GetData("슬라임");
            }
        }
    }
}
