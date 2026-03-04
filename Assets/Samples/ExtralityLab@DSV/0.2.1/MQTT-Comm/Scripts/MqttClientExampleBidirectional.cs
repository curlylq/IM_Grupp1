using System.Collections.Generic;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;

namespace ExtralityLab
{
    public class MqttClientExampleBidirectional : M2MqttUnityClient
    {
        [Header("Topics Config")]
        public string publishTopicName = "myUnityApp/digital";
        public string subscribedTopic = "myUnityApp/message";

        private readonly List<string> eventMessages = new List<string>();

        // Expose connection state for other scripts
        public bool IsConnected => client != null && client.IsConnected;

        protected override void Start()
        {
            Debug.Log($"[MQTT] Start() | broker:{brokerAddress}:{brokerPort} publishTopic:'{publishTopicName}' subscribeTopic:'{subscribedTopic}'");
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            Debug.Log($"[MQTT] Connecting to broker {brokerAddress}:{brokerPort} ...");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            Debug.Log($"[MQTT] Connected! IsConnected={IsConnected}");

            // Optional: comment this out if you don't want a non-true/false message on the digital topic
            // PublishInitialTopic();
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            base.OnConnectionFailed(errorMessage);
            Debug.LogError($"[MQTT] Connection FAILED: {errorMessage}");
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            Debug.LogWarning("[MQTT] Disconnected.");
        }

        protected override void SubscribeTopics()
        {
            Debug.Log($"[MQTT] Subscribing to '{subscribedTopic}'");
            client.Subscribe(new string[] { subscribedTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            Debug.Log($"[MQTT] Unsubscribing from '{subscribedTopic}'");
            client.Unsubscribe(new string[] { subscribedTopic });
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log($"[MQTT] Received topic:'{topic}' msg:'{msg}'");
            StoreMessage(msg);

            if (subscribedTopic == topic)
            {
                Debug.Log($"[MQTT] Topic matches subscribedTopic:'{subscribedTopic}'");
                // Do something with message here if needed
            }
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            Debug.Log("[MQTT] ProcessMessage: " + msg);
        }

        private void OnDestroy()
        {
            Debug.Log("[MQTT] OnDestroy() Disconnecting...");
            Disconnect();
        }

        // Optional initial publish
        public void PublishInitialTopic()
        {
            PublishTopicValue("Connected from Unity...");
        }

        public void PublishTopicValue(string msg)
        {
            if (client == null)
            {
                Debug.LogError("[MQTT] PublishTopicValue failed: client is NULL");
                return;
            }

            if (!client.IsConnected)
            {
                Debug.LogWarning($"[MQTT] PublishTopicValue skipped: not connected. Wanted to publish '{msg}' on '{publishTopicName}'");
                return;
            }

            Debug.Log($"[MQTT] Publishing to '{publishTopicName}' msg:'{msg}'");

            // Use QoS 0 to maximize Arduino compatibility (QoS2 often fails with microcontroller clients)
            client.Publish(
                publishTopicName,
                System.Text.Encoding.UTF8.GetBytes(msg),
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                false
            );
        }

        public void SubscribeToTopic()
        {
            SubscribeTopics();
        }

        public void UnsubscribeFromTopic()
        {
            UnsubscribeTopics();
        }
    }
}

/*using System.Collections.Generic;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;


namespace ExtralityLab
{
    public class MqttClientExampleBidirectional : M2MqttUnityClient
    {
        [Header("Topics Config")]
        public string publishTopicName = "myUnityApp/digital";

        public string subscribedTopic = "myUnityApp/message";

        private List<string> eventMessages = new List<string>();

        protected override void Start()
        {
            // Keep this message below
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            Debug.Log($"MQTT: {publishTopicName} connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            Debug.Log($"MQTT: {publishTopicName} connected!");
            PublishInitialTopic();
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { subscribedTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { subscribedTopic });
        }


        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            StoreMessage(msg);

            if (subscribedTopic == topic)
            {
                // TODO: Decide here on what to do when a message is received
                Debug.Log($"Topic matches: {topic}! Do something with message: {message} ");
            }
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            Debug.Log("Received: " + msg);
        }

        private void OnDestroy()
        {
            Disconnect();
        }
        

        ////// CALLBACKS from Buttons
        public void PublishInitialTopic()
        {
            client.Publish(publishTopicName,
                            System.Text.Encoding.UTF8.GetBytes("Connected from Unity..."),
                            MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                            false);

            Debug.Log($"Topic {publishTopicName} published");
        }

        public void PublishTopicValue(string msg)
        {
            client.Publish(publishTopicName,
                            System.Text.Encoding.UTF8.GetBytes(msg),
                            MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                            false);
        }

        public void SubscribeToTopic()
        {
            SubscribeTopics();
        }

        public void UnsubscribeFromTopic()
        {
            UnsubscribeTopics();
        } 
        
    }
}*/