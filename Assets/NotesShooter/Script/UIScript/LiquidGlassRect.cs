using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リキッドグラスのシェーダーへ、そのImageの実際の大きさを渡すコンポーネント。
/// 角丸の計算にピクセル単位の大きさが要るが、マテリアルに直接書くと
/// 同じマテリアルを使う全てのImageが同じ大きさ扱いになってしまうので、
/// 実行中だけマテリアルを複製して大きさを個別に渡す。
///
/// エディタ実行中は複製しない。複製するとプレハブ保存時に
/// 「複製したマテリアルは破棄されるがImageは参照したまま」という状態になり、
/// マテリアルの割り当てがDefault UI Materialに戻ってしまう。
/// </summary>
[RequireComponent(typeof(Image))]
public class LiquidGlassRect : MonoBehaviour
{
    [Tooltip("角丸の半径(px)。0以下ならマテリアルの値をそのまま使う")]
    [SerializeField] float radius = -1.0f;

    Image image;
    RectTransform rectTransform;
    Material instancedMaterial;
    Vector2 appliedSize = Vector2.zero;

    static readonly int rectSizeId = Shader.PropertyToID("_RectSize");
    static readonly int radiusId = Shader.PropertyToID("_Radius");

    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        appliedSize = Vector2.zero;
        Apply();
    }

    void OnDestroy()
    {
        //実行中に作った複製だけを片付ける
        if (instancedMaterial != null)
        {
            Destroy(instancedMaterial);
            instancedMaterial = null;
        }
    }

    void Update()
    {
        Apply();
    }

    void Apply()
    {
        if (image == null || rectTransform == null)
        {
            return;
        }

        Vector2 size = rectTransform.rect.size;
        if (size == appliedSize)
        {
            return;
        }

        //複製はまだ作っていなければ作る。元のマテリアルには触らない
        if (instancedMaterial == null)
        {
            Material source = image.material;
            if (source == null)
            {
                return;
            }

            instancedMaterial = new Material(source);
            instancedMaterial.name = source.name + " (" + name + ")";
            image.material = instancedMaterial;
        }

        instancedMaterial.SetVector(rectSizeId, new Vector4(size.x, size.y, 0.0f, 0.0f));
        if (0.0f < radius)
        {
            instancedMaterial.SetFloat(radiusId, radius);
        }

        appliedSize = size;
    }
}
