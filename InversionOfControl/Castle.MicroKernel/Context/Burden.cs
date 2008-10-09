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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections.Generic;
	using Core;

	public class Burden
	{
		private object instance;
		private IHandler handler;
		private readonly List<Burden> children = new List<Burden>();

		public void SetRootInstance(object instance, IHandler handler)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (handler == null) throw new ArgumentNullException("handler");

			this.instance = instance;
			this.handler = handler;
		}

		public void AddChild(Burden burden)
		{
			children.Add(burden);
		}

		public ComponentModel Model
		{
			get { return handler.ComponentModel; }
		}

		public void Release(IReleasePolicy policy)
		{
			if (policy == null) throw new ArgumentNullException("policy");

			handler.Release(instance);

			foreach(Burden child in children)
			{
				policy.Release(child.instance);
			}
		}
	}
}