using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;


[CustomEditor (typeof (EasyFontTextMesh))]
public class EasyFontCustomEditor : Editor {
	
	
	private bool	wasPrefabModified;
	private bool 	isFirstTime = true;
	
	void OnEnable()
	{
		EasyFontTextMesh customFont = target as EasyFontTextMesh;
		
		if (customFont.GUIChanged || isFirstTime)
		{
			//customFont.RefreshMeshEditor();
			RefreshAllSceneText(); //Refresh all test to solve the duplicate command issue (Text is not seeing when duplicating). Comment this line an use line above if you have a lot of text
									// and are sufferig slowdonws in the editor when selecting texts
			isFirstTime = false; //This is a hack because on enable is called a lot of times because of the porpertie font
		}
	}
	
	void OnDisable()
	{
		EasyFontTextMesh customFont = target as EasyFontTextMesh;
		
		if (customFont.GUIChanged) //Hack because of the properties is calling this even if there is no OnDisable
			isFirstTime = true;
	}
	
	public override void OnInspectorGUI()
	{

		EditorGUIUtility.LookLikeInspector();
		DrawDefaultInspector();
		
		EasyFontTextMesh customFont = target as EasyFontTextMesh;
		
		SerializedObject serializedObject = new SerializedObject(customFont);
		
		SerializedProperty serializedText 				= serializedObject.FindProperty("_privateProperties.text");
		SerializedProperty serializedFontType 			= serializedObject.FindProperty("_privateProperties.font");
		SerializedProperty serializedFontFillMaterial 	= serializedObject.FindProperty("_privateProperties.customFillMaterial");
		SerializedProperty serializedFontSize 			= serializedObject.FindProperty("_privateProperties.fontSize");
		SerializedProperty serializedCharacterSize 		= serializedObject.FindProperty("_privateProperties.size");
		SerializedProperty serializedTextAnchor 		= serializedObject.FindProperty("_privateProperties.textAnchor");
		SerializedProperty serializedTextAlignment 		= serializedObject.FindProperty("_privateProperties.textAlignment");
		SerializedProperty serializedLineSpacing 		= serializedObject.FindProperty("_privateProperties.lineSpacing");
		SerializedProperty serializedFontColorTop 		= serializedObject.FindProperty("_privateProperties.fontColorTop");
		SerializedProperty serializedFontColorBottom 	= serializedObject.FindProperty("_privateProperties.fontColorBottom");
		SerializedProperty serializedEnableShadow 		= serializedObject.FindProperty("_privateProperties.enableShadow");
		SerializedProperty serializedShadowColor 		= serializedObject.FindProperty("_privateProperties.shadowColor");
		SerializedProperty serializedShadowDistance 	= serializedObject.FindProperty("_privateProperties.shadowDistance");
		SerializedProperty serializedEnableOutline 		= serializedObject.FindProperty("_privateProperties.enableOutline");
		SerializedProperty serializedOutlineColor 		= serializedObject.FindProperty("_privateProperties.outlineColor");
		SerializedProperty serializedOutlineWidth 		= serializedObject.FindProperty("_privateProperties.outLineWidth");
		SerializedProperty serializedHQOutline	 		= serializedObject.FindProperty("_privateProperties.highQualityOutline");
		
		SerializedProperty[] allSerializedProperties = new SerializedProperty[17]
		{	
			serializedText, serializedFontType, serializedFontFillMaterial , serializedFontSize, serializedCharacterSize,serializedTextAnchor, serializedTextAlignment,
			serializedLineSpacing, serializedFontColorTop, serializedFontColorBottom, serializedEnableShadow, serializedShadowColor, serializedShadowDistance,
			serializedEnableOutline, serializedOutlineColor, serializedOutlineWidth, serializedHQOutline
		};
        
		#region properties
		
		
		//Text
		if(serializedText.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedText.prefabOverride);
		
		EditorGUILayout.LabelField(new GUIContent("Text", "This is the text that is going to be used"));
		EditorGUILayout.BeginVertical("box");
		customFont.Text =  EditorGUILayout.TextArea(customFont.Text);
		//customFont.Text =  EditorGUILayout.TextField("Text", customFont.Text); //Old way of inserting text
		
		EditorGUILayout.EndVertical();
		
		
		
		//Font
		
		if(serializedFontType.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontType.prefabOverride);
		
	
		customFont.FontType = EditorGUILayout.ObjectField(new GUIContent("Font","The desired font type"), customFont.FontType, typeof(Font), false) as Font;
	
		
		
		if (customFont.FontType == null)
		{
			customFont.FontType = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font; 
		}
		
		//Font material
		if(serializedFontFillMaterial.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontFillMaterial.prefabOverride);
		
		customFont.CustomFillMaterial = EditorGUILayout.ObjectField(new GUIContent("Custom Fill material", "Use a material different form the one deffined by the font"), customFont.CustomFillMaterial, typeof(Material), false) as Material;
		
		if (customFont.FontType == null)
		{
			customFont.FontType = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font; 
		}
		
		//Font Size
		if(serializedFontSize.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontSize.prefabOverride);
			
		customFont.FontSize = EditorGUILayout.IntField(new GUIContent("Font size", "This is the actual font size. It will set the texture size"), customFont.FontSize);
		
		//CharacterSize
		if(serializedCharacterSize.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedCharacterSize.prefabOverride);
		
		customFont.Size = EditorGUILayout.FloatField(new GUIContent("Character size", "How big the characters are going to be renderer"), customFont.Size); 
		
		
		//Text acnhor
		if(serializedTextAnchor.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedTextAnchor.prefabOverride);
		
		customFont.Textanchor = (EasyFontTextMesh.TEXT_ANCHOR)EditorGUILayout.EnumPopup(new GUIContent("Text Anchor", "Position of the texts pivot's point"), customFont.Textanchor);
		
		//Text alignment
		if(serializedTextAlignment.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedTextAlignment.prefabOverride);

		customFont.Textalignment = (EasyFontTextMesh.TEXT_ALIGNMENT)EditorGUILayout.EnumPopup(new GUIContent("Text alignment", "Line alignment"), customFont.Textalignment);
		
		//Line spacing
		if(serializedLineSpacing .isInstantiatedPrefab)
			SetBoldDefaultFont(serializedLineSpacing.prefabOverride);
		
		customFont.LineSpacing = EditorGUILayout.FloatField(new GUIContent("Line spacing", "Distance between lines"), customFont.LineSpacing); 
		
		// Font color
		if(serializedFontColorTop.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontColorTop.prefabOverride);
		
		customFont.FontColorTop = EditorGUILayout.ColorField (new GUIContent("Top Color", "Color for the top"), customFont.FontColorTop);
		
		if(serializedFontColorBottom.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontColorBottom.prefabOverride);
		
		customFont.FontColorBottom = EditorGUILayout.ColorField (new GUIContent("Bottom Color", "Color for the bottom"), customFont.FontColorBottom);
		
		
		// Shadow
		if(serializedEnableShadow.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedEnableShadow.prefabOverride);
		
			customFont.EnableShadow =  EditorGUILayout.Toggle(new GUIContent("Enable Shadow", "Enable/Disable shadow"), customFont.EnableShadow);
		
		if (customFont.EnableShadow) //Only show the options when enabled
		{
			EditorGUILayout.BeginVertical("box");
			
			if(serializedShadowColor.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedShadowColor.prefabOverride);
			
			customFont.ShadowColor 		= EditorGUILayout.ColorField(new GUIContent("Shadow color", "Sets the sahdow's color"), customFont.ShadowColor);
			
			if(serializedShadowDistance.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedShadowDistance.prefabOverride);
			
			customFont.ShadowDistance 	= EditorGUILayout.Vector3Field("Shadow distance", customFont.ShadowDistance);
			
			EditorGUILayout.EndVertical();
		}
		
		
		//Outline
		if(serializedEnableOutline.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedEnableOutline.prefabOverride);

		 customFont.EnableOutline = EditorGUILayout.Toggle(new GUIContent("Enable Outline", "Enable/Disable the text's outline"), customFont.EnableOutline);
		
		if (customFont.EnableOutline) //Only show the options when enabled
		{
			EditorGUILayout.BeginVertical("box");
			
			if(serializedOutlineColor.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedOutlineColor.prefabOverride);
			
			customFont.OutlineColor = EditorGUILayout.ColorField(new GUIContent("Outline color", "Sets the ouline color"), customFont.OutlineColor);
			
			if(serializedOutlineWidth.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedOutlineWidth.prefabOverride);
			
			customFont.OutLineWidth = EditorGUILayout.FloatField(new GUIContent("Outline width", "Sets the outline width"), customFont.OutLineWidth);
			
			if(serializedHQOutline.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedHQOutline.prefabOverride);
			
			customFont.HighQualityOutline = EditorGUILayout.Toggle(new GUIContent("High Quality", "Increase the number of vertex but gives better results"), customFont.HighQualityOutline);
			
			EditorGUILayout.EndVertical();
		}
		
		#endregion
		
		#region buttons and info
		
		if (GUILayout.Button("Refresh"))
		{
			Debug.Log("Refreshing Text mesh");
			customFont.RefreshMeshEditor();
			
		} 
		
		if (GUILayout.Button("Refresh all"))
		{
			RefreshAllSceneText();
			//OnPlayModeChanged();
		}
		
		GUIStyle buttonStyleRed = new GUIStyle("button");
		buttonStyleRed.normal.textColor = Color.red;
		
		if (GUILayout.Button("Destroy Text component",buttonStyleRed))
		{
			Renderer tempRenderer = customFont.gameObject.renderer;
			MeshFilter	tempMeshFilter = customFont.GetComponent<MeshFilter>();
			DestroyImmediate(customFont);
			DestroyImmediate(tempRenderer);
			DestroyImmediate(tempMeshFilter.sharedMesh);
			DestroyImmediate(tempMeshFilter);
			return;
		}
		
		GUIStyle greenText = new GUIStyle();
		greenText.normal.textColor = Color.green;
		EditorGUILayout.LabelField (string.Format("Vertex count {0}", customFont.GetVertexCount().ToString()),greenText);
		EditorGUILayout.LabelField (string.Format("Font Texture Size {0} x {1}", customFont.renderer.sharedMaterial.mainTexture.width.ToString(),customFont.renderer.sharedMaterial.mainTexture.height.ToString()),greenText);
		
		
		#endregion
		
		#region prefab checks
		//Check if the prefab has changed to refresh the text
		bool checkCurrentPrefabModification = false;
		
		PropertyModification[] modifiedProperties = PrefabUtility.GetPropertyModifications((Object)customFont);
		if (modifiedProperties != null && modifiedProperties.Length > 0)
		{
			for (int i = 0; i<modifiedProperties.Length; i++)
			{
				foreach (SerializedProperty serializerPropertyIterator in allSerializedProperties)
				{
					if (serializerPropertyIterator.propertyPath == modifiedProperties[i].propertyPath)
					{
						wasPrefabModified = true;
					checkCurrentPrefabModification = true;
					}
				}
			}
			
		}
		else
		{
			checkCurrentPrefabModification = false;			
		}
		
		if (wasPrefabModified && !checkCurrentPrefabModification)
		{
			RefreshAllSceneText();
			wasPrefabModified = false;
		}
		
		//Security check. If the mesh is null a prefab revert has been made
		if (customFont.GetComponent<MeshFilter>().sharedMesh == null)
			customFont.RefreshMeshEditor();
			
		#endregion
		customFont.GUIChanged = GUI.changed;
		if (customFont.GUIChanged)
		{
			customFont.RefreshMeshEditor();
			EditorUtility.SetDirty(customFont);
		}
		
	}

	
	void RefreshAllSceneText()
	{
		Object[] customFonts = Resources.FindObjectsOfTypeAll(typeof(EasyFontTextMesh));
		
		if (customFonts.Length > 0)
		{
			for (int i= 0; i < customFonts.Length; i++)
			{
				if (AssetDatabase.GetAssetPath(customFonts[i]) == "") //Only affect the scene assets
				{
					EasyFontTextMesh tempCustomFont =  (EasyFontTextMesh)customFonts[i];	
					tempCustomFont.RefreshMeshEditor(); 
				}
			}
		}
		//GameObject.Find("Perrete2").GetComponent<CustomTextMesh>().name = "Perrete1";
		
		
	}
	
	private MethodInfo boldFontMethodInfo = null;
 
	private void SetBoldDefaultFont(bool value) {
	    
		boldFontMethodInfo = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
		boldFontMethodInfo.Invoke(null, new[] { value as object });
	}
	
	
	
	
}
