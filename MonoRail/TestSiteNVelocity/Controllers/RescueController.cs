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

namespace TestSiteNVelocity.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	[Rescue("general")]
	public abstract class BaseController : SmartDispatcherController
	{
	}

	[Rescue("saveerror")]
	[Rescue("updateerror", typeof(ApplicationException))]
	[ControllerDetails("rescuable2")]
	public class RescueExtendedController : BaseController
	{
		public void Save()
		{
			throw new Exception();
		}

		public void Save2()
		{
			throw new ApplicationException();
		}
	}

	[Rescue("general")]
	[ControllerDetails("rescuable")]
	public class RescueController : SmartDispatcherController
	{
		public void Index()
		{
			throw new ApplicationException();
		}

		[SkipRescue]
		public void MethodWithSkipRescue()
		{
			throw new ApplicationException();
		}

		[Rescue("saveerror")]
		public void Save()
		{
			throw new ApplicationException();
		}

		[Rescue("updateerror")]
		public void Update()
		{
			LayoutName = "master";

			throw new ApplicationException();
		}

		[Rescue("updateerrormsg")]
		public void UpdateMsg()
		{
			throw new ApplicationException("custom msg");
		}
		
		[Rescue("appException",typeof(ApplicationException))]
		[Rescue("argException",typeof(ArgumentException))]
		public void RescueWithExceptionsByType(String exceptionType)
		{
			if( exceptionType == "appException" )
				throw new ApplicationException("appException");
			
			if( exceptionType == "argException" )
				throw new ArgumentException("argException");
				
			throw new Exception();
		}

		[Rescue("methodDefaultException")]
		[Rescue("appException",typeof(ApplicationException))]
		[Rescue("argException",typeof(ArgumentException))]
		public void RescueWithExceptionsByTypeWithDefaultException(string exceptionType)
		{
			if( exceptionType == "appException" )
				throw new ApplicationException("appException");
			
			if( exceptionType == "argException" )
				throw new ArgumentException("argException");
				
			throw new Exception();
		}

		[AccessibleThrough(Verb.Post)]
		public void OnlyPost()
		{
		}
	}
}
