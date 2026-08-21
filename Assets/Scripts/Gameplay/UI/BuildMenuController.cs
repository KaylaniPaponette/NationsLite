using UnityEngine;

[UIDocument(uiDocName:"BuildMenu", stylesheetName:"BuildMenuStyles")]
public class BuildMenuController : MenuController
{
    public UIContainer itemsContainer;

    public override void Init()
    {
        itemsContainer = view.Find<UIContainer>(nameof(itemsContainer));
    }

    public override void OnOpen()
    {
        base.OnOpen();
        PopulateItems();
    }

    public void PopulateItems()
    {
        ClearChildren();

        var profiles = AttractionManager.instance.profiles;
        if (profiles == null)
            return;

        foreach (var profile in profiles)
        {
            // Only show attractions that are not yet placed
            if (AttractionManager.instance.IsAttractionPlaced(profile))
                continue;

            var itemCard = CreateChild<BuildMenuItemController>(itemsContainer);
            itemCard.Setup(profile);
        }
    }
}