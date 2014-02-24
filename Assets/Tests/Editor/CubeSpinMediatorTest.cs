using System;
using NUnit.Framework;
using UnityEngine;
using NSubstitute;

namespace sci.tests
{
	[TestFixture()]
	public class CubeSpinMediatorTest : MediatorTest
	{
		CubeSpinMediator mediator;
		ICubeSpinView view;

		[SetUp]
		public void SetUp() {
			mediator = gameObject.AddComponent<CubeSpinMediator>();
			view = Substitute.For<ICubeSpinView>();
			mediator.view = view;
		}

		[TearDown]
		public void TearDown() {
		}

		[Test()]
		public void TestStartStopToggle()
		{

			mediator.view = view;
			mediator.OnRegister();

			view.onPress+=Raise.Event<Action>();

			view.Received().start();
			view.DidNotReceive().stop();

			view.onPress+=Raise.Event<Action>();

			view.Received().stop();
		}
	}
}

