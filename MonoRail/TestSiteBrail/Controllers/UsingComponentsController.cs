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

namespace Castle.MonoRail.Views.Brail.TestSite.Controllers
{
    using Boo.Lang;
    using Castle.MonoRail.Framework;
    using System;

    [Serializable]
    public class UsingComponentsController : SmartDispatcherController
    {
        public void CaptureFor()
        {
        }

        public void CaptureForWithLayout()
        {
            this.LayoutName = "layout_with_captureFor";
        }

        public void DynamicComponents()
        {
            PropertyBag["components"] = new string[]
                {
                    "SimpleInlineViewComponent3", 
                    "SimpleInlineViewComponent2"
                };

        }

        public void Index1()
        {
        }

        public void Index10()
        {
        }

        public void Index2()
        {
        }

        public void Index3()
        {
        }

        public void Index4()
        {
        }

        public void Index5()
        {
        }

        public void Index8()
        {
            object[] objArray1 = new object[] { 1, 2 };
            List list1 = new List(objArray1, true);
            this.PropertyBag.Add("items", list1);
        }

        public void Index9()
        {
        }

        public void Template()
        {
        }

        public void WithParams()
        {
        }

    }
}

