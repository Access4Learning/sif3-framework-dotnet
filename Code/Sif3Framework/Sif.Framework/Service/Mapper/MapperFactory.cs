/*
 * Copyright 2020 Systemic Pty Ltd
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
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Mapper
{
    /// <summary>
    /// Factory class for managing object to object mappings.
    /// </summary>
    public static class MapperFactory
    {
        private static readonly IMapper Mapper;

        private class InfrastructureServicesConverter : ITypeConverter<infrastructureServiceType[], IDictionary<InfrastructureServiceNames, InfrastructureService>>
        {
            public IDictionary<InfrastructureServiceNames, InfrastructureService> Convert(infrastructureServiceType[] source, IDictionary<InfrastructureServiceNames, InfrastructureService> destination, ResolutionContext context)
            {
                ICollection<InfrastructureService> values = Mapper.Map<infrastructureServiceType[], ICollection<InfrastructureService>>(source);
                IDictionary<InfrastructureServiceNames, InfrastructureService> infrastructureServices = new Dictionary<InfrastructureServiceNames, InfrastructureService>();

                foreach (InfrastructureService infrastructureService in values)
                {
                    infrastructureServices.Add(infrastructureService.Name, infrastructureService);
                }

                return infrastructureServices;
            }
        }

        private class PropertiesConverter : ITypeConverter<propertyType[], IDictionary<string, Property>>
        {
            public IDictionary<string, Property> Convert(propertyType[] source, IDictionary<string, Property> destination, ResolutionContext context)
            {
                ICollection<Property> values = Mapper.Map<propertyType[], ICollection<Property>>(source);
                IDictionary<string, Property> properties = new Dictionary<string, Property>();

                foreach (Property property in values)
                {
                    properties.Add(property.Name, property);
                }

                return properties;
            }
        }

        private class ProvisionedZonesConverter : ITypeConverter<provisionedZoneType[], IDictionary<string, ProvisionedZone>>
        {
            public IDictionary<string, ProvisionedZone> Convert(provisionedZoneType[] source, IDictionary<string, ProvisionedZone> destination, ResolutionContext context)
            {
                ICollection<ProvisionedZone> values = Mapper.Map<provisionedZoneType[], ICollection<ProvisionedZone>>(source);
                IDictionary<string, ProvisionedZone> provisionedZones = new Dictionary<string, ProvisionedZone>();

                foreach (ProvisionedZone provisionedZone in values)
                {
                    provisionedZones.Add(provisionedZone.SifId, provisionedZone);
                }

                return provisionedZones;
            }
        }

        private class RightsConverter : ITypeConverter<rightType[], IDictionary<string, Right>>
        {
            public IDictionary<string, Right> Convert(rightType[] source, IDictionary<string, Right> destination, ResolutionContext context)
            {
                ICollection<Right> values = Mapper.Map<rightType[], ICollection<Right>>(source);
                IDictionary<string, Right> rights = new Dictionary<string, Right>();

                foreach (Right right in values)
                {
                    rights.Add(right.Type, right);
                }

                return rights;
            }
        }

        private class PhasesConverter : ITypeConverter<phaseType[], IDictionary<string, Phase>>
        {
            public IDictionary<string, Phase> Convert(phaseType[] source, IDictionary<string, Phase> destination, ResolutionContext context)
            {
                ICollection<Phase> values = Mapper.Map<phaseType[], ICollection<Phase>>(source);
                IDictionary<string, Phase> phases = new Dictionary<string, Phase>();

                foreach (Phase phase in values)
                {
                    phases.Add(phase.Name, phase);
                }

                return phases;
            }
        }

        static MapperFactory()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationInfo, applicationInfoType>()
                    .ReverseMap();

                cfg.CreateMap<InfrastructureService, infrastructureServiceType>()
                    .ForMember(dest => dest.nameSpecified, opt => opt.MapFrom(src => true))
                    .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name));
                cfg.CreateMap<infrastructureServiceType, InfrastructureService>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name));
                cfg.CreateMap<infrastructureServiceType[], IDictionary<InfrastructureServiceNames, InfrastructureService>>()
                    .ConvertUsing(new InfrastructureServicesConverter());

                cfg.CreateMap<Environment, environmentType>()
                    .ForMember(dest => dest.infrastructureServices, opt => opt.MapFrom(src => src.InfrastructureServices.Values))
                    .ForMember(dest => dest.provisionedZones, opt => opt.MapFrom(src => src.ProvisionedZones.Values))
                    .ForMember(dest => dest.typeSpecified, opt => opt.MapFrom(src => true))
                    .ForMember(dest => dest.fingerprint, opt => opt.Ignore());
                cfg.CreateMap<environmentType, Environment>();

                cfg.CreateMap<ProductIdentity, productIdentityType>()
                    .ReverseMap();

                cfg.CreateMap<Property, propertyType>()
                    .ReverseMap();
                cfg.CreateMap<propertyType[], IDictionary<string, Property>>()
                    .ConvertUsing(new PropertiesConverter());

                cfg.CreateMap<ProvisionedZone, provisionedZoneType>()
                    .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.SifId));
                cfg.CreateMap<provisionedZoneType, ProvisionedZone>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.SifId, opt => opt.MapFrom(src => src.id));
                cfg.CreateMap<provisionedZoneType[], IDictionary<string, ProvisionedZone>>()
                    .ConvertUsing(new ProvisionedZonesConverter());

                cfg.CreateMap<Right, rightType>()
                    .ReverseMap();
                cfg.CreateMap<rightType[], IDictionary<string, Right>>()
                    .ConvertUsing(new RightsConverter());

                cfg.CreateMap<Model.Infrastructure.Service, serviceType>()
                    .ForMember(dest => dest.rights, opt => opt.MapFrom(src => src.Rights.Values));
                cfg.CreateMap<serviceType, Model.Infrastructure.Service>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());

                cfg.CreateMap<Zone, zoneType>()
                    .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.SifId))
                    .ForMember(dest => dest.properties, opt => opt.MapFrom(src => src.Properties.Values));
                cfg.CreateMap<zoneType, Zone>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.SifId, opt => opt.MapFrom(src => src.id));

                cfg.CreateMap<PhaseState, stateType>()
                    .ForMember(dest => dest.createdSpecified, opt => opt.MapFrom(src => src.Created != null))
                    .ForMember(dest => dest.lastModifiedSpecified, opt => opt.MapFrom(src => src.LastModified != null));
                cfg.CreateMap<stateType, PhaseState>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());

                cfg.CreateMap<Phase, phaseType>()
                    .ForMember(dest => dest.rights, opt => opt.MapFrom(src => src.Rights.Values))
                    .ForMember(dest => dest.statesRights, opt => opt.MapFrom(src => src.StatesRights.Values));
                cfg.CreateMap<phaseType, Phase>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<phaseType[], IDictionary<string, Phase>>()
                    .ConvertUsing(new PhasesConverter());

                cfg.CreateMap<Initialization, initializationType>()
                    .ReverseMap();

                cfg.CreateMap<Job, jobType>()
                    .ForMember(dest => dest.phases, opt => opt.MapFrom(src => src.Phases.Values))
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