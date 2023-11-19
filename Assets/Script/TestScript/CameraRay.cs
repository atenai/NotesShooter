using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class CameraRay : MonoBehaviour
{
    [SerializeField] RawImage PurpleHitImage;
    [SerializeField] RawImage RedHitImage;
    [SerializeField] RawImage BlueHitImage;


    Ray ray;//わざと関数内では無く、グローバルに出している

    void Start()
    {
        PurpleHitImage.gameObject.SetActive(true);
        PurpleHitImage.enabled = false;
        RedHitImage.gameObject.SetActive(true);
        RedHitImage.enabled = false;
        BlueHitImage.gameObject.SetActive(true);
        BlueHitImage.enabled = false;
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
        RayHit();
    }

    void Update()
    {
        Debug.Log("Update");
        RayHit();
    }

    void LateUpdate()
    {
        Debug.Log("LateUpdate");
        RayHit();
    }

    void RayHit()
    {
        ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200.0f))
        {
            //何かがレイに触れた場合 
            if (hit.collider != null)
            {
                if (hit.collider.tag == "PurpleTarget")
                {
                    //敵を見つけた際
                    Debug.Log("<color=purple>" + "紫ターゲットをレイで見つけた" + "</color>");
                    // 衝突したオブジェクトを消す
                    //Destroy(hit.collider.gameObject);
                    PurpleHitImage.enabled = true;
                    RedHitImage.enabled = false;
                    BlueHitImage.enabled = false;
                }
                else if (hit.collider.tag == "RightTarget")
                {
                    //敵を見つけた際
                    Debug.Log("<color=red>" + "赤ターゲットをレイで見つけた" + "</color>");
                    // 衝突したオブジェクトを消す
                    //Destroy(hit.collider.gameObject);
                    PurpleHitImage.enabled = false;
                    RedHitImage.enabled = true;
                    BlueHitImage.enabled = false;
                }
                else if (hit.collider.tag == "LeftTarget")
                {
                    //敵を見つけた際
                    Debug.Log("<color=blue>" + "青ターゲットをレイで見つけた" + "</color>");
                    // 衝突したオブジェクトを消す
                    //Destroy(hit.collider.gameObject);
                    PurpleHitImage.enabled = false;
                    RedHitImage.enabled = false;
                    BlueHitImage.enabled = true;
                }
                else if (hit.collider.tag != "LeftTarget" || hit.collider.tag != "RightTarget" || hit.collider.tag != "PurpleTarget")
                {
                    PurpleHitImage.enabled = false;
                    RedHitImage.enabled = false;
                    BlueHitImage.enabled = false;
                }
                else
                {
                    PurpleHitImage.enabled = false;
                    RedHitImage.enabled = false;
                    BlueHitImage.enabled = false;
                }
            }
            else if (hit.collider == null)
            {
                PurpleHitImage.enabled = false;
                RedHitImage.enabled = false;
                BlueHitImage.enabled = false;
            }
            else
            {
                PurpleHitImage.enabled = false;
                RedHitImage.enabled = false;
                BlueHitImage.enabled = false;
            }
        }
        Debug.DrawRay(ray.origin, ray.direction * 200, Color.red, 1);

        //foreach (RaycastHit hit in Physics.RaycastAll(ray))
        //{
        //    if (hit.collider.tag == "RightTarget")
        //    {//敵を見つけた際
        //        Debug.Log("右ターゲットをレイで見つけた");
        //        RedHitImage.SetActive(true);
        //    }
        //    else
        //    {
        //        RedHitImage.SetActive(false);
        //    }

        //    if (hit.collider.tag == "LeftTarget")
        //    {//敵を見つけた際
        //        Debug.Log("左ターゲットをレイで見つけた");
        //        BlueHitImage.SetActive(true);
        //    }
        //    else
        //    {
        //        BlueHitImage.SetActive(false);
        //    }
        //}
    }
}
