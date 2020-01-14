//Usada para remover um item do inventário quando o item é usado no combate
public class RemoveItemEffect : Effect
{
    public ItemBase Item;
    public int Cost = 1;
    
    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        PlayerController.Instance.RemoveItemFromInventory(Item, Cost);
        return new ItemRemovedEffectResult
        {
            Item = Item,
            Quantity = Cost,
            skillInfo = skillInfo
        };
    }

    public class ItemRemovedEffectResult : EffectResult
    {
        public ItemBase Item;
        public int Quantity;
    }
}