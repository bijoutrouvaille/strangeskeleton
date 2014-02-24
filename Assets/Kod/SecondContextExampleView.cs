using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

namespace sci
{
    public class SecondContextExampleView : View {
        
        [Inject]
        public ShpongleSignal shpongle {get;set;}
    }

}