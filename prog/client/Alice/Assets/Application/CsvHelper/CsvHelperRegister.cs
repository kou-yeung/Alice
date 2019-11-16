using UnityEngine;
using Alice;
using Alice.Entities;

namespace CsvHelper
{
    public class CsvHelperRegister
    {
        public static Configuration.Configuration configuration { get; private set; }

        static CsvHelperRegister()
        {
            configuration = new Configuration.Configuration();
            configuration.HeaderValidated = null;
            AddConverter();
            RegisterClassMap();
        }

        /// <summary>
        /// 型変換の登録
        /// </summary>
        static void AddConverter()
        {
            //configuration.TypeConverterCache.AddConverter<Identify>(new IdentifyTypeConverter());
            //configuration.TypeConverterCache.AddConverter<Vector2Int>(new Vector2IntTypeConverter());
            configuration.TypeConverterCache.AddConverter<BattleConst.Effect>(new EnumTypeConverter<BattleConst.Effect>());
            configuration.TypeConverterCache.AddConverter<BattleConst.Target>(new EnumTypeConverter<BattleConst.Target>());
            configuration.TypeConverterCache.AddConverter<Const.Platform>(new EnumTypeConverter<Const.Platform>());
        }

        /// <summary>
        /// クラスマップの登録
        /// </summary>
        static void RegisterClassMap()
        {
            configuration.RegisterClassMap<CharacterMap>();
            configuration.RegisterClassMap<SkillMap>();
            configuration.RegisterClassMap<EffectMap>();
            configuration.RegisterClassMap<PersonalityMap>();
            configuration.RegisterClassMap<ProductMap>();
        }
    }
}
