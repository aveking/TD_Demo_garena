using UnityEngine;
using UnityEditor;

using System;
using System.Collections;

using TDTK;

namespace TDTK{
	
	[CustomEditor(typeof(UnitTower))]
	[CanEditMultipleObjects]
	public class I_UnitTowerEditor : TDEditorInspector {

		private static UnitTower instance;
		void Awake(){
			instance = (UnitTower)target;
			LoadDB();
		}
		
		public override void OnInspectorGUI(){
			base.OnInspectorGUI();
			
			if(instance==null) Awake();
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			
		
			PrefabType type=PrefabUtility.GetPrefabType(instance);
			
			if(type==PrefabType.Prefab || type==PrefabType.PrefabInstance){
				
				bool existInDB=false;
				if(type==PrefabType.PrefabInstance) existInDB=TDEditor.ExistInDB((UnitTower)PrefabUtility.GetPrefabParent(instance));
				else if(type==PrefabType.Prefab) existInDB=TDEditor.ExistInDB(instance);
				
				if(!existInDB){
					EditorGUILayout.Space();
					
					EditorGUILayout.HelpBox("This prefab hasn't been added to database hence it won't be accessible to the game.", MessageType.Warning);
					GUI.color=new Color(1f, 0.7f, .2f, 1f);
					if(GUILayout.Button("Add Prefab to Database")){
						UnitTowerEditorWindow.Init();
						UnitTowerEditorWindow.NewItem(instance);
						UnitTowerEditorWindow.Init();		//call again to select the instance in editor window
					}
					GUI.color=Color.white;
				}
				else{
					EditorGUILayout.HelpBox("Editing tower using Inspector is not recommended.\nPlease use the editor window instead.", MessageType.Info);
					if(GUILayout.Button("Tower Editor Window")) UnitTowerEditorWindow.Init(instance.prefabID);
				}
				
				EditorGUILayout.Space();
			}
			else{
				string text="Tower object won't be available for game deployment, or accessible in TDTK editor until it's made a prefab and added to TDTK database.";
				text+="\n\nYou can still edit the tower using default inspector. However it's not recommended";
				EditorGUILayout.HelpBox(text, MessageType.Warning);
				
				EditorGUILayout.Space();
				if(GUILayout.Button("Tower Editor Window")) UnitTowerEditorWindow.Init(instance.prefabID);
			}
			
			
			DefaultInspector();
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
		}
		
	}

}