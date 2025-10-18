using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.1f, 0.1f, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            FindFirstObjectByType<UIBehaviour>().OnCollectiblesPicked();
            FindFirstObjectByType<GameBehaviour>().CollectibleCollected();
            gameObject.SetActive(false);
        }
    }
}
