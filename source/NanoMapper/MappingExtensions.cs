﻿using System;
using System.Linq;

namespace NanoMapper {

    /// <summary>
    /// Provides object entry point extensions that exposes the ApplyTo(...) mapping application method.
    /// </summary>
    public static class MappingExtensions {

        /// <summary>
        /// Applies all applicable property values from the source object onto the target object
        /// </summary>
        public static void ApplyTo<TSource, TTarget>(this TSource source, TTarget target)
            where TSource : class where TTarget : class
            => ApplyTo(source, target, Mapper.GlobalInstance);

        /// <summary>
        /// Applies all applicable property values from the source object onto the target object
        /// using the specified mapping overrides.
        /// </summary>
        public static void ApplyTo<TSource, TTarget>(this TSource source, TTarget target, Action<IMappingConfiguration<TSource, TTarget>> configure) where TSource : class where TTarget : class
            => ApplyTo(source, target, Mapper.GlobalInstance, configure);

        /// <summary>
        /// Applies all applicable property values from the source object onto the target object
        /// </summary>
        public static void ApplyTo<TSource, TTarget>(this TSource source, TTarget target, IMapper mapper) where TSource : class where TTarget : class
            => ApplyTo(source, target, mapper, null);

        /// <summary>
        /// Applies all applicable property values from the source object onto the target object
        /// using the specified mapping overrides.
        /// </summary>
        public static void ApplyTo<TSource, TTarget>(this TSource source, TTarget target, IMapper mapper, Action<IMappingConfiguration<TSource, TTarget>> configure)  where TSource : class where TTarget : class {

            var mapping = mapper.Mappings.Values.First() as IMappingConfiguration<TSource, TTarget>; // FIXME

            if (configure != null) {
                configure(mapping);
            }

            ExecuteApplication(source, target, mapping);


            //if (!mapper.Mappings.TryGetValue(new Tuple<Type, Type>(typeof(TSource), typeof(TTarget)), out object mappingConfigEntry)) {

            //}



            //if (source.Equals(target)) {
            //    return;
            //}

            //var sourceType = source.GetType().GetTypeInfo();
            //var targetType = target.GetType().GetTypeInfo();

            //var sourceProps = sourceType.GetProperties().Where(p => p.CanRead);
            //var targetProps = targetType.GetProperties().Where(p => p.CanWrite);
            //var properties = sourceProps.Intersect(targetProps, new ComparePropertiesByNameAndType()).ToList();

            //var sourceFields = sourceType.GetFields();
            //var targetFields = targetType.GetFields();
            //var fields = sourceFields.Intersect(targetFields, new CompareFieldsByNameAndType()).ToList();

            //Copy(source, target, properties, fields);
        }

        private static void ExecuteApplication<TSource, TTarget>(this TSource source, TTarget target, IMappingConfiguration<TSource, TTarget> mapping) where TSource : class where TTarget : class {
            mapping.Execute(source, target);
        }
        
    }

}
