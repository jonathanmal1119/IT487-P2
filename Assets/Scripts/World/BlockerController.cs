using UnityEngine;

public class BlockerController : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject NextObj;
    public GameObject PreviousObj;

    public void Explode()
    {
        Explosion.SetActive(true);
        ShowNextObj();
        Destroy(this.gameObject, 2);
       
    }

    void ShowNextObj()
    {
        PreviousObj.SetActive(false);
        NextObj.SetActive(true);
    }
}
