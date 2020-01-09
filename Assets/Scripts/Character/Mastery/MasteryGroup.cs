using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class MasteryGroup
{
    public Character Character;

    public Dictionary<Mastery, MasteryInstance> Masteries;

    public MasteryGroup(Character character)
    {
        Masteries = new Dictionary<Mastery, MasteryInstance>();

        character.Base.Masteries.ForEach(mastery =>
        {
            var instance = new MasteryInstance(mastery,this);
            Masteries.Add(mastery, instance);
        });
    }

    public string Serialize()
    {
        var masteryLevels = Masteries.Values.Select(instance => instance.CurrentLevel).ToArray();
        var json = JsonConvert.SerializeObject(masteryLevels, Formatting.None);
        return json;
    }

    public void Deserialize(string json)
    {
        try
        {
            var masteryLevels = JsonConvert.DeserializeObject<int[]>(json);
            var instances = Masteries.Values.ToArray();

            for (int i = 0; i < masteryLevels.Length; i++)
            {
                instances[i].CurrentLevel = masteryLevels[i];
            }
        }
        catch
        {
            throw new DeserializationFailureException(typeof(MasteryGroup));
        }
    }
}