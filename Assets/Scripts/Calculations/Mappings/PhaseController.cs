using UnityEngine;

public class PhaseController : MonoBehaviour
{
    public static PhaseController Instance { get; private set; }

    public static bool StopCounting { get; set; }
    public static float Phase { get; private set; }

    private void Awake()
    {
        StopCounting = false;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!StopCounting)
            Phase += Time.deltaTime;
    }
}
