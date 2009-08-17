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

namespace NVelocity.Runtime.Resource.Loader
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using Commons.Collections;
	using NVelocity.Exception;
	using Util;

	public class AssemblyRelativeResourceLoader : ResourceLoader
	{
		private ArrayList assemblyNames;
		private ArrayList prefixes;

		/// <summary> 
		/// Initialize the template loader with a
		/// a resources class.
		/// </summary>
		public override void Init(ExtendedProperties configuration)
		{
			assemblyNames = configuration.GetVector("assembly");
			prefixes = configuration.GetVector("prefix");
			if (assemblyNames.Count != prefixes.Count)
			{
				throw new ResourceNotFoundException("Need to specify prefixes!");
			}
		}

		///
		/// <summary> Get the InputStream that the Runtime will parse
		/// to create a template.
		/// </summary>
		public override Stream GetResourceStream(String templateName)
		{
			// Make sure we have a valid templateName.
			if (templateName == null || templateName.Length == 0)
			{
				// If we don't get a properly formed templateName
				// then there's not much we can do. So
				// we'll forget about trying to search
				// any more paths for the template.
				throw new ResourceNotFoundException("Need to specify a file name or file path!");
			}

			String template = StringUtils.NormalizePath(templateName);

			if (template.StartsWith("\\"))
			{
				template = template.Substring(1);
			}

			if (template == null || template.Length == 0)
			{
				String msg =
					string.Format(
						"File resource error : argument {0} contains .. and may be trying to access content outside of template root.  Rejected.",
						template);

				throw new ResourceNotFoundException(msg);
			}

			if (template.StartsWith("/"))
			{
				template = template.Substring(1);
			}

			template = template.Replace('\\', '.');
			template = template.Replace('/', '.');

			for(int i = 0; i < assemblyNames.Count; i++)
			{
				String assemblyName = (String) assemblyNames[i];
				String prefix = (String) prefixes[i];
				Assembly assembly;

				try
				{
					assembly = Assembly.Load(assemblyName);
				}
				catch(Exception ex)
				{
					throw new ResourceNotFoundException(string.Format("Assembly could not be found {0}", assemblyName), ex);
				}

				prefix = prefix.Replace('\\', '.');
				prefix = prefix.Replace('/', '.');
				Stream stream = assembly.GetManifestResourceStream(prefix + "." + template);

				if (stream != null)
				{
					return stream;
				}
			}

			throw new ResourceNotFoundException(
				string.Format("AssemblyResourceLoader Error: cannot locate resource {0}", template));
		}

		/// <summary> Given a template, check to see if the source of InputStream
		/// has been modified.
		/// </summary>
		public override bool IsSourceModified(Resource resource)
		{
			return false;
		}

		/// <summary> Get the last modified time of the InputStream source
		/// that was used to create the template. We need the template
		/// here because we have to extract the name of the template
		/// in order to locate the InputStream source.
		/// </summary>
		public override long GetLastModified(Resource resource)
		{
			return resource.LastModified;
		}
	}
}
