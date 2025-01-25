using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource[] destroyNoise;

    private void Start()
    {
        instance = this;
    }

    public void playRandomDestroyNoises()
    {
        var number = Random.Range(0, destroyNoise.Length);
        destroyNoise[number].Play();
    }
}