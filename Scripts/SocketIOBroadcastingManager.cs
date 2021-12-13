using SocketIOClient;
using System;
using UnityEngine;

namespace SimpleSocketIOBroadcastingSDK
{
    public class SocketIOBroadcastingManager : MonoBehaviour
    {
        private static SocketIOBroadcastingManager instance;
        public static SocketIOBroadcastingManager Instance
        {
            get
            {
                if (!instance)
                    new GameObject("_SocketIOBroadcastingManager").AddComponent<SocketIOBroadcastingManager>();
                return instance;
            }
        }

        public string serviceAddress = "http://localhost:8212";
        public event Action<SocketIOResponse> onMsg;
        private SocketIO client;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        public async void Connect()
        {
            Disconnect();
            client = new SocketIO(serviceAddress);
            client.On("msg", OnMsg);
            await client.ConnectAsync();
        }

        public async void Disconnect()
        {
            if (client != null && client.Connected)
                await client.DisconnectAsync();
            client = null;
        }

        private void OnMsg(SocketIOResponse response)
        {
            if (onMsg != null)
                onMsg.Invoke(response);
        }

        public async void BroadcastAll(params object[] data)
        {
            await client.EmitAsync("all", data);
        }

        public async void BroadcastOther(object data)
        {
            await client.EmitAsync("other", data);
        }
    }
}
