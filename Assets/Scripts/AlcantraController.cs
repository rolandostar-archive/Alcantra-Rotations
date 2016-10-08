using UnityEngine;
using System.Collections;
using Uniduino;

public class AlcantraController : MonoBehaviour {
	public bool ConnectToArduino = false; // Para Debug

	public Arduino arduino;
	public Material BackgroundMat; // Material a cambiar color
	public GameObject HollowHex; // Objeto a rotar
	public GameObject Music; // Musica
	public GameObject Loading;
	public int pin = 0; // Pin Para leer Pot1
	public float spinSpeed; // Velocidad de Rotacion
	public float vol = 1.0f; // Volumen de la cancion
	public float mag = 2f; // Magnitud
	public float minDarkness = 0.2f; // Variacion de color
	public static float flipLength =1f; // Duracion de transicion

	private int pinValue;
	private static Color initialColor=Color.red;

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

	void Update() {
		if(ConnectToArduino){
			pinValue = arduino.analogRead(pin);
			HollowHex.transform.rotation = Quaternion.Euler(-90,pinValue*spinSpeed,0);
		}
	}

	void ConfigurePins() {
		arduino.pinMode(pin, PinMode.ANALOG);
		arduino.reportAnalog(pin, 1);
		startGame();
	}

	void startGame(){
		this.StartCoroutine (Cycler ());
		Loading.SetActive(false);
		Music.SetActive(true);
	}

	private static Color NextColor() {
		int choice =Random.Range(0,5);
		Color col = Color.black;
		switch(choice){
			case(0):
			col = Color.blue;
			break;
			case(1):
			col = Color.red;
			break;
			case(2):
			col = Color.green;
			break;
			case(3):
			col = Color.white;
			break;
			case(4):
			col = Color.magenta;
			break;
		}
		if(col==initialColor)
		return NextColor();
		else 
		return col;
	}

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

	void OnApplicationQuit() {
	//        Debug.Log("Application ending after " + Time.time + " seconds");
		BackgroundMat.SetColor("_Color", Color.red);
	}
}
