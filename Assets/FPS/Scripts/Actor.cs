using UnityEngine;

// This class contains general information describing an actor (player or enemies).此类包含描述演员（玩家或敌人）的一般信息
// It is mostly used for AI detection logic and determining if an actor is friend or foe.它主要用于 AI 检测逻辑并确定演员是朋友还是敌人
public class Actor : MonoBehaviour
{
    [Tooltip("Represents the affiliation (or team) of the actor. Actors of the same affiliation are friendly to eachother代表演员的隶属关系（或团队）。 同一所属的演员彼此友好")]
    public int affiliation;
    [Tooltip("Represents point where other actors will aim when they attack this actor表示其他 Actor 攻击该 Actor 时将瞄准的点")]
    public Transform aimPoint;

    ActorsManager m_ActorsManager;

    private void Start()
    {
        m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();
        DebugUtility.HandleErrorIfNullFindObject<ActorsManager, Actor>(m_ActorsManager, this);

        // Register as an actor
        if (!m_ActorsManager.actors.Contains(this))
        {
            m_ActorsManager.actors.Add(this); 
        }
    }

    private void OnDestroy()
    {
        // Unregister as an actor
        if (m_ActorsManager)
        {
            m_ActorsManager.actors.Remove(this);
        }
    }
}
