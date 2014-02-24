using strange.extensions.signal.impl;
using strange.extensions.command.impl;
using UnityEngine;

namespace sci
{
    public class ShpongleSignal : Signal<string> {};
    public class StartSignal : Signal {};
    public class StartCommand : Command { public override void Execute () {
            Debug.Log("Starting");
        } };

}