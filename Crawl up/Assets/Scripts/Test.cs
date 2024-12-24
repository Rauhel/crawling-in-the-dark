using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.Instance.Publish(EventCenter.EVENT_LEARNED_BASIC_CRAWL);
        EventCenter.Instance.Publish(EventCenter.EVENT_LEARNED_GECKO_CRAWL);
        EventCenter.Instance.Publish(EventCenter.EVENT_LEARNED_TURTLE_CRAWL);
        EventCenter.Instance.Publish(EventCenter.EVENT_LEARNED_SNAKE_CRAWL);
        EventCenter.Instance.Publish(EventCenter.EVENT_LEARNED_CAT_CRAWL);
        EventCenter.Instance.Publish(EventCenter.EVENT_LEARNED_CHAMELEON_CRAWL);
    }

    // Update is called once per frame
    void Update()
    {

    }
}