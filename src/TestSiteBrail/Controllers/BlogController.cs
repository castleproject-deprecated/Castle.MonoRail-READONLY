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

namespace Castle.MonoRail.Views.Brail.TestSite.Controllers
{
    using Castle.MonoRail.Framework;
    using System;

    [Serializable]
    public class BlogController : SmartDispatcherController
    {
        public void View(int year, int month)
        {
            this.RenderText(string.Format("Blog: year={0} month={1}", year, month));
        }

    }
}

