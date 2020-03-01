#if RCSCE
using UnityEngine;

public class CouponSystem : MonoBehaviour {
    void Start() {
        Database.singleton.onConnected.AddListener(Database.singleton.Connect_CouponSystem);
    }
}
#endif