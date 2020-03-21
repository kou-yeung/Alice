using CsvHelper.Configuration;
using System;

namespace Alice.Entities
{
    /// <summary>
    /// 課金アイテム
    /// </summary>
    [Serializable]
    public class Product
    {
        public string ID;
        public Const.Platform Platform;     // OS
        public int Alarm;                   // 獲得できるアラーム数
        public int Bonus;                   // おまけ
        public bool AdminOnly;              // 管理者専用
    }

    public sealed class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Map(x => x.ID).Name("ID");
            Map(x => x.Platform).Name("Platform");
            Map(x => x.Alarm).Name("増加量");
            Map(x => x.Bonus).Name("おまけ");
            Map(x => x.AdminOnly).Name("管理者専用");
        }
    }
}