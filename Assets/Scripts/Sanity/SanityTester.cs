using UnityEngine;

public class SanityTester : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SanityManager.Instance.ModifySanity(-10f);

        if (Input.GetKeyDown(KeyCode.E))
            SanityManager.Instance.ModifySanity(10f);
    }
}