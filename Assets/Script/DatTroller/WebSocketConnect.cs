using UnityEngine;
using WebSocketSharp;

public class WebSocketController: MonoBehaviour {
    private WebSocket ws;
    public CarController carController;

    void Start() {
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += (sender, e) => {
            Debug.Log("Received from server: " + e.Data);
            carController.MoveCar(e.Data);
        };
        ws.Connect();
    }

    void OnDestroy() {
        if (ws != null) {
            ws.Close();
        }
    }
}
