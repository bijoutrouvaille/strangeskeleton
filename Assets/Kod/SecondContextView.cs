
using System;
using UnityEngine;
using strange.extensions.context.impl;

namespace sci
{
    public class SecondContextView : ContextView
    {
        void Awake()
        {
            context = new SecondContext(this, true);
            context.Start();
            
        }
    }
}