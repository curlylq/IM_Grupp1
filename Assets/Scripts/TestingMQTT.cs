using ExtralityLab;
using UnityEngine;

public class TestingMQTT : MonoBehaviour
{
    public MqttClientExampleBidirectional mqttClient;

    void Start()
    {
        Invoke("DelayMethod", 5);
    }

    void DelayMethod()
    {
        print("Aktiverar");
        mqttClient.PublishTopicValue("true");
    }
}
