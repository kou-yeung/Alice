using CsvHelper.Configuration;

namespace Alice.Entities
{
    public class Character
    {
        public string ID;
        public string Name;
    }

    public sealed class CharacterMap : ClassMap<Character>
    {
        public CharacterMap()
        {
            Map(x => x.ID).Name("ID");
            Map(x => x.Name).Name("名前");
        }
    }
}
