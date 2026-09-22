using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Tooltip("显示当前健康状况的图像组件")]
    public Image healthFillImage;

    public Health m_PlayerHealth;

    private void Start()
    {
        CharacterManager playerCharacterController = FindObjectOfType<CharacterManager>();
        DebugUtility.HandleErrorIfNullFindObject<CharacterManager, PlayerHealthBar>(playerCharacterController, this);

        m_PlayerHealth = playerCharacterController.GetComponent<Health>();
        DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerHealthBar>(m_PlayerHealth, this, playerCharacterController.gameObject);
    }

    void Update()
    {
        // update health bar value
        healthFillImage.fillAmount = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
    }
}
