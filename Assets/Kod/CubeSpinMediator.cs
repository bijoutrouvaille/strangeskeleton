using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

namespace sci
{
    public class CubeSpinMediator : Mediator {
        
        [Inject]
        public CubeSpinView concreteView {set {view = value;} }
        public ICubeSpinView view {get;set;}

        // Use this for initialization
        public override void OnRegister() {
            Debug.Log("Hello");
            view.onPress+=onViewPress;
        }
        public override void OnRemove() {
            Debug.Log("Good Bye");
        }

        bool on = false;
        void onViewPress() {
            on = !on;
            if (on) view.start(); else view.stop();
        }

    }

}