/*
 * Copyright 2016 Systemic Pty Ltd
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
using System;
using System.Collections.Generic;
using System.Xml;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Mapper
{

    public static class MapperFactory
    {

        class InfrastructureServicesConverter : ITypeConverter<infrastructureServiceType[], IDictionary<InfrastructureServiceNames, InfrastructureService>>
        {

            public IDictionary<InfrastructureServiceNames, InfrastructureService> Convert(ResolutionContext context)
            {
                ICollection<InfrastructureService> values = AutoMapper.Mapper.Map<infrastructureServiceType[], ICollection<InfrastructureService>>((infrastructureServiceType[])context.SourceValue);
                IDictionary<InfrastructureServiceNames, InfrastructureService> infrastructureServices = new Dictionary<InfrastructureServiceNames, InfrastructureService>();

                foreach (InfrastructureService infrastructureService in values)
                {
                    infrastructureServices.Add(infrastructureService.Name, infrastructureService);
                }

                return infrastructureServices;
            }

        }

        class PropertiesConverter : ITypeConverter<propertyType[], IDictionary<string, Property>>
        {

            public IDictionary<string, Property> Convert(ResolutionContext context)
            {
                ICollection<Property> values = AutoMapper.Mapper.Map<propertyType[], ICollection<Property>>((propertyType[])context.SourceValue);
                IDictionary<string, Property> properties = new Dictionary<string, Property>();

                foreach (Property property in values)
                {
                    properties.Add(property.Name, property);
                }

                return properties;
            }

        }

        class ProvisionedZonesConverter : ITypeConverter<provisionedZoneType[], IDictionary<string, ProvisionedZone>>
        {

            public IDictionary<string, ProvisionedZone> Convert(ResolutionContext context)
            {
                ICollection<ProvisionedZone> values = AutoMapper.Mapper.Map<provisionedZoneType[], ICollection<ProvisionedZone>>((provisionedZoneType[])context.SourceValue);
                IDictionary<string, ProvisionedZone> provisionedZones = new Dictionary<string, ProvisionedZone>();

                foreach (ProvisionedZone provisionedZone in values)
                {
                    provisionedZones.Add(provisionedZone.SifId, provisionedZone);
                }

                return provisionedZones;
            }

        }

        class RightsConverter : ITypeConverter<rightType[], IDictionary<string, Right>>
        {

            public IDictionary<string, Right> Convert(ResolutionContext context)
            {
                ICollection<Right> values = AutoMapper.Mapper.Map<rightType[], ICollection<Right>>((rightType[])context.SourceValue);
                IDictionary<string, Right> rights = new Dictionary<string, Right>();

                foreach (Right right in values)
                {
                    rights.Add(right.Type, right);
                }

                return rights;
            }

        }

        class StatesConverter : ITypeConverter<stateType[], IList<PhaseState>>
        {

            public IList<PhaseState> Convert(ResolutionContext context)
            {
                return new List<PhaseState>(AutoMapper.Mapper.Map<stateType[], ICollection<PhaseState>>((stateType[])context.SourceValue));
            }

        }

        class PhasesConverter : ITypeConverter<phaseType[], IDictionary<string, Phase>>
        {

            public IDictionary<string, Phase> Convert(ResolutionContext context)
            {
                ICollection<Phase> values = AutoMapper.Mapper.Map<phaseType[], ICollection<Phase>>((phaseType[])context.SourceValue);
                IDictionary<string, Phase> phases = new Dictionary<string, Phase>();

                foreach (Phase phase in values)
                {
                    phases.Add(phase.Name, phase);
                }

                return phases;
            }

        }

        class DeleteIdsConverter : ITypeConverter<deleteIdType[], ICollection<string>>
        {

            public ICollection<string> Convert(ResolutionContext context)
            {
                ICollection<string> deleteIds = new List<string>();

                foreach (deleteIdType deleteId in (deleteIdType[])context.SourceValue)
                {
                    deleteIds.Add(deleteId.id);
                }

                return deleteIds;
            }

        }

        static MapperFactory()
        {
            AutoMapper.Mapper.CreateMap<ApplicationInfo, applicationInfoType>();
            AutoMapper.Mapper.CreateMap<applicationInfoType, ApplicationInfo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            AutoMapper.Mapper.CreateMap<InfrastructureService, infrastructureServiceType>()
                .ForMember(dest => dest.nameSpecified, opt => opt.UseValue<bool>(true))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name));
            AutoMapper.Mapper.CreateMap<infrastructureServiceType, InfrastructureService>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name));
            AutoMapper.Mapper.CreateMap<infrastructureServiceType[], IDictionary<InfrastructureServiceNames, InfrastructureService>>()
                .ConvertUsing<InfrastructureServicesConverter>();

            AutoMapper.Mapper.CreateMap<Environment, environmentType>()
                .ForMember(dest => dest.infrastructureServices, opt => opt.MapFrom(src => src.InfrastructureServices.Values))
                .ForMember(dest => dest.provisionedZones, opt => opt.MapFrom(src => src.ProvisionedZones.Values))
                .ForMember(dest => dest.typeSpecified, opt => opt.UseValue<bool>(true))
                .ForMember(dest => dest.fingerprint, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<environmentType, Environment>();

            AutoMapper.Mapper.CreateMap<ProductIdentity, productIdentityType>();
            AutoMapper.Mapper.CreateMap<productIdentityType, ProductIdentity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            AutoMapper.Mapper.CreateMap<Property, propertyType>();
            AutoMapper.Mapper.CreateMap<propertyType, Property>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<propertyType[], IDictionary<string, Property>>()
                .ConvertUsing<PropertiesConverter>();

            AutoMapper.Mapper.CreateMap<ProvisionedZone, provisionedZoneType>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.SifId));
            AutoMapper.Mapper.CreateMap<provisionedZoneType, ProvisionedZone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SifId, opt => opt.MapFrom(src => src.id));
            AutoMapper.Mapper.CreateMap<provisionedZoneType[], IDictionary<string, ProvisionedZone>>()
                .ConvertUsing<ProvisionedZonesConverter>();

            AutoMapper.Mapper.CreateMap<Right, rightType>();
            AutoMapper.Mapper.CreateMap<rightType, Right>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<rightType[], IDictionary<string, Right>>()
                .ConvertUsing<RightsConverter>();

            AutoMapper.Mapper.CreateMap<Model.Infrastructure.Service, serviceType>()
                .ForMember(dest => dest.rights, opt => opt.MapFrom(src => src.Rights.Values));
            AutoMapper.Mapper.CreateMap<serviceType, Model.Infrastructure.Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            AutoMapper.Mapper.CreateMap<Zone, zoneType>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.SifId))
                .ForMember(dest => dest.properties, opt => opt.MapFrom(src => src.Properties.Values));
            AutoMapper.Mapper.CreateMap<zoneType, Zone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SifId, opt => opt.MapFrom(src => src.id));

            AutoMapper.Mapper.CreateMap<PhaseState, stateType>()
                .ForMember(dest => dest.createdSpecified, opt => opt.MapFrom(src => src.Created != null))
                .ForMember(dest => dest.lastModifiedSpecified, opt => opt.MapFrom(src => src.LastModified != null));
            AutoMapper.Mapper.CreateMap<stateType, PhaseState>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<stateType[], IList<PhaseState>>()
                .ConvertUsing<StatesConverter>();

            AutoMapper.Mapper.CreateMap<Phase, phaseType>()
                .ForMember(dest => dest.rights, opt => opt.MapFrom(src => src.Rights.Values))
                .ForMember(dest => dest.statesRights, opt => opt.MapFrom(src => src.StatesRights.Values));
            AutoMapper.Mapper.CreateMap<phaseType, Phase>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<phaseType[], IDictionary<string, Phase>>()
                .ConvertUsing<PhasesConverter>();

            AutoMapper.Mapper.CreateMap<Job, jobType>()
                .ForMember(dest => dest.phases, opt => opt.MapFrom(src => src.Phases.Values))
                .ForMember(dest => dest.createdSpecified, opt => opt.MapFrom(src => src.Created != null))
                .ForMember(dest => dest.lastModifiedSpecified, opt => opt.MapFrom(src => src.LastModified != null))
                .ForMember(dest => dest.stateSpecified, opt => opt.MapFrom(src => src.State != null))
                .ForMember(dest => dest.timeout, opt => opt.MapFrom(src => XmlConvert.ToString(src.Timeout)))
                .ForMember(dest => dest.initialization, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<jobType, Job>()
                .ForMember(dest => dest.Timeout, opt => opt.MapFrom(src => XmlConvert.ToTimeSpan(src.timeout)));

            AutoMapper.Mapper.CreateMap<ResponseError, errorType>();
            AutoMapper.Mapper.CreateMap<errorType, ResponseError>();

            AutoMapper.Mapper.CreateMap<CreateStatus, createType>();
            AutoMapper.Mapper.CreateMap<createType, CreateStatus>();

            AutoMapper.Mapper.CreateMap<DeleteStatus, deleteStatus>();
            AutoMapper.Mapper.CreateMap<deleteStatus, DeleteStatus>();

            AutoMapper.Mapper.CreateMap<UpdateStatus, updateType>();
            AutoMapper.Mapper.CreateMap<updateType, UpdateStatus>();

            AutoMapper.Mapper.CreateMap<MultipleCreateResponse, createResponseType>()
                .ForMember(dest => dest.creates, opt => opt.MapFrom(src => src.StatusRecords));
            AutoMapper.Mapper.CreateMap<createResponseType, MultipleCreateResponse>()
                .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.creates));

            AutoMapper.Mapper.CreateMap<MultipleDeleteResponse, deleteResponseType>()
                .ForMember(dest => dest.deletes, opt => opt.MapFrom(src => src.StatusRecords));
            AutoMapper.Mapper.CreateMap<deleteResponseType, MultipleDeleteResponse>()
                .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.deletes));

            AutoMapper.Mapper.CreateMap<MultipleUpdateResponse, updateResponseType>()
                .ForMember(dest => dest.updates, opt => opt.MapFrom(src => src.StatusRecords));
            AutoMapper.Mapper.CreateMap<updateResponseType, MultipleUpdateResponse>()
                .ForMember(dest => dest.StatusRecords, opt => opt.MapFrom(src => src.updates));

            AutoMapper.Mapper.CreateMap<deleteIdType[], ICollection<string>>()
                .ConvertUsing<DeleteIdsConverter>();

            AutoMapper.Mapper.CreateMap<deleteRequestType, MultipleDeleteRequest>()
                .ForMember(dest => dest.RefIds, opt => opt.MapFrom(src => src.deletes));

            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public static D CreateInstance<S, D>(S source)
        {
            D destination = default(D);
            destination = AutoMapper.Mapper.Map<D>(source);
            return destination;
        }

        public static ICollection<D> CreateInstances<S, D>(IEnumerable<S> source)
        {
            ICollection<D> destination = null;
            destination = AutoMapper.Mapper.Map<IEnumerable<S>, ICollection<D>>(source);
            return destination;
        }

    }

}
