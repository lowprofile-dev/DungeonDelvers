using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class InventoryConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var Inventory = value as List<Item>;

        JArray array = new JArray();

        foreach (var item in Inventory)
        {
            JObject o = JObject.FromObject(new SerializedItem
            {
                SaveType = item.GetType().ToString(),
                ItemSave = item.Serialize()
            });
            array.Add(o);
        }
        array.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var Inventory = new List<Item>();

        var jtoken =JToken.Load(reader);
        var items = jtoken.ToObject<List<SerializedItem>>();

        foreach (var item in items)
        {
            ItemSave parsedSave;
            var saveObject = item.ItemSave as JObject;

            switch (item.SaveType)
            {
                case "Equippable":
                {
                    parsedSave = saveObject.ToObject<EquippableSave>();
                    break;
                }
                case "Consumable":
                {
                    parsedSave = saveObject.ToObject<ConsumableSave>();
                    break;
                }
                case "MiscItem":
                {
                    parsedSave = saveObject.ToObject<MiscItemSave>();
                    break;
                }
                default:
                    throw new DeserializationFailureException(typeof(Item));
            }

            var builtItem = ItemInstanceBuilder.BuildInstance(parsedSave);
            Inventory.Add(builtItem);
        }

        
        return Inventory;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<Item>);
    }

    private class SerializedItem
    {
        public string SaveType = "";
        public object ItemSave;
    }
}

