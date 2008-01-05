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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using NUnit.Framework;

	public abstract class BasePEVerifyTestCase
	{
		protected ProxyGenerator generator;
		protected PersistentProxyBuilder builder;

		private bool verificationDisabled;

		[SetUp]
		public virtual void Init()
		{
			ResetGeneratorAndBuilder();
			verificationDisabled = false;
		}

		public void ResetGeneratorAndBuilder ()
		{
			builder = new PersistentProxyBuilder();
			generator = new ProxyGenerator(builder);
		}

		public void DisableVerification ()
		{
			verificationDisabled = true;
		}

		public bool IsVerificationDisabled
		{
			get { return verificationDisabled; }
		}

#if !MONO // mono doesn't have PEVerify

		[TearDown]
		public virtual void TearDown ()
		{
			if (!IsVerificationDisabled)
			{
				// Note: only supports one generated assembly at the moment
				string path = builder.SaveAssembly();
				if (path != null)
					RunPEVerifyOnGeneratedAssembly (path);
			}
		}

		public void RunPEVerifyOnGeneratedAssembly(string assemblyPath)
		{
			Process process = new Process();

			string path = Path.Combine(ConfigurationManager.AppSettings["sdkDir"], "peverify.exe");

			if (!File.Exists(path))
			{
				path = Path.Combine(ConfigurationManager.AppSettings["x86SdkDir"], "peverify.exe");
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException(
					"Please check the sdkDir configuration setting and set it to the location of peverify.exe");
			}

			process.StartInfo.FileName = path;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
			process.StartInfo.Arguments = "\"" + assemblyPath + "\" /VERBOSE";
			process.StartInfo.CreateNoWindow = true;
			process.Start ();
			string processOutput = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			string result = process.ExitCode + " code ";

			Console.WriteLine(GetType().FullName + ": " + result);

			if (process.ExitCode != 0)
			{
				Console.WriteLine(processOutput);
				Assert.Fail("PeVerify reported error(s): " + Environment.NewLine + processOutput, result);
			}
		}

#else

		[TearDown]
		public virtual void TearDown ()
		{
		}

#endif
	}
}