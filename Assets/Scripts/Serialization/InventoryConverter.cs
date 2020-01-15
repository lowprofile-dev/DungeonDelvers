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
        int x = items.Count;

        //while (reader.Read())
        //{
        //    var token = reader.Value;
        //    //var itemSaveObject = (JObject)token;

        //    //var typeToken = itemSaveObject["SaveType"];
        //    //var type = typeToken.Value<string>();

        //    //switch (type)
        //    //{
        //    //    case "Equippable":
        //    //        {
        //    //            var save = itemSaveObject["ItemSave"].Value<EquippableSave>();
        //    //            var item = new Equippable(save);
        //    //            Inventory.Add(item);
        //    //            break;
        //    //        }
        //    //    case "Consumable":
        //    //        {
        //    //            var save = itemSaveObject["ItemSave"].Value<ConsumableSave>();
        //    //            var item = new Consumable(save);
        //    //            Inventory.Add(item);
        //    //            break;
        //    //        }
        //    //    case "MiscItem":
        //    //        {
        //    //            var save = itemSaveObject["ItemSave"].Value<MiscItemSave>();
        //    //            var item = new MiscItem(save);
        //    //            Inventory.Add(item);
        //    //            break;
        //    //        }
        //    //    default:
        //    //        {
        //    //            Debug.LogError(type);
        //    //            break;
        //    //        }
        //    //}
        //}
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

