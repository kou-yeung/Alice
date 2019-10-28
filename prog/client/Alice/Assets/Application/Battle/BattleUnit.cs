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
        /// 効果x1
        /// </summary>
        class Condition
        {
            public BattleConst.Effect effect { get; private set; }
            public int value { get; private set; }
            public int remain { get; set; }

            public Condition(BattleConst.Effect effect, int value, int remain = 2)
            {
                this.effect = effect;
                this.value = value;
                this.remain = remain;
            }

            /// <summary>
            /// これはバフ効果か？
            /// </summary>
            /// <returns></returns>
            public bool IsBuff()
            {
                var b = (BattleConst.Effect)(Mathf.FloorToInt((int)this.effect / 100) * 100);
                return b == BattleConst.Effect.Buff_Base;
            }
            /// <summary>
            /// これはデバフ効果か？
            /// </summary>
            /// <returns></returns>
            public bool IsDebuff()
            {
                var b = (BattleConst.Effect)(Mathf.FloorToInt((int)this.effect / 100) * 100);
                return b == BattleConst.Effect.Debuff_Base;
            }
        }

        /// <summary>
        /// 参照情報
        /// </summary>
        public class Current
        {
            public int HP;
            public int MaxHP;
            public int Atk;
            public int Def;
            public int MAtk;
            public int MDef;
            public int Wait;
            public bool IsDead { get { return HP <= 0; } }

            public Current(Character data, int level)
            {
                this.MaxHP = this.HP = data.Base.HP + level * data.Grow.HP;
                this.Atk = data.Base.Atk + level * data.Grow.Atk;
                this.Def = data.Base.Def + level * data.Grow.Def;
                this.MAtk = data.Base.MAtk + level * data.Grow.MAtk;
                this.MDef = data.Base.MDef + level * data.Grow.MDef;
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
        public Dictionary<string, int> cooltimes = new Dictionary<string, int>();
        public int Position { get; private set; }

        List<Condition> conditions = new List<Condition>();
        UserUnit data;
        UnitState state;

        public BattleUnit(string uniq, UserUnit data, UserDeck deck, BattleConst.Side side)
        {
            this.uniq = uniq;
            this.data = data;
            this.Position = deck.position;
            this.characterData = MasterData.characters.First(v => v.ID == data.characterId);
            this.current = new Current(this.characterData, data.Level());
            this.ais = MasterData.personalities.First(v => v.Name == this.characterData.Personality).AI;

            // スキルID -> スキルデータ
            foreach (var skill in data.skill)
            {
                this.skills.Add(MasterData.FindSkillByID(skill));
                this.cooltimes[skill] = -1;
            }

            root = new GameObject(this.uniq);
            this.side = side;

            // アクター
            var actorPrefab = LoaderService.Instance.Load<GameObject>("Prefab/Actor.prefab");
            actor = GameObject.Instantiate(actorPrefab).GetComponent<Actor>();
            actor.transform.SetParent(root.transform);

            // ステート
            var statePrefab = LoaderService.Instance.Load<GameObject>("Prefab/UnitState.prefab");
            state = GameObject.Instantiate(statePrefab).GetComponent<UnitState>();
            state.transform.SetParent(root.transform);

            state.Setup(this);
            var sprites = LoaderService.Instance.Load<Sprites>($"Character/{this.characterData.Image}/walk.asset");
            actor.sprites = sprites;

            if(side == BattleConst.Side.Enemy)
            {
                root.transform.localScale = new Vector3(-1, 1, 1);
                state.transform.localScale = new Vector3(-1, 1, 1);
            }

            state.UpdateCooltime(this);
        }

        /// <summary>
        /// 後始末
        /// </summary>
        public void Destory()
        {
            GameObject.Destroy(root);
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

        /// <summary>
        /// ダメージを与える
        /// </summary>
        /// <param name="value"></param>
        public void Damage(int value)
        {
            current.HP = Mathf.Max(0, current.HP - value);
            state.SetHP(current.HP);
            state.PlayDamage(value);
        }


        /// <summary>
        /// 回復
        /// </summary>
        /// <param name="value"></param>
        public void Recovery(int value)
        {
            current.HP = Mathf.Min(current.MaxHP, current.HP + value);
            state.SetHP(current.HP);
            state.PlayRecovery(value);
        }

        /// <summary>
        /// スキル使用した。クールタイム登録する
        /// </summary>
        /// <param name="skill"></param>
        public void UseSkill(Skill skill)
        {
            if (skill == null) return;
            cooltimes[skill.ID] = skill.CoolTime;
        }

        /// <summary>
        /// 指定したスキルが使用できるかどうかを確認する
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public bool CanUseSkill(Skill skill)
        {
            if (skill.Passive) return false;

            int cooltime;
            if(cooltimes.TryGetValue(skill.ID, out cooltime))
            {
                return cooltime < 0;
            }
            return false;
        }

        /// <summary>
        /// 行動前に呼び出される
        /// </summary>
        public void PreAction()
        {
            var keys = cooltimes.Keys.ToArray();
            // クールタイム減らす
            foreach (var key in keys)
            {
                --cooltimes[key];
            }

            // 効果の持続回数過ぎたものを削除する
            foreach (var v in conditions) { --v.remain; }
            conditions = conditions.Where(v => v.remain >= 0).ToList();

            // 更新
            state.UpdateCondition(this);
            // クールタイム更新
            state.UpdateCooltime(this);
        }

        /// <summary>
        /// 行動後に呼び出される
        /// </summary>
        public void PostAction()
        {
            // Wait加算
            float buff = GetCondition(BattleConst.Effect.Buff_Wait);
            float debuff = GetCondition(BattleConst.Effect.Debuff_Wait);
            var ratio = Mathf.Max(0, 1 + ((buff - debuff) / 100f));
            current.Wait = Mathf.FloorToInt(characterData.Wait * ratio);
            // クールタイム更新
            state.UpdateCooltime(this);
        }

        /// <summary>
        /// 効果を追加します
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="value"></param>
        public void AddCondition(BattleConst.Effect effect, int value, int remain)
        {
            Debug.Log($"{uniq}: AddCondition({effect}, {value})");
            conditions.Add(new Condition(effect, value, remain));
            state.UpdateCondition(this);
        }

        /// <summary>
        /// 指定した効果の合計効果値を取得
        /// </summary>
        /// <param name="effect"></param>
        public int GetCondition(BattleConst.Effect effect)
        {
            var res = 0;
            foreach(var v in conditions.Where(v => v.effect == effect))
            {
                res += v.value;
            }
            return res;
        }

        /// <summary>
        /// 指定した効果が所持しているか
        /// </summary>
        /// <param name="effect"></param>
        public bool HasCondition(BattleConst.Effect effect)
        {
            foreach (var v in conditions)
            {
                if (v.effect == effect) return true;
            }
            return false;
        }

        /// <summary>
        /// バフの数を取得
        /// </summary>
        /// <returns></returns>
        public int BuffCount()
        {
            return conditions.Count(v => v.IsBuff());
        }
        /// <summary>
        /// デバフの数を取得
        /// </summary>
        /// <returns></returns>
        public int DebuffCount()
        {
            return conditions.Count(v => v.IsDebuff());
        }

        /// <summary>
        /// 指定した効果を消す
        /// </summary>
        public void CancenCondition(BattleConst.Effect effect)
        {
            switch (effect)
            {
                case BattleConst.Effect.BuffCancel_All:
                    {
                        var index = conditions.FindLastIndex(v => v.IsBuff());
                        if (index != -1) conditions.RemoveAt(index);
                    }
                    break;
                case BattleConst.Effect.DebuffCancel_All:
                    {
                        var index = conditions.FindLastIndex(v => v.IsDebuff());
                        if (index != -1) conditions.RemoveAt(index);
                    }
                    break;
                default:
                    {
                        var index = conditions.FindLastIndex(v => v.effect == BattleConst.Cancel2Effect(effect));
                        if (index != -1) conditions.RemoveAt(index);
                    }
                    break;
            }
            state.UpdateCondition(this);
        }
    }
}
