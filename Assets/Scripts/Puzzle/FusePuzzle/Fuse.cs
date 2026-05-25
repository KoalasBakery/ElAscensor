using UnityEngine;

public class Fuse : MonoBehaviour
{
    public Transform fuseStart, fuseEnd;
    public SpriteRenderer fuseStartSprite=> fuseStart.GetComponent<SpriteRenderer>();
    public SpriteRenderer fuseEndSprite => fuseEnd.GetComponent<SpriteRenderer>();
    public LineRenderer lineRend;

    public void ActiveFuse()
    { 
        fuseStart.gameObject.SetActive(true);
        fuseEnd.gameObject.SetActive(true);
        lineRend.gameObject.SetActive(true);
    }
    public void DisableFuse()
    {
        fuseStart.gameObject.SetActive(false);
        fuseEnd.gameObject.SetActive(false);
        lineRend.gameObject.SetActive(false);
    }
}
