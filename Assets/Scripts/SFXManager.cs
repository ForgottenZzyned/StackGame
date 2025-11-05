using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    public AudioSource sfxSourcePrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        AudioSource source = Instantiate(sfxSourcePrefab, transform);
        source.clip = clip;
        source.volume = sfxVolume;
        source.pitch = pitch;
        source.Play();
        Destroy(source.gameObject, (clip.length / pitch) + 0.1f);
    }
}
