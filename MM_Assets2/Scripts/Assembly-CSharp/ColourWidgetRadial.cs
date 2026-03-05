using Motorways.UI;
using UnityEngine;

public class ColourWidgetRadial : MonoBehaviour
{
	public ColourWidget ColourWidget;

	public void OnColourChanged()
	{
		ColourWidget.AfterColourChanged();
	}
}
