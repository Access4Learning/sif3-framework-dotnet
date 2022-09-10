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
using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Models.Requests;
using Sif.Framework.Models.Responses;
using Sif.Specification.Infrastructure;
using System.Collections.Generic;
using System.Xml;
using Environment = Sif.Framework.Models.Infrastructure.Environment;

namespace Sif.Framework.Services.Mapper
{
    /// <summary>
    /// Factory class for managing object to object mappings.
    /// </summary>
    public static class MapperFactory
    {
        public static IMapper Mapper { get; }

        static MapperFactory()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationInfo, applicationInfoType>()
                    .ReverseMap();

                cfg.CreateMap<InfrastructureService, infrastructureServiceType>()
                    .ForMember(dest => dest.nameSpecified, opt => opt.MapFrom(src => true));
                cfg.CreateMap<infrastructureServiceType, InfrastructureService>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());

                cfg.CreateMap<Environment, environmentType>()
                    .ForMember(dest => dest.typeSpecified, opt => opt.MapFrom(src => true))
                    .ForMember(dest => dest.fingerprint, opt => opt.Ignore());
                cfg.CreateMap<environmentType, Environment>();

                cfg.CreateMap<ProductIdentity, productIdentityType>()
                    .ReverseMap();

                cfg.CreateMap<Property, propertyType>()
                    .ReverseMap();

                cfg.CreateMap<ProvisionedZone, provisionedZoneType>()
                    .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.SifId));
                cfg.CreateMap<provisionedZoneType, ProvisionedZone>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.SifId, opt => opt.MapFrom(src => src.id));

                cfg.CreateMap<Right, rightType>()
                    .ReverseMap();

                cfg.CreateMap<Service, serviceType>();
                cfg.CreateMap<serviceType, Service>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());

                cfg.CreateMap<Zone, zoneType>()
                    .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.SifId));
                cfg.CreateMap<zoneType, Zone>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.SifId, opt => opt.MapFrom(src => src.id));

                cfg.CreateMap<PhaseState, stateType>()
                    .ForMember(dest => dest.createdSpecified, opt => opt.MapFrom(src => src.Created != null))
                    .ForMember(dest => dest.lastModifiedSpecified, opt => opt.MapFrom(src => src.LastModified != null));
                cfg.CreateMap<stateType, PhaseState>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());

                cfg.CreateMap<Phase, phaseType>();
                cfg.CreateMap<phaseType, Phase>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());

                cfg.CreateMap<Initialization, initializationType>()
                    .ReverseMap();

                cfg.CreateMap<Job, jobType>()
                    .ForMember(dest => dest.createdSpecified, opt => opt.MapFrom(src => src.Created != null))
                    .ForMember(dest => dest.lastModifiedSpecified, opt => opt.MapFrom(src => src.LastModified != null))
                    .ForMember(dest => dest.stateSpecified, opt => opt.MapFrom(src => src.State != null))
                    .ForMember(dest => dest.timeout, opt => opt.MapFrom(src => XmlConvert.ToString(src.Timeout)));
                cfg.CreateMap<jobType, Job>()
                    .ForMember(dest => dest.Timeout, opt => opt.MapFrom(src => XmlConvert.ToTimeSpan(src.timeout)));

                cfg.CreateMap<ResponseError, errorType>()
                    .ReverseMap();

                cfg.CreateMap<CreateStatus, createType>()
                    .ReverseMap();

                cfg.CreateMap<DeleteStatus, deleteStatus>()
                    .ReverseMap();

                cfg.CreateMap<UpdateStatus, updateType>()
                    .ReverseMap();

                cfg.CreateMap<MultipleCreateResponse, createResponseType>()
                    .ForMember(dest => dest.creates, opt => opt.MapFrom(src => src.StatusRecords));
                cfg.CreateMap<createResponseType, MultipleCreateResponse>()
                    .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.creates));

                cfg.CreateMap<MultipleDeleteResponse, deleteResponseType>()
                    .ForMember(dest => dest.deletes, opt => opt.MapFrom(src => src.StatusRecords));
                cfg.CreateMap<deleteResponseType, MultipleDeleteResponse>()
                    .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.deletes));

                cfg.CreateMap<MultipleUpdateResponse, updateResponseType>()
                    .ForMember(dest => dest.updates, opt => opt.MapFrom(src => src.StatusRecords));
                cfg.CreateMap<updateResponseType, MultipleUpdateResponse>()
                    .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.updates));

                cfg.CreateMap<deleteIdType[], ICollection<string>>()
                    .ReverseMap();

                cfg.CreateMap<deleteRequestType, MultipleDeleteRequest>()
                    .ForMember(dest => dest.RefIds, opt => opt.MapFrom(src => src.deletes));
            });

            config.AssertConfigurationIsValid();
            Mapper = config.CreateMapper();
        }

        /// <summary>
        /// Map a source object to a destination object.
        /// </summary>
        /// <typeparam name="TSource">Type of the source object.</typeparam>
        /// <typeparam name="TDestination">Type of the destination object.</typeparam>
        /// <param name="source">Source object.</param>
        /// <returns>Destination object.</returns>
        public static TDestination CreateInstance<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// Map a source collection to a destination collection.
        /// </summary>
        /// <typeparam name="TSource">Type of the source collection.</typeparam>
        /// <typeparam name="TDestination">Type of the destination collection.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <returns>Destination collection.</returns>
        public static ICollection<TDestination> CreateInstances<TSource, TDestination>(IEnumerable<TSource> source)
        {
            return Mapper.Map<IEnumerable<TSource>, ICollection<TDestination>>(source);
        }
    }
}