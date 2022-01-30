using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimatorHelper : MonoBehaviour
{
    PlayerMotor m_PlayerMotor;
    MonsterMode m_MosnterMode;

    [SerializeField] List<AudioClip> m_MonsterSteps;
    [SerializeField] List<AudioClip> m_ChildSteps;
    [SerializeField] AudioClip m_AttackSound;
    [SerializeField] AudioClip m_PopOutOfGround;


    [SerializeField] AudioSource m_AudioSource;


    // Start is called before the first frame update
    void Start()
    {
        m_PlayerMotor = GetComponentInParent<PlayerMotor>();
        m_MosnterMode = GetComponentInParent<MonsterMode>();
        m_AudioSource = GetComponent<AudioSource>();

    }

    public void EndGame()
    {
        GameObject.FindGameObjectWithTag("ScreenFade").GetComponent<Animator>().SetTrigger("EndGame");
    }

    public void PlayInitalAudio()
    {
        m_AudioSource.PlayOneShot(m_PopOutOfGround);
    }
    
    public void NextLevel()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void PlayChildStep()
    {
        int i = Random.Range(0, m_ChildSteps.Count);
        m_AudioSource.PlayOneShot(m_ChildSteps[i]);
    }
    public void PlayMonsterStep()
    {
        int i = Random.Range(0, m_MonsterSteps.Count);
        m_AudioSource.PlayOneShot(m_MonsterSteps[i]);
    }
    
    public void Attack()
    {
        m_AudioSource.PlayOneShot(m_AttackSound);
        m_MosnterMode.CalculateAttack();
    }
    public void ToggleMovementLock()
    {
        m_PlayerMotor.ToggleMovementLock();
    }
}
