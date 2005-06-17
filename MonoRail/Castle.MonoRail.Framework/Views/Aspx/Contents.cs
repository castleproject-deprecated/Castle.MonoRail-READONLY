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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Web;
	using System.Web.UI;

	/// <summary>
	/// Control used on master pages to represent 
	/// the position where the child page contents 
	/// should be written.
	/// </summary>
	public class Contents : Control
	{
		protected override void Render(HtmlTextWriter writer)
		{
			byte[] contentsArray = (byte[]) HttpContext.Current.Items["rails.contents"];

			if (contentsArray != null)
			{
				String contents = writer.Encoding.GetString(contentsArray);

				writer.InnerWriter.Write( contents.TrimEnd('\0') );
			}
			else
			{
				writer.Write("<strong>The child page wasn't processed by the rails engine. " + 
					"Was this page invoked directly?</strong>");
			}
		}
	}
}
