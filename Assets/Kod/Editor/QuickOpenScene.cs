using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class QuickOpenScene : EditorWindow {
	
	string sceneKeys = "";
	string lastSceneKeys = " "; // it's different from above to activate the filtering mechanism on first check
	
	int selectedIndex = 0;
	int clickedSceneIndex = -1;
	Vector2 scroll;

	static bool close = false;
	
	Event e;
	static FileInfo[] sceneFiles;
	static List<SplitItem> sceneStrings = new List<SplitItem>();
	
	string[] filteredScenes = new string [0] ;
	
	static char[] pathSplitDelimiters = new char[] {'/','\\','-','_','(',')','.',' '};
	
	
	GUIStyle optionStyle;
	GUIStyle selectedOptionStyle;
	
	

	[MenuItem("File/Quick Open Scene... %.")]
	public static void ShowWindow ()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindowWithRect<QuickOpenScene> (
			new Rect (50, 50, 300, 100), true, "Quick Open Scene", true
			);
		
		close = false;
		sceneFiles = (new DirectoryInfo (Application.dataPath)).GetFiles ("*.unity", SearchOption.AllDirectories);

		SplitSceneStrings ();
		
	}

	void InitStyles ()
	{
		if (optionStyle != null)
			return;
		
		optionStyle = new GUIStyle (GUI.skin.label);
		
		
		optionStyle.hover.textColor = new Color (.1f, .4f, .1f);
		optionStyle.onHover.textColor = new Color (.1f, .4f, .1f);
		
		selectedOptionStyle = new GUIStyle (optionStyle);
		selectedOptionStyle.normal.textColor = new Color (.1f, .7f, .1f);
		selectedOptionStyle.fontStyle = FontStyle.Bold;
		
		//ShowAsDropDown (new Rect (0, 0, 10, 10), new Vector2 (300,400));
	}
	
	void OnGUI ()
	{
		InitStyles ();
		
		e = Event.current;
		
		if (CheckForClose ())
			return;
		
		FilterScenesIfNeeded ();
		
		ReactOnScenesBrowseKeys ();
		
	
		GUILayout.Label ("Scene Filter: ", EditorStyles.boldLabel);
		GUILayout.Label (
			"hint: Will match partial names, and also initial andCamelCase capital letters.", 
			EditorStyles.miniLabel);
		GUI.SetNextControlName ("SceneName");
		
		sceneKeys = EditorGUILayout.TextField (sceneKeys);
		
		
		

		
		var size = DrawSelector ();
		
		EditorGUILayout.Space ();
		GUILayout.Label (
			"Use [Up/Down] keys to navigate the list and [Return] key to load a scene.", 
			EditorStyles.miniLabel);
		
		position = new Rect (position.x, position.y, Mathf.Max (50 + size.x, 400), 120 + size.y); 
		
		
		//if (GUI.GetNameOfFocusedControl () == string.Empty) {
		GUI.FocusControl ("SceneName");
		//}
		
		

	}

	Vector2 DrawSelector ()
	{
		scroll = new Vector2 (0, 0);
		EditorGUILayout.Space ();
		EditorGUILayout.BeginScrollView (scroll);
		
		var i = 0;
		float height = 0;
		float width = 0;
		Vector2 size;
		
		optionStyle.wordWrap = false;
		GUIContent content;
		
		
		foreach (string option in filteredScenes) {
			
			content = new GUIContent (option);
			
			GUIStyle style = selectedIndex == i ? selectedOptionStyle : optionStyle;
			
			bool clicked = GUILayout.Button (
				
				content,
				style
				
				);
			
			if (clicked) {
				clickedSceneIndex = i;
			}
			
			size = style.CalcSize (content);
			height += size.y;
			
			width = Mathf.Max (width, size.x);
			
			i++;
		}
		
		EditorGUILayout.EndScrollView ();
		
		//return new Vector2 (700, i * 14);
		return new Vector2 (width, height);
	}
	
	bool ReactOnScenesBrowseKeys ()
	{
		if (clickedSceneIndex != -1) {
			OpenScene (clickedSceneIndex);
			return true;
		}
		if (e.type == EventType.KeyDown) {
			switch (e.keyCode) {
			case  KeyCode.DownArrow:
			
				if (filteredScenes.Length > (selectedIndex + 1)) {
					
					selectedIndex++;
				} else {
					selectedIndex = 0;
				}

				break;
			case KeyCode.UpArrow:
				
				if (selectedIndex > 0) {
					selectedIndex--;
				} else {
					selectedIndex = filteredScenes.Length - 1;
				}
				

				break;
				
			case KeyCode.Return:
				if (filteredScenes.Length > 0) {
					OpenScene (selectedIndex);
					return true;
				}
				break;
			default:
				break;
			}
		}
		return false;
	}

	void FilterScenesIfNeeded ()
	{
		if (sceneKeys == lastSceneKeys) {
			return;
		}
		
		lastSceneKeys = sceneKeys;
		
		var scenes = new List<string> ();
		
		string sceneKeysClean = sceneKeys;
		foreach (char del in pathSplitDelimiters) {
			sceneKeysClean = sceneKeysClean.Replace (del.ToString(), "");
		}
		string[][] searchTermSets = TermPermutate (sceneKeysClean);
		
		foreach (SplitItem item in sceneStrings) {
			
			
			
			string fullName = item.full;
			
			string [] nameTerms = item.bits;

			if (sceneKeys == string.Empty /* add all if empty */ 
				|| (sceneKeys.Length > 1 && fullName.Contains (sceneKeys))) {

				scenes.Add (fullName);
				
			} else {
				
				foreach (string[] termSet in searchTermSets) {
					
					var termMatch = false;
					var termStop = -1; // order of terms found should be the same as order of terms queried for
					
					foreach (string term in termSet) {
						
						termMatch = false;
						
						for (int i = 0; i < nameTerms.Length; i++) {
						
							if (nameTerms [i].ToLower ().StartsWith (term.ToLower ())) {
							
								if (i > termStop) {
									termMatch = true;
									termStop = i;
									break;
								}
							}
						}
						
						if (!termMatch) {
							break;
						}
					}
					
					if (termMatch) { // each term matched
						scenes.Add (fullName);
						break;
					}
				}
			}
			
			
			
		}
		
		filteredScenes = scenes.ToArray ();
		for (int i = 0; i < scenes.Count; i++) {
			
			filteredScenes [i] = scenes [i].Replace ("/", "::");
		}
		
		
		selectedIndex = 0;
	}

	// This one should be easy; have at it!
	string[][] TermPermutate (string str)
	{
		
		int len = str.Length;
		
		if (len == 0)
			return new string[0][];
		if (len == 1) 
			return new string[][] { new string[] {str} };
		
		
		string [][] _set = new string [ (int)System.Math.Pow (2, len - 1) ][];
		
		
		string head = str [0].ToString ();
		string tail = str.Substring (1);
		
		var _tailSet = TermPermutate (tail);
		
		int tailLen = _tailSet.Length;
		
		for (int i = 0; i < tailLen; i++) {
		
			int _tailSubSetLen = _tailSet [i].Length;
			int i2 = i + tailLen;
			
			_set [i2] = new string[_tailSubSetLen + 1];
			_set [i2] [0] = head;
			_tailSet [i].CopyTo (_set [i2], 1);
			
			_set [i] = _tailSet [i];
			_set [i] [0] = head + _set [i] [0];
			
		}
		
		return _set;
	}
	void printFilterScenesIfNeeded ()
	{
		foreach (string s in filteredScenes) {
			
			Debug.Log (s);
		
		}
	}

	static List<string> ExplodeIntoWords (string str)
	{
		int i = 0;
		List<string> pieces = new List<string> ();
		string piece = "";
		
		foreach (char c in str.ToCharArray()) {
			
			if (pathSplitDelimiters.Contains (c)) {
				
				if (piece != "")
					pieces.Add (piece);
				
				piece = "";
			} else if (char.IsUpper (c) && !char.IsUpper (str [i - 1])) {
				
				if (piece != "")
					pieces.Add (piece);
				
				piece = c.ToString ();
				
			} else {
				piece += c.ToString ();
			}
			i++;
		}
		if (piece != "")
			pieces.Add (piece);
		return pieces;
	}
	
	static void SplitSceneStrings ()
	{
		sceneStrings.Clear ();
		
		SplitItem item;
		
		foreach (FileInfo file in sceneFiles) {
			
			item = new SplitItem ();
			
			item.full = file.FullName.Replace(Application.dataPath,"");
			item.bits = ExplodeIntoWords (item.full).ToArray (); 
			
			sceneStrings.Add (item);
		}
	}
	void OpenScene (int index)
	{
		OpenScene (Application.dataPath + "/" + filteredScenes [index].Replace ("::", "/"));
		
	}
	
	void OpenScene (string path)
	{
		var ok = EditorApplication.SaveCurrentSceneIfUserWantsTo ();
		if (ok) {
			EditorApplication.OpenScene (path);
			close = true;
		}
	}
	
	List<IEnumerator> routines  = new List<IEnumerator>();
	List<int> routineFrames = new List<int>();
	
	void Update ()
	{
		ManageRoutines ();
	}

	void ManageRoutines ()
	{
		IEnumerator r;
		
		for (int i = 0; i < routines.Count; i++) {
			
			r = routines [i];
			routineFrames [i]--;
		
			if (routineFrames [i] <= 0) {
			
				if (!r.MoveNext ()) {
					routines.RemoveAt (i);
					routineFrames.RemoveAt (i);
				} else {
					
					try {
						routineFrames [i] = (int)r.Current;
					} catch {
						routineFrames [i] = 0;
					}
					
				}
			}
		}
	}
	
	void StartRoutine (IEnumerator r)
	{
		routines.Add (r);
		routineFrames.Add(0);
	}
	
	bool closeInitiated = false;
	bool CheckForClose ()
	{
		if (close && !closeInitiated) { // bug workarounds, ridiculous
		
			closeInitiated = true;
			StartRoutine (DilatoryClose ());
		}
		
		
		if (e.type == EventType.keyDown && e.keyCode == KeyCode.Escape) {
			close = true;
		}
		
		return close;
	}
	IEnumerator DilatoryClose ()
	{
		yield return 4;
		Close ();
	}
	void OnLostFocus () {
		close = true;
	}
	class SplitItem {
		public string full;
		public string[] bits;
		public SplitItem (string full, string[] bits)
		{
			this.full = full;
			this.bits = bits;
		}
		public SplitItem ()
		{
		}
	}
}
