/*
 * Copyright 2022 Systemic Pty Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Sif.Framework.Demo.Provider.Models;
using Sif.Framework.Models.Parameters;
using Sif.Framework.Models.Query;
using Sif.Framework.Services.Providers;

namespace Sif.Framework.Demo.Provider.Services;

public class SchoolInfoService : IBasicProviderService<SchoolInfo>
{
    public SchoolInfo Create(
        SchoolInfo obj,
        bool? mustUseAdvisory = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        throw new NotImplementedException();
    }

    public SchoolInfo Retrieve(
        string refId,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        throw new NotImplementedException();
    }

    public List<SchoolInfo> Retrieve(
        uint? pageIndex = null,
        uint? pageSize = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        var school = new SchoolInfo { SchoolName = "Applecross SHS" };
        var schools = new List<SchoolInfo> { school };

        return schools;
    }

    public List<SchoolInfo> Retrieve(
        IEnumerable<EqualCondition> conditions,
        uint? pageIndex = null,
        uint? pageSize = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        var school = new SchoolInfo { SchoolName = "Rossmoyne Conditional SHS" };
        var schools = new List<SchoolInfo> { school };

        return schools;
    }

    public List<SchoolInfo> Retrieve(
        SchoolInfo obj,
        uint? pageIndex = null,
        uint? pageSize = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        throw new NotImplementedException();
    }

    public void Update(
        SchoolInfo obj,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        throw new NotImplementedException();
    }

    public void Delete(
        string refId,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        throw new NotImplementedException();
    }
}