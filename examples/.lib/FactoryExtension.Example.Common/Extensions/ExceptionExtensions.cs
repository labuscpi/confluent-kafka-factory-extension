// Copyright 2021. labuscpi
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Text;

namespace FactoryExtension.Example.Common.Extensions
{
    public static class ExceptionExtensions
    {
        
        public static string GetMessage(this Exception ex)
        {
            var e = ex;

            var sb = new StringBuilder(e.Message);
            while (e.InnerException != null)
            {
                e = e.InnerException;
                sb.Append("::").Append(e.Message);
            }

            return sb.ToString();
        }
    }
}