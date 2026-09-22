using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float MaxHealth;

    public float CriticalHealthRatio = 0.3f;//临界生命值

    public float DeadTime;

    public UnityAction<float, GameObject> onDamaged;
    public UnityAction<float> onHealed;
    public UnityAction onDie;

    public float CurrentHealth { get; set; }
    public bool invincible { get; set; }//无敌开关，不可战胜
    public bool canPickup() => CurrentHealth < MaxHealth;
    public float getRatio() => CurrentHealth / MaxHealth;//Ratio, 比率，系数，求出比值
    public bool isCritical() => getRatio() <= CriticalHealthRatio;//临界生命开关

    bool IsDead;

    public void Start()
    {
        CurrentHealth = MaxHealth;//当前生命值为最大

    }

    public void Heal(float healthAmount)//健康的数量
    {
        float healthBefore = CurrentHealth;
        CurrentHealth += healthAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);//

        // call OnHeal action
        float trueHealthAmount = CurrentHealth - healthBefore;
        if (trueHealthAmount > 0f && onHealed != null)
        {
            onHealed.Invoke(trueHealthAmount);//Invoke提出，援引
        }
    }

    public void TakeDamage(float damage, GameObject damageSource)//伤害计算模块
    {
        //if (invincible)//如果无敌开关启用，不扣血
        //    return;

        float healthBefore = CurrentHealth;
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(value:CurrentHealth, min:0f, max:MaxHealth);//将当前生命值限制在0到最大生命值时间，如果伤害大于当前生命值，则返回0

        // call OnDamage action
        float trueDamageAmount = healthBefore - CurrentHealth;

        if (trueDamageAmount > 0f && onDamaged != null)
        {
            onDamaged.Invoke(trueDamageAmount, damageSource);
        }

        HandleDeath();//处决程序
    }

    public void Kill()
    {
        CurrentHealth = 0f;

        if (onDamaged != null)
        {
            onDamaged.Invoke(MaxHealth, null);
        }

        if (gameObject.tag == "Player")
        {
            Application.Quit();
        }
        else
        {
            HandleDeath();//处决程序
        }
    }

    private void HandleDeath()
    {
        //if (CurrentHealth <= 0f)
        //{

        //    if (gameObject.tag == "Enemy")
        //    {
        //        gameObject.SetActive(false);
        //        IsDead = true;
        //    }
        //    else
        //    {
        //        Destroy(gameObject, DeadTime);
        //    }       
        //}
        if (IsDead)
            return;

        // call OnDie action
        if (CurrentHealth <= 0f)
        {
            if (onDie != null)
            {
                IsDead = true;
                onDie.Invoke();
            }
        }

    }

}
