using UnityEngine;
using Dolby.Millicast;

public class MultiSource : MonoBehaviour
{
    public string multiSourceName = "camOne";

    private McPublisher publisher;

    private void Start()
    {
        // Get the existing publisher from the McPublisher component
        publisher = GetComponent<McPublisher>();

        // Set the multi-source ID in the publisher options
        publisher.options.multiSourceId = multiSourceName;
    }
}
