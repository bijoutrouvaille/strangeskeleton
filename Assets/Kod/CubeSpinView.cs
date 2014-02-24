using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

namespace sci
{
    public class CubeSpinView : View, ICubeSpinView {

        public event System.Action onPress;
        bool on = false;
        void Update() {
            if (on) {
                var r = transform.localEulerAngles    ;
                if (r.x>=90) r = new Vector3(0,r.y,r.z);
                transform.localEulerAngles = new Vector3(r.x+1,r.y,r.z);
            }
        }
        void OnMouseDown() {
            if (onPress!=null) onPress();
        }
        public void start(){ on = true; }
        public void stop(){ on = false; }
    }



}