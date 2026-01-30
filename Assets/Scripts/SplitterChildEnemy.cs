
using UnityEngine;

public class SplitterChildEnemy : Enemy
{
    public float lifeTime = 2f;
    public float alpha = 0.45f;
    public float scaleMultiplier = 0.35f;
    public float speedMultiplier = 1.4f;

    protected override void Start()
    {
        base.Start();
        transform.localScale *= scaleMultiplier;
        moveSpeed *= speedMultiplier;
        SetAlpha(alpha);
        Destroy(gameObject, lifeTime);
    }

    void SetAlpha(float a)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = a;
                    mat.color = c;

                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                }
            }
        }
    }

    public override void OnSliced(Vector3 sliceDirection)
    {
        base.OnSliced(sliceDirection);
    }
}
