using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

public class ConsoleHelper {
	
	[MenuItem("Sci/Clear Console &%l")]
	public static void ClearLog()
	{
	    Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
	    Type type = assembly.GetType("UnityEditorInternal.LogEntries");
	    MethodInfo method = type.GetMethod("Clear");
	    method.Invoke(new object(), null);
		
//		Debug.Log ( "methods");
//
//		foreach (var item in type.GetMethods()) {
//			var ps = new string[item.GetParameters().Length];
//			var i = 0;
//			foreach (var p in item.GetParameters()) {
//				
//				ps[i] = p.Name + " " + p.ParameterType.Name;
//				i++;
//			}
//			Debug.Log ( item.Name + " " + item.ReturnType + " params: [ "  + string.Join (" ",ps) + " ] static: " + item.IsStatic);
//
//		}
		

	}
	
}
