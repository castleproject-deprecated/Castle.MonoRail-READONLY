// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
namespace Castle.MonoRail.Views.Brail
{
	using System.Configuration;
	using System.Reflection;
	using System.Xml;

	public class BrailConfigurationSection : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			BooViewEngineOptions options = new BooViewEngineOptions();
			if (section.Attributes["batch"] != null)
				options.BatchCompile = bool.Parse(section.Attributes["batch"].Value);
			if (section.Attributes["saveToDisk"] != null)
				options.SaveToDisk = bool.Parse(section.Attributes["saveToDisk"].Value);
			if (section.Attributes["debug"] != null)
				options.Debug = bool.Parse(section.Attributes["debug"].Value);
			if (section.Attributes["extention"] != null)
				options.Extention = section.Attributes["extention"].Value;
			if (section.Attributes["saveDirectory"] != null)
				options.SaveDirectory = section.Attributes["saveDirectory"].Value;
			if (section.Attributes["commonScriptsDirectory"] != null)
				options.CommonScriptsDirectory = section.Attributes["commonScriptsDirectory"].Value;
			foreach(XmlNode refence in section.SelectNodes("reference"))
			{
				Assembly asm = Assembly.Load(refence.Attributes["assembly"].Value);
				options.AssembliesToReference.Add(asm);
			}
			return options;
		}
	}
}