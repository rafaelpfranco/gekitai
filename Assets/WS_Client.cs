using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
public class WS_Client : MonoBehaviour
{
    WebSocket ws;
    public GameObject controller;
    public Queue queue;

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue();

        ws = new WebSocket("ws://localhost:8080");
        var self = this;
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            // Guarda a mensagem recebida no servido em uma fila
            self.queue.Enqueue(e.Data);                       
        };
    }
    
    // Envia string do cliente para o servidor
    public void SendServer(string msg)
    {
        if(ws != null)
        {
            ws.Send(msg);
        }
    }

    // Processa a mensagem e dependendo do type envia para um método diferente no controller
    public void MsgProcess(string type, string payload)
    {
        if(type == "Cor")
        {
            controller.transform.GetComponent<controller>().SetColorPlayer(payload);
        }
        if(type == "Chat")
        {
            controller.transform.GetComponent<controller>().chat.text += payload;
        }
        if(type == "Move")
        {
            controller.transform.GetComponent<controller>().NewPiece(payload);
        }
        if(type == "Surrender")
        {
            controller.transform.GetComponent<controller>().Surrender(payload);
        }
        if(type == "PlayAgain")
        {
            controller.transform.GetComponent<controller>().PlayAgain();
        }
        if(type == "Ready")
        {
            controller.transform.GetComponent<controller>().StartGame(payload);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Se a fila estiver disponível ele ajusta a mensagem
        if(queue.Count > 0)
        {
            var msg = queue.Dequeue().ToString().Split(new[] {':'}, 2);
            string type = msg[0];
            string payload = msg[1];
            MsgProcess(type, payload);
        }
    }
}
