// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.Web;

	/// <summary>
	/// Delegates to ASP.Net TraceContext.
	/// </summary>
	public class TraceAdapter : ITrace
	{
		private TraceContext trace;

		/// <summary>
		/// Initializes a new instance of the <see cref="TraceAdapter"/> class.
		/// </summary>
		/// <param name="traceContext">The trace context.</param>
		public TraceAdapter(TraceContext traceContext)
		{
			trace = traceContext;
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="message">The message.</param>
		public void Warn(String message)
		{
			trace.Warn(message);
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		public void Warn(String category, String message)
		{
			trace.Warn(category, message);
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		/// <param name="errorInfo">The error info.</param>
		public void Warn(String category, String message, Exception errorInfo)
		{
			trace.Warn(category, message, errorInfo);
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="message">The message.</param>
		public void Write(String message)
		{
			trace.Write(message);
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		public void Write(String category, String message)
		{
			trace.Write(category, message);
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		/// <param name="errorInfo">The error info.</param>
		public void Write(String category, String message, Exception errorInfo)
		{
			trace.Write(category, message, errorInfo);
		}
	}
}