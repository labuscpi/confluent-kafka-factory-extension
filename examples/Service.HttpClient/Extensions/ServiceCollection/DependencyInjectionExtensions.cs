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
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Service.HttpClient.Interfaces.Settings;
using Service.HttpClient.Models.Settings;

namespace Service.HttpClient.Extensions.ServiceCollection
{
    public static class DependencyInjectionExtensions
    {
        public static void AddHttpClient(this IServiceCollection services, string name, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(HttpClientSettings)).Get<HttpClientSettings>();
            services.TryAddSingleton<IHttpClientSettings>(settings);

            var uri = $"{settings.Protocol}://{settings.Host}:{settings.Port}/";
            services.AddHttpClient(name, c =>
            {
                c.BaseAddress = new Uri(uri, UriKind.Absolute);
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
        }
    }
}
