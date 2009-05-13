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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor
{
	using System;
	using System.Xml;

	using Castle.Core.Resource;

	public interface IXmlProcessorEngine
	{
		void AddNodeProcessor(Type type);

		void DispatchProcessAll(IXmlProcessorNodeList nodeList);

		void DispatchProcessCurrent(IXmlProcessorNodeList nodeList);

		void AddProperty(XmlElement element);
		
		bool HasProperty(String name);
		
		XmlElement GetProperty(String name);

		bool HasFlag(String flag);

		void AddFlag(String flag);
		
		void RemoveFlag(String flag);

		void PushResource(IResource resource);
		
		IResource GetResource(string uri);
		
		void PopResource();

		bool HasSpecialProcessor( XmlNode node );
	}
}
