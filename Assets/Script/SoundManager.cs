using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip runSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // [เพิ่มใหม่] ค้นหา Audio Source ทั้งหมดในตัวมันเอง แล้วจับแยกหน้าที่ให้อัตโนมัติ!
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length >= 2)
            {
                sfxSource = sources[0]; // ให้ตัวแรกเล่นเสียงสั้น
                loopSource = sources[1]; // ให้ตัวที่สองเล่นเสียงวิ่งวนลูป
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayJump()
    {
        if (jumpSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(jumpSound);
        }
    }

    public void PlayDash()
    {
        if (dashSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(dashSound);
        }
    }

    public void PlayRun()
    {
        if (runSound != null && loopSource != null)
        {
            if (!loopSource.isPlaying || loopSource.clip != runSound)
            {
                loopSource.clip = runSound;
                loopSource.loop = true;
                loopSource.Play();
            }
        }
    }

    public void StopRun()
    {
        if (loopSource != null && loopSource.clip == runSound && loopSource.isPlaying)
        {
            loopSource.Stop();
        }
    }
}