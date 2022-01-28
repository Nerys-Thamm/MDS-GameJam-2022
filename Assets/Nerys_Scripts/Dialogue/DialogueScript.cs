using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Script")]
public class DialogueScript : ScriptableObject
{
    public AudioClip m_blipSound;

    
    public enum PhraseType
    {
        DEFAULT,
        SFX,
        YESNO
    }

    [System.Serializable]
    public struct Phrase
    {
        public string m_message;
        public Sprite m_characterSprite;
        public PhraseType m_type;
        public float m_speed;
        public SFX_Effect m_sfx;
        public AudioClip m_blip;
    }

    public List<Phrase> m_Phrases = new List<Phrase>();

}
