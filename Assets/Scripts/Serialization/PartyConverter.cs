using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.Utilities;

public class PartyConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var Party = value as List<Character>;
        
        JArray array = new JArray();

        Party.ForEach(partyMember =>
        {
            JObject o = JObject.FromObject(partyMember.Serialize());
            array.Add(o);
        });
        
        array.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var Party = new List<Character>();

        var jtoken = JToken.Load(reader);
        var saves = jtoken.ToObject<List<CharacterSave>>();

        saves.ForEach(save =>
        {
            var partyMember = new Character(save);
            Party.Add(partyMember);
        });

        return Party;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<Character>);
    }
}