using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;


namespace RevitUndocumentedAPI
{
	public static class RevitUndocumentedExtensions
	{
        public static IList<RibbonItem> AddLabeledStackedItems(this RibbonPanel ribbonPanel, string labelName, RibbonItemData item1, RibbonItemData item2 = null)
        {
			if(labelName == null)
            {
				throw new ArgumentNullException(nameof(labelName));
			}
			if (item1 == null)
			{
				throw new ArgumentNullException(nameof(item1));
			}

			Type rRibbonPanelType = typeof(RibbonPanel);
			MethodInfo canItemBeAddedToRowPanelMethod = 
				rRibbonPanelType.GetMethod("canItemBeAddedToRowPanel", BindingFlags.NonPublic | BindingFlags.Instance);

			if (!(canItemBeAddedToRowPanelMethod.Invoke(ribbonPanel, new object[] { item1 }) as bool?) ?? false)
			{
				string text = " is not supported as a stacked item.";
				string name = item1.Name;
				string text2 = "Item " + name;
				throw new ArgumentException(text2 + text, nameof(item1));
			}

			if (item2 != null && (!(canItemBeAddedToRowPanelMethod.Invoke(ribbonPanel, new object[] { item2 }) as bool?) ?? false))
			{
				string text = " is not supported as a stacked item.";
				string name = item2.Name;
				string text2 = "Item " + name;
				throw new ArgumentException(text2 + text, nameof(item2));
			}

			Autodesk.Windows.RibbonRowPanel ribbonRowPanel = new Autodesk.Windows.RibbonRowPanel();
			Collection<Autodesk.Windows.RibbonItem> items = ribbonRowPanel.Items;

			Autodesk.Windows.RibbonLabel label = new Autodesk.Windows.RibbonLabel() { Text = labelName, Height = 23 };
			items.Add(label);
			Autodesk.Windows.RibbonRowBreak ribbonRowBreak = new Autodesk.Windows.RibbonRowBreak();
			items.Add(ribbonRowBreak);

			List<RibbonItem> list = new List<RibbonItem>();

			MethodInfo addItemToRowPanelMethod =
				rRibbonPanelType.GetMethod("addItemToRowPanel", BindingFlags.NonPublic | BindingFlags.Instance);

			RibbonItem rItem1 = addItemToRowPanelMethod.Invoke(ribbonPanel, new object[] { ribbonRowPanel, item1 }) as RibbonItem;
			list.Add(rItem1);

			if(item2 != null)
            {
				Autodesk.Windows.RibbonRowBreak ribbonRowBreak2 = new Autodesk.Windows.RibbonRowBreak();
				items.Add(ribbonRowBreak2);

				RibbonItem rItem2 = addItemToRowPanelMethod.Invoke(ribbonPanel, new object[] { ribbonRowPanel, item2 }) as RibbonItem;
				list.Add(rItem2);
			}

			FieldInfo aRibbonPanelField = rRibbonPanelType.GetField("m_RibbonPanel", BindingFlags.NonPublic | BindingFlags.Instance);
			Autodesk.Windows.RibbonPanel aRibbonPanel = aRibbonPanelField.GetValue(ribbonPanel) as Autodesk.Windows.RibbonPanel;

			aRibbonPanel.Source.Items.Add(ribbonRowPanel);
			return list;
		}
	}
}
