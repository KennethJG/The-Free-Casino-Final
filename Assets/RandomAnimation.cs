using UnityEngine;

public class RandomSlotSpin : MonoBehaviour
{
    public string[] animationNames = { "Take 001", "Take 002", "Take 003","Take 004","Take005","Take006", "Take007" };
    private Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
        PlayRandomAnimation();
    }

    public void PlayRandomAnimation()
    {
        if (animationNames.Length == 0) return;

        int index = Random.Range(0, animationNames.Length);
        string selected = animationNames[index];

        if (anim != null && anim.GetClip(selected) != null)
        {
            anim.Play(selected);
        }
        else
        {
            Debug.LogWarning($"Animation '{selected}' not found!");
        }
    }
}
