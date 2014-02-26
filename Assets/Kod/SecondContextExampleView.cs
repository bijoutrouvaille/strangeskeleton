using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

namespace sci
{
    public class SecondContextExampleView : View {
        
        [Inject]
        public ShpongleSignal shpongle {get{
                return _shpongle;
            }set{
                _shpongle = value;
            }}
        public ShpongleSignal _shpongle = null;

        bool sent = false;
        void Update() {
            if (shpongle!=null && !sent) {
                sent = true;
                shpongle.AddListener( x => Debug.Log(this.name+" got shpongle "+x));
                StartCoroutine(send());
            }
        }
        IEnumerator send() {
            yield return new WaitForSeconds(2f);
//            Debug.Log("dispatching shpongle on "+this.name);
            shpongle.Dispatch(this.name);
        }
    }

}