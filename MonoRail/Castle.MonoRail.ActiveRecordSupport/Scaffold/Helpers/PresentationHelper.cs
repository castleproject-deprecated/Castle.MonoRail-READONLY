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

namespace Castle.MonoRail.ActiveRecordSupport.Scaffold.Helpers
{
	using System;
	using System.Collections;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.MonoRail.Framework.Helpers;
	
	public class PresentationHelper : AbstractHelper
	{
		public String StartAlternateTR(int index, String styleClass, String altStyleClass)
		{
			return String.Format("<tr class=\"{0}\">", index % 2 == 0 ? styleClass : altStyleClass);
		}

		public String BestAlignFor(Type type)
		{
			if (type == typeof(String))
			{
				return "left";
			}

			return "center";
		}
		
		public String LinkToBack(String text, IDictionary attributes)
		{
			return String.Format( "<a href=\"javascript:history.go(-1);\" {1}>{0}</a>", 
			                      text, GetAttributes(attributes) );
		}

		public String LinkToList(ActiveRecordModel model, bool useModelName, String text, IDictionary attributes)
		{
			if (useModelName)
			{
				return String.Format("<a href=\"list{0}.{1}\" {3}>{2}</a>", model.Type.Name, 
					Context.UrlInfo.Extension, text, GetAttributes(attributes));
			}
			else
			{
				return String.Format("<a href=\"list.{0}\" {2}>{1}</a>",
					Context.UrlInfo.Extension, text, GetAttributes(attributes));
			}
		}

		public String LinkToNew(ActiveRecordModel model, bool useModelName, String text, IDictionary attributes)
		{
			if (useModelName)
			{
				return String.Format("<a href=\"new{0}.{1}\" {3}>{2}</a>", model.Type.Name, 
					Context.UrlInfo.Extension, text, GetAttributes(attributes));
			}
			else
			{
				return String.Format("<a href=\"new.{0}\" {2}>{1}</a>", 
					Context.UrlInfo.Extension, text, GetAttributes(attributes));
			}
		}

		public String LinkToEdit(ActiveRecordModel model, bool useModelName, 
		                         String text, object key, IDictionary attributes)
		{
			if (useModelName)
			{
				return String.Format("<a href=\"edit{0}.{1}?id={4}\" {3}>{2}</a>", model.Type.Name, 
					Context.UrlInfo.Extension, text, GetAttributes(attributes), key);
			}
			else
			{
				return String.Format("<a href=\"edit.{0}?id={3}\" {2}>{1}</a>", 
					Context.UrlInfo.Extension, text, GetAttributes(attributes), key);
			}
		}

		public String LinkToConfirm(ActiveRecordModel model, bool useModelName, String text, object key, IDictionary attributes)
		{
			if (useModelName)
			{
				return String.Format("<a href=\"confirm{0}.{1}?id={4}\" {3}>{2}</a>", model.Type.Name, 
					Context.UrlInfo.Extension, text, GetAttributes(attributes), key);
			}
			else
			{
				return String.Format("<a href=\"confirm.{0}?id={3}\" {2}>{1}</a>", 
					Context.UrlInfo.Extension, text, GetAttributes(attributes), key);
			}
		}

		public String LinkToRemove(ActiveRecordModel model, bool useModelName, String text, object key, IDictionary attributes)
		{
			if (useModelName)
			{
				return String.Format("<a href=\"remove{0}.{1}?id={4}\" {3}>{2}</a>", model.Type.Name, 
					Context.UrlInfo.Extension, text, GetAttributes(attributes), key);
			}
			else
			{
				return String.Format("<a href=\"remove.{0}?id={3}\" {2}>{1}</a>", 
					Context.UrlInfo.Extension, text, GetAttributes(attributes), key);
			}
		}
	}
}
