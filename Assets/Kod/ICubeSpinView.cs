using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

namespace sci
{
    public interface ICubeSpinView {
        void start();
        void stop();
        event System.Action onPress;
    }



}