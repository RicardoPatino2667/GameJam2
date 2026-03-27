using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    public Transform attackRange;
    public float hitRange = 0.5f;
    public LayerMask enemyLayers;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackRange.position,
            hitRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(10);
            }
        }
    }
}