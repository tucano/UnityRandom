using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using URandom;

public class UnityRandomEditorWindow : EditorWindow {
	
	#region Attributes
	
	float[] shufflebag = {1,2,3,4,5,6,7,8,9,10};
	Dictionary<float,int> wshufflebag;
	
	float alpha = 0;
	float beta  = 0;
	bool rotation = false;
	
	int _graph_area_width = 500;
	int _graph_area_height = 500;
	int _area_margin = 10;
	int _editor_area_width = 400;
	int _editor_area_height = 400;
	
	int _range_min = 0;
	int _range_max = Int32.MaxValue;
	
	bool transform = false;
	int samplig_size = 500;
	int max_samplig_size = 10000; // depend on the type
	float temperature = 5.0f;
	
	int seed;
	string stringSeed = Int32.MaxValue.ToString(); 
	
	// FOR CAP AND RING
	float spotAngle = 20.0f;
	float innerAngle = 20.0f;
	float outerAngle = 60.0f;
	ArrayList randomList;
	String filename = "sampling.txt";
	private String path;
	StreamWriter fileWriter = null;
	
	// GENERAL TYPE
	public enum RandomType
	{
		NUMBER = 1,
		VECTOR_2D = 2,
		VECTOR_3D = 3,
		COLOR = 4,
		DICE = 5,
		SHUFFLEBAG = 6,
		WEIGHTEDBAG = 7
	}
	private RandomType _randomType = RandomType.NUMBER;
	
	// TYPE OF RANDOM FLOAT NUMBER
	public enum RandomNumberType
	{
		VALUE = 0,
		RANGE = 1,
		POISSON = 2,
		EXPONENTIAL = 3,
		GAMMA = 4
	}
	private RandomNumberType _randomNumberType = RandomNumberType.VALUE;
	
	// TYPE OF RANDOM VECTOR 2D
	public enum RandomVector2DType
	{
		SQUARE = 0,
		CIRCLE = 1,
		DISK = 2
	}
	private RandomVector2DType _randomVector2DType = RandomVector2DType.SQUARE;
	
	// TYPE OF RANDOM VECTOR 3D
	public enum RandomVector3DType
	{
		INCUBE = 0,
		ONCUBE = 1,
		INSPHERE = 2,
		ONSPHERE = 3,
		ONCAP = 4,
		ONRING = 5
	}
	private RandomVector3DType _randomVector3DType = RandomVector3DType.ONSPHERE;
	
	// THIS IS THE NORMALIZATION FOR UnityRandom Class
	private UnityRandom.Normalization normalization = UnityRandom.Normalization.STDNORMAL;
	
	// THIS IS THE DICE TYPE
	private DiceRoll.DiceType dice = DiceRoll.DiceType.D6;
	private int nroll = 1;
	
	#endregion
	
	
	#region Init,Update,OnGUI
	// Add menu named "Unity Random" to the Window menu
	[MenuItem ("Window/Unity Random")]	
	static void Init () 
	{
		// Get existing open window or if none, make a new one:
		UnityRandomEditorWindow window = (UnityRandomEditorWindow)EditorWindow.GetWindow(typeof (UnityRandomEditorWindow), false, "UNITY RANDOM");
		window.Show();		
	}
	
	// Update
	void Update()
	{
		if (rotation)
		{
			// NAIF ROTATION
			alpha += Time.deltaTime * 500;
			beta += Time.deltaTime * 500;
			this.Repaint();
		}
	}
		
	// OnGUI
	void OnGUI() 
	{
		// SAMPLING SIZE
		switch (_randomType) {
			case RandomType.NUMBER:    max_samplig_size  = 10000; break;
			case RandomType.VECTOR_2D: max_samplig_size  = 5000; break;	
			case RandomType.VECTOR_3D: max_samplig_size  = 1000; break;
			case RandomType.COLOR:     max_samplig_size  = 400;  break;
			case RandomType.DICE:      max_samplig_size  = 5000;  break;	
			default: max_samplig_size = 5000; break;
		}
		// ADJUST CURRENT SIZE
		if (samplig_size > max_samplig_size) samplig_size = max_samplig_size;
		
		// DRAWING AREA
		GUILayout.BeginArea(new Rect(_area_margin,_area_margin,_graph_area_width, _graph_area_height));
		GUILayout.Box("", GUILayout.Width(_graph_area_width), GUILayout.Height(_graph_area_height));
		if (randomList != null && randomList.Count > 0) {
			switch (_randomType) {
			case RandomType.NUMBER:
				switch (_randomNumberType) {
				case RandomNumberType.VALUE:
					UnityRandomEditorDraw.DrawXYPlot(randomList, _graph_area_width, _graph_area_height);	
				break;
				case RandomNumberType.RANGE:
					UnityRandomEditorDraw.DrawXYPlot(randomList, _graph_area_width, _graph_area_height, _range_min, _range_max);
				break;
				default:
					UnityRandomEditorDraw.DrawXYPlot(randomList, _graph_area_width, _graph_area_height, true);
				break;
				}
			break;
			case RandomType.VECTOR_2D:
			UnityRandomEditorDraw.DrawV2Plot(randomList, _graph_area_width, _graph_area_height, _randomVector2DType);
			break;
			case RandomType.VECTOR_3D:
			UnityRandomEditorDraw.DrawV3Plot(randomList, _graph_area_width, _graph_area_height, _randomVector3DType, alpha, beta);
			break;
			case RandomType.COLOR:
			UnityRandomEditorDraw.DrawColorPlot(randomList, _graph_area_width, _graph_area_height);
			break;
			case RandomType.DICE:
			// generate a new ArrayList with the Sum, then send to DrawXYPlot
			ArrayList sums = RandomDiceSums();
			sums.Sort();
			UnityRandomEditorDraw.DrawXYPlot(sums, _graph_area_width, _graph_area_height, true);
			break;
			case RandomType.SHUFFLEBAG:
			UnityRandomEditorDraw.DrawXYPlot(randomList, _graph_area_width, _graph_area_height, shufflebag[0], shufflebag[shufflebag.Length - 1]);
			break;
			case RandomType.WEIGHTEDBAG:
			UnityRandomEditorDraw.DrawXYPlot(randomList, _graph_area_width, _graph_area_height, 1, 10);
			break;
			default:
			// defailt is no drawing
			break;
			}
		}
		GUILayout.EndArea();
		
		// SAMPLE RANDOM BUTTON
		GUILayout.BeginArea(new Rect(_area_margin, _area_margin + _area_margin + _graph_area_height, _graph_area_width, 60));		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Generate Random Numbers",GUILayout.Height(60))) {
			this.SampleRandom();
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		// CONTROL PANEL RIGHT BOX
		GUILayout.BeginArea(new Rect(_area_margin + _area_margin +_graph_area_width, _area_margin, _editor_area_width, _editor_area_height));

		// TITLE
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("CHOOSE TYPE OF RANDOM TYPE: ");
		_randomType = (RandomType) EditorGUILayout.EnumPopup(_randomType, GUILayout.Width(100));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();
		
		switch (_randomType) {
		
		case RandomType.NUMBER:	RandomNumbersGUI();
		break;
			
		case RandomType.VECTOR_2D: RandomVector2DGUI();
		break;
		
		case RandomType.VECTOR_3D: RandomVector3DGUI();
		break;
		
		case RandomType.COLOR: RandomColorGUI();
		break;
		
		case RandomType.DICE: RandomDiceGUI();
		break;
		
		case RandomType.SHUFFLEBAG: ShuffleBagGUI();	
		break;
		
		case RandomType.WEIGHTEDBAG: ShuffleBagGUI();	
		break;
			
		default:
		break;
		}
		
		EditorGUILayout.BeginVertical("box");
		samplig_size = EditorGUILayout.IntSlider ("Sampling Size:", samplig_size, 1, max_samplig_size);
		EditorGUILayout.EndVertical();
		
		if (randomList != null && randomList.Count > 0) {
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("SAVE",GUILayout.Width(100))) this.Save();
			GUILayout.FlexibleSpace();
			filename = EditorGUILayout.TextField(filename);
			EditorGUILayout.EndHorizontal();			
			EditorGUILayout.EndVertical();
		}
		
		
		// 3D VIEWS BUTTONS
		if (randomList != null && randomList.Count > 0 && _randomType == RandomType.VECTOR_3D && 
			(_randomVector3DType == RandomVector3DType.INSPHERE || 
			 _randomVector3DType == RandomVector3DType.ONSPHERE ||
			 _randomVector3DType == RandomVector3DType.ONCAP ||
			_randomVector3DType == RandomVector3DType.ONRING )) {
			EditorGUILayout.BeginVertical("box");
			String rotationLabel = rotation ? "STOP ROTATE VIEW" : "START ROTATE VIEW";
			if (GUILayout.Button(rotationLabel,GUILayout.Height(60))) {
				rotation = !rotation;
			}
			EditorGUILayout.EndVertical();
		}
		GUILayout.EndArea();
		
		// if GUI has changed empty the Array
		if (GUI.changed && randomList != null && randomList.Count != 0) { 
			CleanList();
			this.Repaint();
		}
		
		// if Array is empty stop rotation and reset alpha/beta
		if (randomList == null || randomList.Count == 0) {
			rotation = false;
			alpha = beta = 0;
		}
	}
	
	#endregion
	
	#region PrivateMethods
	private void CleanList()
	{
		// FIXME I NEED TO REMOVE OBJECTS MANUALLY?
		randomList.Clear();
	}
	
	private ArrayList RandomDiceSums()
	{
		ArrayList result = new ArrayList();
		foreach ( object obj in randomList ) 
		{
			DiceRoll _roll = (DiceRoll) obj;
			result.Add( _roll.Sum() );
		}
		return result;
	}
	
	// DICES GUI
	private void RandomDiceGUI()
	{
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TEST RANDOM DICES");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();
		
		SeedBoxGUI();
		
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Dice:", GUILayout.Width(100));
		dice = (DiceRoll.DiceType) EditorGUILayout.EnumPopup(dice, GUILayout.Width(100));
		GUILayout.FlexibleSpace();
		GUILayout.Label( (nroll + dice.ToString()), GUILayout.Width(100));
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("#Rolls:", GUILayout.Width(100));
		nroll = EditorGUILayout.IntSlider(nroll, 0, 10);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();
	}
	
	// BOXSEEDCONFIGURATION
	private void SeedBoxGUI()
	{
		EditorGUILayout.BeginVertical("box");		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("RandomSeed:", GUILayout.Width(100));
		stringSeed = EditorGUILayout.TextField(stringSeed, GUILayout.Width(100));
		ParseSeed();
		GUILayout.FlexibleSpace();
		GUILayout.Label("range: +/-" + Int32.MaxValue);
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();
	}
	
	private void ShuffleBagGUI()
	{
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TEST SHUFFLE BAG");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();		
		SeedBoxGUI();
	}
	
	private void RandomColorGUI()
	{
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TEST RANDOM COLORS");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();		
		SeedBoxGUI();
		
		// BOX TRANSFORMATIONS
		EditorGUILayout.BeginVertical("box");	
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TRANSFORMATIONS");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical("box");
		transform = EditorGUILayout.Toggle("Transform?",transform);
		EditorGUILayout.EndVertical();
		
		if (transform) NormalizationNumberTypes();
	}
	
	private void RandomVector3DGUI()
	{
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TEST RANDOM VECTOR3");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Random Type:", GUILayout.Width(100));
		_randomVector3DType = (RandomVector3DType) EditorGUILayout.EnumPopup(_randomVector3DType, GUILayout.Width(100));
		GUILayout.FlexibleSpace();
		switch (_randomVector3DType) {
		case RandomVector3DType.INCUBE:
		case RandomVector3DType.ONCUBE:
			GUILayout.Label("Vector3: [-1,1] r = 1");
		break;
		case RandomVector3DType.ONSPHERE:
		case RandomVector3DType.INSPHERE:
		case RandomVector3DType.ONCAP:
		case RandomVector3DType.ONRING:
			GUILayout.Label("Vector3: [-1,1] r = 1");
		break;
		default:
		break;
		}
		EditorGUILayout.EndHorizontal();
		
		switch (_randomVector3DType) {
		case RandomVector3DType.ONCAP:
			spotAngle = EditorGUILayout.Slider ("Spot Angle:", spotAngle, 0.0f, 180.0f);
		break;
		case RandomVector3DType.ONRING:
			innerAngle = EditorGUILayout.Slider ("Inner Angle:", innerAngle, 0.0f, 180.0f);
			outerAngle = EditorGUILayout.Slider ("Outer Angle:", outerAngle, 0.0f, 180.0f);
			if (innerAngle > outerAngle) innerAngle = outerAngle;
		break;
		default:
		break;
		}
		
		EditorGUILayout.EndVertical();
		
		SeedBoxGUI();
		
		// BOX TRANSFORMATIONS
		EditorGUILayout.BeginVertical("box");	
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TRANSFORMATIONS");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical("box");
		transform = EditorGUILayout.Toggle("Transform?",transform);
		EditorGUILayout.EndVertical();
		
		if (transform) NormalizationNumberTypes();
	}
	
	private void RandomVector2DGUI()
	{
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TEST RANDOM VECTOR2");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical("box");
		// TYPE
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Random Type:", GUILayout.Width(100));
		_randomVector2DType = (RandomVector2DType) EditorGUILayout.EnumPopup(_randomVector2DType, GUILayout.Width(100));
		GUILayout.FlexibleSpace();
		switch (_randomVector2DType) {
		case RandomVector2DType.SQUARE:
			GUILayout.Label("Vector2: [-1,1]");
		break;		
		case RandomVector2DType.CIRCLE:
			GUILayout.Label("Vector2: [-1,1] r = 1");
		break;		
		case RandomVector2DType.DISK:
			GUILayout.Label("Vector2: [-1,1] r = 1");
		break;			
		default:
		break;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		SeedBoxGUI();
		
		// BOX TRANSFORMATIONS
		EditorGUILayout.BeginVertical("box");	
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TRANSFORMATIONS");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical("box");
		transform = EditorGUILayout.Toggle("Transform?",transform);
		EditorGUILayout.EndVertical();
		
		if (transform) NormalizationNumberTypes();
	}
	
	private void RandomNumbersGUI()
	{
		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TEST RANDOM NUMBERS");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();		
		EditorGUILayout.EndVertical();
		
		
		// BOX RANDOM NUMBER CONFIGURATION
		EditorGUILayout.BeginVertical("box");
		// TYPE
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Random Type:", GUILayout.Width(100));
		_randomNumberType = (RandomNumberType) EditorGUILayout.EnumPopup( _randomNumberType, GUILayout.Width(100));		
		// HELP TYPE
		GUILayout.FlexibleSpace();
		if (_randomNumberType == RandomNumberType.VALUE) {
			GUILayout.Label("Float: 0 < x < 1");
		} else {
			GUILayout.Label("Float: 0 < x < " + Int32.MaxValue);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		// RANGES SLIDERS
		if (_randomNumberType == RandomNumberType.RANGE) 
		{
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Any Int Number in range: 0 - " + Int32.MaxValue + ". Test range: 0 - 100");
			EditorGUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Label("Min:");
			GUILayout.Label("Max:");
			GUILayout.EndVertical();			
			GUILayout.BeginVertical();
			_range_min = EditorGUILayout.IntSlider(_range_min, 0, 100, GUILayout.ExpandWidth(true));
			_range_max = EditorGUILayout.IntSlider(_range_max, 0, 100, GUILayout.ExpandWidth(true));			
			if (_range_min > _range_max) _range_max = _range_min;
			GUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
		
		SeedBoxGUI();
		
		// BOX TRANSFORMATIONS
		switch (_randomNumberType) {
		case RandomNumberType.VALUE:
		case RandomNumberType.RANGE:
			EditorGUILayout.BeginVertical("box");	
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("TRANSFORMATIONS");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical("box");
			transform = EditorGUILayout.Toggle("Transform?",transform);
			EditorGUILayout.EndVertical();
			
			if (transform) NormalizationNumberTypes();
		break;
		
		case RandomNumberType.POISSON:
			EditorGUILayout.BeginVertical("box");
			temperature = EditorGUILayout.Slider ("Lambda", temperature, 0.0f, 100.0f);
			EditorGUILayout.EndVertical();
		break;
		
		case RandomNumberType.EXPONENTIAL:
			EditorGUILayout.BeginVertical("box");
			temperature = EditorGUILayout.Slider ("Lambda", temperature, 0.0f, 10.0f);
			EditorGUILayout.EndVertical();
		break;
		
		case RandomNumberType.GAMMA:
			EditorGUILayout.BeginVertical("box");
			temperature = (float) EditorGUILayout.IntSlider ("Integer Order", (int) temperature, 0, 100);
			EditorGUILayout.EndVertical();
		break;
			
		default:
		break;
		}
	}
	
	private void NormalizationNumberTypes()
	{	
		EditorGUILayout.BeginVertical("box");
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Tranformation Type:");
		normalization = (UnityRandom.Normalization) EditorGUILayout.EnumPopup( normalization, GUILayout.Width(100.0f));
		EditorGUILayout.EndHorizontal();
		
		switch (normalization) {
		
		case UnityRandom.Normalization.STDNORMAL:
			temperature = EditorGUILayout.Slider ("Temperature", temperature, 0.0f, 10.0f);
		break;
		
		case UnityRandom.Normalization.POWERLAW:
			temperature = EditorGUILayout.Slider ("Power", temperature, 0.0f, 100.0f);
		break;

		default:
			 Debug.LogError("Unrecognized Option");
		break;
		}
		EditorGUILayout.EndVertical();
	}
		
	private void SampleRandom()
	{
		Debug.Log("GENERATING RANDOM " + _randomType + " WITH SEED: " + seed);
		
		UnityRandom urand = new UnityRandom(seed);
		if (randomList == null)	randomList = new ArrayList();
		randomList.Clear();
		
		switch (_randomType) 
		{
			case RandomType.NUMBER: SampleNumber(ref urand); break;
			case RandomType.VECTOR_2D: SampleVector2D(ref urand); break;
			case RandomType.VECTOR_3D: SampleVector3D(ref urand); break;
			case RandomType.COLOR: SampleColor(ref urand); break;
			case RandomType.DICE: SampleDice(ref urand); break;	
			case RandomType.SHUFFLEBAG: SampleShuffle(ref urand); break;
			case RandomType.WEIGHTEDBAG: SampleWShuffle(ref urand); break;
			default: SampleNumber(ref urand); break;
		}
		
		this.Repaint();
	}
	
	private void SampleShuffle(ref UnityRandom _urand)
	{
		ShuffleBagCollection<float> thebag = _urand.ShuffleBag(shufflebag);
		for (int m = 0; m < samplig_size; m++) 
		{
			randomList.Add(thebag.Next());
		}
		randomList.Sort();
	}
	
	// Really no time to make it better (FIXME)
	private void SampleWShuffle(ref UnityRandom _urand)
	{
		wshufflebag = new Dictionary<float,int>();
		// fill the wshufflebag
		wshufflebag[1] = 5;
		wshufflebag[2] = 10;
		wshufflebag[3] = 10;
		wshufflebag[4] = 25;
		wshufflebag[5] = 25;
		wshufflebag[6] = 10;
		wshufflebag[7] = 5;
		wshufflebag[8] = 5;
		wshufflebag[9] = 3;
		wshufflebag[10] = 2;
		
		ShuffleBagCollection<float> thebag = _urand.ShuffleBag(wshufflebag);		
		for (int m = 0; m < samplig_size; m++) 
		{
			randomList.Add(thebag.Next());
		}
		randomList.Sort();
	}
	
	private void SampleDice(ref UnityRandom _urand)
	{
		for (int i = 0; i < samplig_size; i++) 
		{
			randomList.Add( _urand.RollDice(nroll, dice));
		}
	}
	
	private void SampleColor(ref UnityRandom _urand)
	{
		for (int i = 0; i < samplig_size; i++) 
		{
			if (transform) {
				randomList.Add(_urand.Rainbow(normalization,temperature));
			} else {
				randomList.Add(_urand.Rainbow());
			}
		}
	}
	
	private void SampleVector3D(ref UnityRandom _urand)
	{
		for (int i = 0; i < samplig_size; i++) 
		{
			switch (_randomVector3DType) {
			case RandomVector3DType.INCUBE:
				if (transform) {
					randomList.Add(_urand.PointInACube(normalization,temperature));
				} else {
					randomList.Add(_urand.PointInACube());
				}
			break;
			case RandomVector3DType.ONCUBE:
				if (transform) {
					randomList.Add(_urand.PointOnACube(normalization,temperature));
				} else {
					randomList.Add(_urand.PointOnACube());
				}
			break;
			case RandomVector3DType.INSPHERE:
				randomList.Add(_urand.PointInASphere());
			break;
			case RandomVector3DType.ONSPHERE:
				randomList.Add(_urand.PointOnASphere());
			break;
			case RandomVector3DType.ONCAP:
				randomList.Add(_urand.PointOnCap(spotAngle));
			break;
			case RandomVector3DType.ONRING:
				randomList.Add(_urand.PointOnRing(innerAngle,outerAngle));
			break;
			default:
			break;
			}
		}
	}
	
	private void SampleVector2D(ref UnityRandom _urand)
	{
		for (int i = 0; i < samplig_size; i++) 
		{
			switch (_randomVector2DType) {
			case RandomVector2DType.SQUARE:
				if (transform) {
					randomList.Add(_urand.PointInASquare(normalization,temperature));
				} else {
					randomList.Add(_urand.PointInASquare());
				}
			break;
			case RandomVector2DType.CIRCLE:
				if (transform) {
					randomList.Add(_urand.PointInACircle(normalization,temperature));
				} else {
					randomList.Add(_urand.PointInACircle());
				}
			break;
			case RandomVector2DType.DISK:
				if (transform) {
					randomList.Add(_urand.PointInADisk(normalization,temperature));					
				} else {
					randomList.Add(_urand.PointInADisk());
				}
			break;
			default:
			break;
			}
		}
	}
	
	private void SampleNumber(ref UnityRandom _urand)
	{
		for (int i = 0; i < samplig_size; i++) 
		{
			switch (_randomNumberType) {
				
			case RandomNumberType.VALUE:
				if (transform) {
					randomList.Add(_urand.Value( normalization, temperature ));
				} else {
				 	randomList.Add(_urand.Value());
				}
			break;
				
			case RandomNumberType.RANGE:
				if (transform) {
					randomList.Add(_urand.Range(_range_min, _range_max, normalization, temperature ));
				} else {
				 	randomList.Add(_urand.Range(_range_min, _range_max));
				}
			break;

				
			case RandomNumberType.POISSON:
				randomList.Add(_urand.Possion(temperature));
			break;
				
			case RandomNumberType.EXPONENTIAL:
				randomList.Add(_urand.Exponential(temperature));
			break;
				
			case RandomNumberType.GAMMA:
				randomList.Add(_urand.Gamma(temperature));
			break;
				
			default:
			break;
			}
		}
		randomList.Sort();
	}
	
	private void Save()
	{
		path = Application.dataPath + "/UnityRandom/Samples/" + filename;
		Debug.Log("SAVING TO FILE: " + path);
		
		fileWriter = File.CreateText(path);
		
		foreach ( object obj in randomList ) 
		{
			// FIXME SAMPLING WRITE LINE FOR Vector3 and Vector2
			switch (_randomType) 
			{
			case RandomType.NUMBER: 
				fileWriter.WriteLine(obj); 
			break;
			case RandomType.VECTOR_2D: 
				Vector2 pos = (Vector2) obj;
				fileWriter.WriteLine(pos.x + "\t" + pos.y); 
			break;
			case RandomType.VECTOR_3D: 
				Vector3 pos3 = (Vector3) obj;
				fileWriter.WriteLine(pos3.x + "\t" + pos3.y + "\t" + pos3.z); 
			break;
			default:
			break;
			}
			
		}
		
		// when you are done writing
		fileWriter.Close();
		
		UnityEngine.Object myfile = AssetDatabase.LoadMainAssetAtPath("Assets/UnityRandom/Samples/" + filename);
		AssetDatabase.OpenAsset(myfile);
	}
	
	private void ParseSeed()
	{
		checked 
		{
			try 
			{
				seed = int.Parse(stringSeed);
			} 
			catch (System.Exception)
			{
				seed = Int32.MaxValue;
				stringSeed = seed.ToString();
				this.Repaint();
			}
		}	
	}
	
	#endregion
	
	#region Events
	// BEWARE of TEXTURE2D MEMORY LEAKS!!!!
	void OnDisable()
	{
		// KILL TEXTURES
		UnityEngine.Object.DestroyImmediate(UnityRandomEditorDraw.tex);
		UnityEngine.Object.DestroyImmediate(UnityRandomEditorDraw.aaLineTex);
		UnityEngine.Object.DestroyImmediate(UnityRandomEditorDraw.lineTex);
	}
	#endregion
}
