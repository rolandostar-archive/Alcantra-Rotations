using UnityEngine;
using System.Collections;
using Uniduino;

public class AlcantraController : MonoBehaviour {
	// Debug Options
	public bool ConnectToArduino = false;

	// References
	public Arduino arduino;
	public Material BackgroundMat; // Material a cambiar color

	public GameObject colorSelector; // Objeto a rotar
	public GameObject trackArrow; // Objeto a rotar
	public GameObject Music; // Musica
	public GameObject Loading;

	// Arduino Pins
	public int pot_1 = 0;
	public int pot_2 = 1;
	public int servo = 9;

	// Configuration Vars
	public float spinSpeed; // Velocidad de Rotacion
	public float vol = 1.0f; // Volumen de la cancion
	public float mag = 2f; // Magnitud
	public float minDarkness = 0.2f; // Variacion de color
	public static float flipLength =1f; // Duracion de transicion

	// Internal Vars
	private float pot_1_Value;
	private float pot_2_Value;
	private static Color initialColor=Color.red;

	// Invoked at game start
	void Start() {
		Application.runInBackground = true;

		if(ConnectToArduino){
			arduino = Arduino.global;
			//arduino.Log = (s) => Debug.Log("Arduino: " +s);
			arduino.Setup(ConfigurePins);
		}else{
			startGame();
		}
		
	}

	// Invoked once Every Frame
	void Update() {
		if(ConnectToArduino){
			// Get Pot values
			pot_1_Value = arduino.analogRead(pot_1);
			pot_2_Value = arduino.analogRead(pot_2);

			// Map to Circles
			pot_1_Value = MapInterval(pot_1_Value,0,1024,0,720); // 2 Circles
			pot_2_Value = MapInterval(pot_2_Value,0,1024,0,360);

			// Apply Rotation to objects
			colorSelector.transform.localRotation = Quaternion.Euler(0,0,pot_1_Value);
			trackArrow.transform.rotation = Quaternion.Euler(0,0,pot_2_Value);
		}else{ // Arduino-less Section
			colorSelector.transform.Rotate (0,0,50*Time.deltaTime);
			trackArrow.transform.Rotate (0,0,100*Time.deltaTime);
		}
	}

	// Arduino Init
	void ConfigurePins() {
		arduino.pinMode(pot_1, PinMode.ANALOG);
		arduino.pinMode(pot_2, PinMode.ANALOG);
		arduino.reportAnalog(pot_1, 1);
		arduino.reportAnalog(pot_2, 1);
		arduino.pinMode(servo, PinMode.SERVO);
		startGame();
	}

	// Starts Game (After connection to Arduino)
	void startGame(){
		this.StartCoroutine (Cycler ());
		Loading.SetActive(false);
		GameObject.Find("Main Camera").GetComponent<CameraSpinner>().enabled = true;
		Music.SetActive(true);
	}

	// Randomly Selects a color
	private static Color NextColor() {
		int choice =Random.Range(0,5);
		Color col = Color.black;
		switch(choice){
			case(0): col = Color.blue; break;
			case(1): col = Color.red; break;
			case(2): col = Color.green; break;
			case(3): col = Color.white; break;
			case(4): col = Color.magenta; break;
		}
		if(col==initialColor) return NextColor();
		else return col;
	}

	// Changes Color
	private IEnumerator Cycler() {
		while(true){
			float waitTime =4f;
			yield return new WaitForSeconds(waitTime);
			Color oldColor = initialColor;
			Color newColor = NextColor();
			for(float i=0f;i<1;i+=Time.deltaTime/flipLength){
				initialColor = BackgroundMat.color = Color.Lerp(oldColor, newColor, i);
				yield return new WaitForSeconds(0f);
			}
			initialColor=newColor;
		}
	}

	// Clamps values
	float MapInterval(float val, float srcMin, float srcMax, float dstMin, float dstMax) {
    	if (val>=srcMax) return dstMax;
    	if (val<=srcMin) return dstMin;
    	return dstMin + (val-srcMin) / (srcMax-srcMin) * (dstMax-dstMin);
 	}

	void OnApplicationQuit() {
	//        Debug.Log("Application ending after " + Time.time + " seconds");
		BackgroundMat.SetColor("_Color", Color.red);
	}
}
