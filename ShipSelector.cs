using UnityEngine;
using System.Collections;

public class ShipSelector : MonoBehaviour
{
	GUIContent[] raceList;
	GUIContent[] impShips;
	GUIContent[] chaosShips;
	private Popup p1Race;
	private GUIStyle listStyle = new GUIStyle();
	
	private void Start()
	{

		listStyle.normal.textColor = Color.white; 
		listStyle.onHover.background =
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.padding.left =
		listStyle.padding.right =
		listStyle.padding.top =
		listStyle.padding.bottom = 4;

		raceList = new GUIContent[2];
		raceList[0] = new GUIContent("Imperial");
		raceList[1] = new GUIContent("Chaos");

		impShips = new GUIContent[3];
		impShips[0] = new GUIContent("Gothic Class Cruiser");
		impShips[1] = new GUIContent("Tyrant Class Cruiser");
		impShips[2] = new GUIContent("Dauntles Class Light Cruiser");

		chaosShips = new GUIContent[3];
		chaosShips[0] = new GUIContent("Carnage Class Cruiser");
		chaosShips[1] = new GUIContent("Murder Class Cruiser");
		chaosShips[2] = new GUIContent("Slaughter Class Cruiser");
		
		p1Race = new Popup(new Rect(50, 100, 100, 20), raceList[0], raceList, "button", "box", listStyle);
	}
	
	private void OnGUI () 
	{
		p1Race.Show();
	}
}
