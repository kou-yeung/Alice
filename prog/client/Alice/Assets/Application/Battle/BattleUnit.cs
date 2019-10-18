/// <summary>
/// バトルユニット:キャラ一体分
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;
using Zoo.Assets;
using Alice.Entities;
using System.Linq;

namespace Alice
{
    public class BattleUnit
    {
        /// <summary>
        /// 参照情報
        /// </summary>
        public class Current
        {
            public int HP;
            public int Wait;

            public Current(Character data)
            {
                this.Wait = data.Wait;
            }
        }

        public GameObject root { get; private set; }
        public Actor actor { get; private set; }

        public string uniq { get; private set; }
        public BattleConst.Side side { get; private set; }
        public Character characterData { get; private set; }
        public Current current { get; private set; }
        public List<Skill> skills { get; private set; } = new List<Skill>();
        public string[] ais { get; private set; }

        public BattleUnit(string uniq, string id, BattleConst.Side side)
        {
            this.uniq = uniq;
            this.characterData = MasterData.characters.First(v => v.ID == id);
            this.current = new Current(this.characterData);
            this.ais = MasterData.personalities.First(v => v.Name == this.characterData.Personality).AI;

            // スキルID -> スキルデータ
            foreach (var skill in new[] { "Skill_001_001" })
            {
                this.skills.Add(MasterData.skills.First(v => v.ID == skill));
            }

            root = new GameObject(this.uniq);
            this.side = side;
            var prefab = LoaderService.Instance.Load<GameObject>("Actor/Actor.prefab");
            actor = GameObject.Instantiate(prefab).GetComponent<Actor>();
            actor.transform.SetParent(root.transform, true);

            var sprites = LoaderService.Instance.Load<Sprites>(string.Format("Character/$yuhinamv{0}.asset", this.characterData.Image));
            actor.sprites = sprites;

            if(side == BattleConst.Side.Enemy)
            {
                root.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        /// <summary>
        /// 発動トリガ
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int Trigger(BattleConst.SkillType type)
        {
            return this.characterData.Trigger[(int)type];
        }
    }
}
