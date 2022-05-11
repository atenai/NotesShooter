using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRay : MonoBehaviour
{
    public GameObject PurpleHitImage;
    public GameObject RedHitImage;
    public GameObject BlueHitImage;

    // Start is called before the first frame update
    void Start()
    {
        PurpleHitImage.SetActive(false);
        RedHitImage.SetActive(false);
        BlueHitImage.SetActive(false);
    }

    void Update()
    {
        

        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200.0f))
        {//何かがレイに触れた場合 
            if (hit.collider != null)
            {
                if (hit.collider.tag == "PurpleTarget")
                {//敵を見つけた際
                    Debug.Log("紫ターゲットをレイで見つけた");
                    // 衝突したオブジェクトを消す
                    //Destroy(hit.collider.gameObject);
                    PurpleHitImage.SetActive(true);
                }
                else if (hit.collider.tag == "RightTarget")
                {//敵を見つけた際
                    Debug.Log("赤ターゲットをレイで見つけた");
                    // 衝突したオブジェクトを消す
                    //Destroy(hit.collider.gameObject);
                    RedHitImage.SetActive(true);
                }
                else if (hit.collider.tag == "LeftTarget")
                {//敵を見つけた際
                    Debug.Log("青ターゲットをレイで見つけた");
                    // 衝突したオブジェクトを消す
                    //Destroy(hit.collider.gameObject);
                    BlueHitImage.SetActive(true);
                }
                else if (hit.collider.tag != "LeftTarget" || hit.collider.tag != "RightTarget" || hit.collider.tag != "PurpleTarget")
                {
                    PurpleHitImage.SetActive(false);
                    RedHitImage.SetActive(false);
                    BlueHitImage.SetActive(false);
                }
            }
            else if (hit.collider == null)
            {
                PurpleHitImage.SetActive(false);
                RedHitImage.SetActive(false);
                BlueHitImage.SetActive(false);
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
