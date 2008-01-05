// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Applications.PestControl.Services.SourceControl
{
	using System;

	using Castle.Applications.PestControl.Model;

	/// <summary>
	/// Summary description for SvnSourceControl.
	/// </summary>
	public class SvnSourceControl : ISourceControl
	{
		ParameterDefinitionCollection _params;

		public SvnSourceControl()
		{
			_params = new ParameterDefinitionCollection();

			_params.Add( new ParameterDefinition("URL") );
			_params.Add( new ParameterDefinition("User name") );
			_params.Add( new ParameterDefinition("Password") );
		}

		#region ISourceControl Members

		public String Name
		{
			get { return "SubVersion Repository"; }
		}

		public String Key
		{
			get { return "svnsc"; }
		}

		public bool HasModifications(Project project, DateTime since)
		{
			return false;
		}

		public ParameterDefinitionCollection ParametersDefinition
		{
			get { return _params; }
		}

		#endregion
	}
}
