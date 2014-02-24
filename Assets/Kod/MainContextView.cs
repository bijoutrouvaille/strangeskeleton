using System;
using UnityEngine;
using strange.extensions.context.impl;

namespace sci
{
    public class MainContextView : ContextView
    {
        void Awake()
        {
            context = new MainContext(this, true);
            context.Start();
        }
    }
}