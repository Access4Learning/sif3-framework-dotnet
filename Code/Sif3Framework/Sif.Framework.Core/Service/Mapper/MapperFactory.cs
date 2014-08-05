/*
 * Copyright 2014 Systemic Pty Ltd
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
using Sif.Specification.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Service.Mapper
{

    public static class MapperFactory
    {

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

        class RightsConverter : ITypeConverter<rightType[], IDictionary<RightType, Right>>
        {

            public IDictionary<RightType, Right> Convert(ResolutionContext context)
            {
                ICollection<Right> values = AutoMapper.Mapper.Map<rightType[], ICollection<Right>>((rightType[])context.SourceValue);
                IDictionary<RightType, Right> rights = new Dictionary<RightType, Right>();

                foreach (Right right in values)
                {
                    rights.Add(right.Type, right);
                }

                return rights;
            }

        }

        class ZonePropertiesFlattenResolver : ValueResolver<Zone, propertiesType>
        {

            protected override propertiesType ResolveCore(Zone source)
            {
                propertiesType propertiesType = null;

                if (source != null && source.Properties != null && source.Properties.Count > 0)
                {
                    Property property = source.Properties.Values.ElementAt(0);
                    propertiesType = new propertiesType();
                    propertiesType.property = AutoMapper.Mapper.Map<propertyType>(property);
                }

                return propertiesType;
            }

        }

        class ZonePropertiesUnflattenResolver : ValueResolver<zoneType, IDictionary<string, Property>>
        {

            protected override IDictionary<string, Property> ResolveCore(zoneType source)
            {
                IDictionary<string, Property> properties = null;

                if (source != null && source.properties != null && source.properties.property != null && string.IsNullOrWhiteSpace(source.properties.property.name))
                {
                    Property property = AutoMapper.Mapper.Map<Property>(source.properties.property);
                    properties = new Dictionary<string, Property> { { property.Name, property } };
                }

                return properties;
            }

        }

        static MapperFactory()
        {
            AutoMapper.Mapper.CreateMap<ApplicationInfo, applicationInfoType>();
            AutoMapper.Mapper.CreateMap<applicationInfoType, ApplicationInfo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            AutoMapper.Mapper.CreateMap<Environment, environmentType>()
                .ForMember(dest => dest.infrastructureServices, opt => opt.MapFrom(src => src.InfrastructureServices.Values))
                .ForMember(dest => dest.provisionedZones, opt => opt.MapFrom(src => src.ProvisionedZones.Values));
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
            AutoMapper.Mapper.CreateMap<rightType[], IDictionary<RightType, Right>>()
                .ConvertUsing<RightsConverter>();

            AutoMapper.Mapper.CreateMap<Model.Infrastructure.Service, serviceType>()
                .ForMember(dest => dest.rights, opt => opt.MapFrom(src => src.Rights.Values));
            AutoMapper.Mapper.CreateMap<serviceType, Model.Infrastructure.Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            AutoMapper.Mapper.CreateMap<Zone, zoneType>()
                .ForMember(dest => dest.properties, opt => opt.ResolveUsing<ZonePropertiesFlattenResolver>());
            AutoMapper.Mapper.CreateMap<zoneType, Zone>()
                .ForMember(dest => dest.Properties, opt => opt.ResolveUsing<ZonePropertiesUnflattenResolver>());

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
