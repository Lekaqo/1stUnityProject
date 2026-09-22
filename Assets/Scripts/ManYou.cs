using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManYou : MonoBehaviour
{
    public CharacterManager characterManager;
    public Animator characterAnimator;

    private void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        characterAnimator = transform.GetComponent<Animator>();
        characterAnimator.enabled = false;
    }


    public void Manyou()
    {
        characterManager.enabled = false;

        characterAnimator.enabled = true;

        characterAnimator.Play(stateName: "Step1");
    }

    public void TuiChuManyou()
    {
        characterManager.enabled = true;

        characterAnimator.enabled = false;
    }


}
