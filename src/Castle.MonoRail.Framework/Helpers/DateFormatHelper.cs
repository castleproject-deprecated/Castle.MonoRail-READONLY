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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;

	/// <summary>
	/// Simple helper for date formatting
	/// </summary>
	public class DateFormatHelper : AbstractHelper
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DateFormatHelper"/> class.
		/// </summary>
		public DateFormatHelper() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DateFormatHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public DateFormatHelper(IEngineContext engineContext) : base(engineContext) { }
		#endregion

		/// <summary>
		/// Alternative representation of a difference
		/// between the specified date and now. If within 24hr
		/// it returns <c>Today</c>. If within 48hr it returns
		/// <c>Yesterday</c>. If within 40 days it returns
		/// <c>x days ago</c> and otherwise it returns
		/// <c>x months ago</c>
		/// <para>
		/// TODO: Think about i18n
		/// </para>
		/// </summary>
		/// <param name="date">The date in the past (should be equal or less than now)</param>
		/// <returns></returns>
		public String AlternativeFriendlyFormatFromNow(DateTime date)
		{
			TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
			TimeSpan cur = new TimeSpan(date.Ticks);

			TimeSpan diff = now.Subtract(cur);

			if (diff.TotalHours <= 24)
			{
				return "Today";
			}
			else if (diff.TotalHours <= 48)
			{
				return "Yesterday";
			}
			else if (diff.TotalDays <= 40)
			{
				return String.Format("{0} days ago", diff.Days);
			}
			else 
			{
				return String.Format("{0} months ago", (diff.Days / 30));
			}
		}

		/// <summary>
		/// Returns the difference from the 
		/// specified <c>date</c> the the current date
		/// in a friendly string like "1 day ago"
		/// <para>
		/// TODO: Think about i18n
		/// </para>
		/// </summary>
		/// <param name="date">The date in the past (should be equal or less than now)</param>
		/// <returns></returns>
		public String FriendlyFormatFromNow(DateTime date)
		{
			TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
			TimeSpan cur = new TimeSpan(date.Ticks);

			TimeSpan diff = now.Subtract(cur);

			if (diff.TotalSeconds == 0)
			{
				return "Just now";
			}

			if (diff.Days == 0)
			{
				if (diff.Hours == 0)
				{
					if (diff.Minutes == 0)
					{
						return String.Format("{0} second{1} ago", 
							diff.Seconds, diff.Seconds > 1 ? "s" : String.Empty);
					}
					else
					{
						return String.Format("{0} minute{1} ago", 
							diff.Minutes, diff.Minutes > 1 ? "s" : String.Empty);
					}
				}
				else
				{
					return String.Format("{0} hour{1} ago", 
						diff.Hours, diff.Hours > 1 ? "s" : String.Empty);
				}
			}
			else
			{
				return String.Format("{0} day{1} ago", 
					diff.Days, diff.Days > 1 ? "s" : String.Empty);
			}
		}

		/// <summary>
		/// Formats to short date
		/// </summary>
		/// <param name="date"></param>
		/// <returns>Short date, or <c>String.Empty</c> if <paramref name="date"/> is <c>null</c>.</returns>
		public String ToShortDate(DateTime? date)
		{
			return date.HasValue ? date.Value.ToShortDateString() : string.Empty;
		}
		
		/// <summary>
		/// Formats to short date
		/// </summary>
		/// <param name="date"></param>
		/// <returns>Short date and time, or <c>String.Empty</c> if <paramref name="date"/> is <c>null</c>.</returns>
		public String ToShortDateTime(DateTime? date)
		{
			return date.HasValue ? date.Value.ToShortDateString() + " " + date.Value.ToShortTimeString() : string.Empty;
		}
	}
}
