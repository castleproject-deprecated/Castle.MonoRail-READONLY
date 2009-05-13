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

namespace Extending2.Components
{
	using System;
	using System.Threading;

	using System.Windows.Forms;

	/// <summary>
	/// Summary description for WinformApplicationRunner
	/// </summary>
	public class WinformApplicationRunner : IStartable
	{
		private Thread _thread;
		private MainForm _form;

		public WinformApplicationRunner(MainForm form)
		{
			_form = form;
		}

		public void Start()
		{
			_thread = new Thread(new ThreadStart(StartApp));
			_thread.Start();
		}

		private void StartApp()
		{
			Application.Run( _form );
		}
	}
}
