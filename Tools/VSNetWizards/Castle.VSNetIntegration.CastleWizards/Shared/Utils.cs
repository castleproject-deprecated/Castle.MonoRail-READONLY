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

namespace Castle.VSNetIntegration.CastleWizards.Shared
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using System.Xml;
	using EnvDTE;
	using EnvConstants = EnvDTE.Constants;

	public class Utils
	{
		public static void AddReference(Project project, Project otherProject)
		{
			VSLangProj.VSProject vsProject;

			vsProject = (VSLangProj.VSProject) project.Object;
			vsProject.References.AddProject(otherProject);
		}

		public static void AddReference(Project project, String assembly)
		{
			VSLangProj.VSProject vsProject;

			vsProject = (VSLangProj.VSProject) project.Object;
			vsProject.References.Add(assembly);
		}

		public static void PerformReplacesOn(Project project, String projectName,
		                                     String localProjectPath, String filename)
		{
			String[] elems = filename.Split('\\');

			ProjectItem item = null;

			foreach(String elem in elems)
			{
				if (item == null)
				{
					item = project.ProjectItems.Item(elem);
				}
				else
				{
					item = item.ProjectItems.Item(elem);
				}
			}

			if (item == null)
			{
				throw new Exception("Path for ProjectItem not found: " + filename);
			}

			PerformReplacesOn(project, projectName, localProjectPath, item);
		}

		public static void PerformReplacesOn(Project project, String projectName,
		                                     String localProjectPath, ProjectItem item)
		{
			Window codeWindow = item.Open(EnvConstants.vsViewKindTextView);

			codeWindow.Activate();

			ReplaceToken(codeWindow, "!NAMESPACE!", projectName);
			ReplaceToken(codeWindow, "!APPPHYSICALDIR!", localProjectPath);

			codeWindow.Close(vsSaveChanges.vsSaveChangesYes);
		}

		public static void ReplaceToken(Window window, string token, string replaceWith)
		{
			window.Document.ReplaceText(token, replaceWith, 256);
		}

		public static void AddCommonPostBuildEvent(Project project)
		{
			VSLangProj.VSProject vsProject = (VSLangProj.VSProject) project.Object;

			vsProject.Project.Properties.Item("PostBuildEvent").Value =
				"copy \"$(ProjectDir)\\App.config\" \"$(TargetPath).config\"";
		}

		public static XmlDocument CreateXmlDomForConfig(ExtensionContext context, Project project, ProjectItem item,
		                                                String file)
		{
			if (context.Properties[Constants.ConfigFileList] == null)
			{
				context.Properties[Constants.ConfigFileList] = new ArrayList();
			}

			Window codeWindow = item.Open(EnvConstants.vsViewKindCode);

			codeWindow.Activate();

			TextDocument objTextDoc = ((EnvDTE.TextDocument) (
			                                                 	codeWindow.Document.Object("TextDocument")));

			EditPoint objEditPt = objTextDoc.StartPoint.CreateEditPoint();
			objEditPt.StartOfDocument();
			String content = objEditPt.GetText(objTextDoc.EndPoint);

			codeWindow.Close(vsSaveChanges.vsSaveChangesYes);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(content);

			context.Properties[file] = doc;

			(context.Properties[Constants.ConfigFileList] as IList).Add(file);

			return doc;
		}

		public static XmlDocument CreateXmlDomForConfig(ExtensionContext context, Project project, String file)
		{
			if (context.Properties[Constants.ConfigFileList] == null)
			{
				context.Properties[Constants.ConfigFileList] = new ArrayList();
			}

			ProjectItem item = project.ProjectItems.Item(file);

			Window codeWindow = item.Open(EnvConstants.vsViewKindCode);

			codeWindow.Activate();

			TextDocument objTextDoc = ((EnvDTE.TextDocument) (
			                                                 	codeWindow.Document.Object("TextDocument")));

			EditPoint objEditPt = objTextDoc.StartPoint.CreateEditPoint();
			objEditPt.StartOfDocument();
			String content = objEditPt.GetText(objTextDoc.EndPoint);

			codeWindow.Close(vsSaveChanges.vsSaveChangesYes);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(content);

			context.Properties[file] = doc;

			(context.Properties[Constants.ConfigFileList] as IList).Add(file);

			return doc;
		}

		public static void EnsureDirExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		/// <summary>
		/// Create a valid C# identifier from an input string
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string CreateValidIdentifierFromName(string name)
		{
			StringBuilder validIdentifier = new StringBuilder();
			foreach(char c in name)
			{
				// Only allow valid identifier elements
				//  letter-character
				//  decimal-digit-character
				//  connecting-character
				//  combining-character
				//  formatting character

				UnicodeCategory category = Char.GetUnicodeCategory(c);
				switch(category)
				{
					case UnicodeCategory.LetterNumber:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.LowercaseLetter:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.UppercaseLetter:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.TitlecaseLetter:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.ModifierLetter:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.OtherLetter:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.NonSpacingMark:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.SpacingCombiningMark:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.DecimalDigitNumber:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.ConnectorPunctuation:
						validIdentifier.Append(c);
						break;
					case UnicodeCategory.Format:
						validIdentifier.Append(c);
						break;
					default:
						continue;
				}
			}

			return validIdentifier.ToString();
		}
	}
}