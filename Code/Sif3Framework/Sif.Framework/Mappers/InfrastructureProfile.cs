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

using AutoMapper;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Requests;
using Sif.Framework.Model.Responses;
using Sif.Specification.Infrastructure;
using System.Collections.Generic;
using System.Xml;

namespace Sif.Framework.Mappers
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            CreateMap<ApplicationInfo, applicationInfoType>()
                .ReverseMap();

            CreateMap<InfrastructureService, infrastructureServiceType>()
                .ForMember(dest => dest.nameSpecified, opt => opt.MapFrom(src => true));
            CreateMap<infrastructureServiceType, InfrastructureService>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Environment, environmentType>()
                .ForMember(dest => dest.typeSpecified, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.fingerprint, opt => opt.Ignore());
            CreateMap<environmentType, Environment>();

            CreateMap<ProductIdentity, productIdentityType>()
                .ReverseMap();

            CreateMap<Property, propertyType>()
                .ReverseMap();

            CreateMap<ProvisionedZone, provisionedZoneType>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.SifId));
            CreateMap<provisionedZoneType, ProvisionedZone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SifId, opt => opt.MapFrom(src => src.id));

            CreateMap<Right, rightType>()
                .ReverseMap();

            CreateMap<Model.Infrastructure.Service, serviceType>();
            CreateMap<serviceType, Model.Infrastructure.Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Zone, zoneType>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.SifId));
            CreateMap<zoneType, Zone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SifId, opt => opt.MapFrom(src => src.id));

            CreateMap<PhaseState, stateType>()
                .ForMember(dest => dest.createdSpecified, opt => opt.MapFrom(src => src.Created != null))
                .ForMember(dest => dest.lastModifiedSpecified, opt => opt.MapFrom(src => src.LastModified != null));
            CreateMap<stateType, PhaseState>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Phase, phaseType>();
            CreateMap<phaseType, Phase>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Initialization, initializationType>()
                .ReverseMap();

            CreateMap<Job, jobType>()
                .ForMember(dest => dest.createdSpecified, opt => opt.MapFrom(src => src.Created != null))
                .ForMember(dest => dest.lastModifiedSpecified, opt => opt.MapFrom(src => src.LastModified != null))
                .ForMember(dest => dest.stateSpecified, opt => opt.MapFrom(src => src.State != null))
                .ForMember(dest => dest.timeout, opt => opt.MapFrom(src => XmlConvert.ToString(src.Timeout)));
            CreateMap<jobType, Job>()
                .ForMember(dest => dest.Timeout, opt => opt.MapFrom(src => XmlConvert.ToTimeSpan(src.timeout)));

            CreateMap<ResponseError, errorType>()
                .ReverseMap();

            CreateMap<CreateStatus, createType>()
                .ReverseMap();

            CreateMap<DeleteStatus, deleteStatus>()
                .ReverseMap();

            CreateMap<UpdateStatus, updateType>()
                .ReverseMap();

            CreateMap<MultipleCreateResponse, createResponseType>()
                .ForMember(dest => dest.creates, opt => opt.MapFrom(src => src.StatusRecords));
            CreateMap<createResponseType, MultipleCreateResponse>()
                .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.creates));

            CreateMap<MultipleDeleteResponse, deleteResponseType>()
                .ForMember(dest => dest.deletes, opt => opt.MapFrom(src => src.StatusRecords));
            CreateMap<deleteResponseType, MultipleDeleteResponse>()
                .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.deletes));

            CreateMap<MultipleUpdateResponse, updateResponseType>()
                .ForMember(dest => dest.updates, opt => opt.MapFrom(src => src.StatusRecords));
            CreateMap<updateResponseType, MultipleUpdateResponse>()
                .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.updates));

            CreateMap<deleteIdType[], ICollection<string>>()
                .ReverseMap();

            CreateMap<deleteRequestType, MultipleDeleteRequest>()
                .ForMember(dest => dest.RefIds, opt => opt.MapFrom(src => src.deletes));
        }
    }
}