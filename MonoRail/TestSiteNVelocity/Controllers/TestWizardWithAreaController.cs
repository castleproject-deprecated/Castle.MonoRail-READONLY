// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace TestSiteNVelocity.Controllers
{
	using System;
	using Castle.MonoRail.Framework;

	[DynamicActionProvider(typeof(WizardActionProvider))]
	[ControllerDetails(Area="wizard")]
	public class TestWizardWithAreaController : Controller, IWizardController
	{
		public void Index()
		{
			RenderText("Hello!");
		}

		public bool UseCurrentRouteForRedirects
		{
			get { return false; }
		}

		public void OnWizardStart()
		{
		}

		public bool OnBeforeStep(String wizardName, String stepName, IWizardStepPage step)
		{
			return true;
		}

		public void OnAfterStep(String wizardName, String stepName, IWizardStepPage step)
		{
			
		}

		public IWizardStepPage[] GetSteps(IEngineContext context)
		{
			return new WizardStepPage[] { new Page1(), new Page2(), new Page3(), new Page4() };
		}
		
		public class Page1 : WizardStepPage
		{
			public void InnerAction()
			{
				Flash["InnerActionInvoked"] = true;

				RenderText("InnerAction contents");
			}

			public void InnerAction2()
			{
			}
		}

		public class Page2 : WizardStepPage
		{
			protected override void RenderWizardView()
			{
				RenderText("A content rendered using RenderText");
			}
		}

		public class Page3 : WizardStepPage
		{
			protected override void RenderWizardView()
			{
				RenderView("page3");
			}
		}

		public class Page4 : WizardStepPage
		{
			public void InnerAction()
			{
				Flash["InnerActionInvoked"] = true;

				DoNavigate();
			}		
		}
	}

	
}
