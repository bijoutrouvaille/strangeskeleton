using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.signal.api;
using strange.extensions.signal.impl;

namespace sci
{
    public class SecondContext : MVCSContext
    {

        public SecondContext (MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
        {
        }
        
        protected override void mapBindings()
        {
            injectionBinder.Bind<StartSignal>().ToSingleton();
            injectionBinder.Bind<ShpongleSignal>().ToSingleton().CrossContext();
        }
        // Next two methods are mods to use various extensions, in particular:
        // Signals rather than Events
        protected override void addCoreComponents()
        {
            base.addCoreComponents();
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }
        override public void Launch()
        {
            base.Launch();
            StartSignal startSignal= (StartSignal)injectionBinder.GetInstance<StartSignal>();
            startSignal.Dispatch();
        }
    }
}