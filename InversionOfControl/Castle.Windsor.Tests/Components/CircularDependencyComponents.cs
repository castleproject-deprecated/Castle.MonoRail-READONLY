// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Tests.Components
{
	public interface IView
	{
		void Display();
	}

	public interface IController
	{
		void Process();
	}

	public class View : IView
	{
		IController controller;

		public IController Controller
		{
			get { return controller; }
			set { controller = value; }
		}

		public void Display()
		{
		}
	}

	public class Controller : IController
	{
		IView View;

		public Controller(IView view)
		{
			View = view;
		}

		public void Process()
		{
		}
	}
	
	public class CompA
	{
		public CompA(CompB compb)
		{
		}
	}
	
	public class CompB
	{
		public CompB(CompC compC)
		{
		}
	}
	
	public class CompC
	{
		public CompC(CompD compD)
		{
		}
	}
	
	public class CompD
	{
		public CompD(CompA compA)
		{
		}
	}
}