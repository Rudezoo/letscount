using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour //��ü���� ���� ��Ʈ��
{

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip mainMusic;
    [SerializeField] AudioClip inGameMusic;
    [SerializeField] AudioClip menuClicksound;
    [SerializeField] AudioClip menuClicksound2;
    [SerializeField] AudioClip wrongSound;
    [SerializeField] AudioClip winMusic;

    public static SoundManager instance;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.clip = mainMusic; //���۽� ���θ޴� ������ ư��
        audioSource.Play();
    }

    public void playInGameMusic()
    {
        audioSource.Stop();
        audioSource.clip = inGameMusic;
        audioSource.Play();
    }
    
    public void playMenuSound()
    {
        audioSource.PlayOneShot(menuClicksound);
    }
    public void playMenuSound2()
    {
        audioSource.PlayOneShot(menuClicksound2);
    }

    public void playWrongSound()
    {
        audioSource.PlayOneShot(wrongSound);
    }

    public void playWinMusic()
    {
        audioSource.Stop();
        audioSource.clip = winMusic;
        audioSource.Play();
    }
}
