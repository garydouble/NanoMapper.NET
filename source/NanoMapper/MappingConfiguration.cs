﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NanoMapper {

    /// <summary>
    /// Represents a mapping configuration object that is used to configure object property mappings.
    /// </summary>
    public interface IMappingConfiguration<TSource, TTarget> where TSource : class where TTarget : class {

        /// <summary>
        /// Maps the given property from source to target where
        /// both source and target contain compatible property
        /// applications.
        /// </summary>
        IMappingConfiguration<TSource, TTarget> Property<TResult>(Expression<Func<TTarget, TResult>> propertyExpression) where TResult : class;

        /// <summary>
        /// Maps the given property from source to target
        /// using the specified @translationFunc to perform
        /// mapping.
        /// </summary>
        IMappingConfiguration<TSource, TTarget> Property<TResult>(Expression<Func<TTarget, TResult>> propertyExpression, Func<TSource, TResult> translationFunc) where TResult : class;

        /// <summary>
        /// Specifies that this property should be ignored.
        /// </summary>
        IMappingConfiguration<TSource, TTarget> Ignore<TResult>(Expression<Func<TTarget, TResult>> propertyExpression) where TResult : class;

        /// <summary>
        /// Executes the mapping application based on the current mapping configuration.
        /// </summary>
        void Execute(TSource source, TTarget target);
    }


    /// <inheritdoc cref="IMappingConfiguration{TSource,TTarget}" />
    public class MappingConfiguration<TSource, TTarget> : IMappingConfiguration<TSource, TTarget> where TSource : class where TTarget : class {

        public IMappingConfiguration<TSource, TTarget> Property<TResult>(Expression<Func<TTarget, TResult>> propertyExpression) where TResult : class {
            var propertyInfo = ExtractPropertyInfoFromPropertyExpression(propertyExpression);

            TResult translate(TSource source) => (TResult)propertyInfo.GetValue(source);

            PropertyMappings[propertyInfo] = (Func<TSource, TResult>)translate;

            return this;
        }

        public IMappingConfiguration<TSource, TTarget> Property<TResult>(Expression<Func<TTarget, TResult>> propertyExpression, Func<TSource, TResult> translationFunc) where TResult : class {
            var propertyInfo = ExtractPropertyInfoFromPropertyExpression(propertyExpression);

            if (PropertyMappings.ContainsKey(propertyInfo)) {
                PropertyMappings[propertyInfo] = translationFunc;
            }
            else {
                PropertyMappings.Add(propertyInfo, translationFunc);
            }

            return this;
        }

        public IMappingConfiguration<TSource, TTarget> Ignore<TResult>(Expression<Func<TTarget, TResult>> propertyExpression) where TResult : class {
            var propertyInfo = ExtractPropertyInfoFromPropertyExpression(propertyExpression);

            PropertyMappings.Remove(propertyInfo);

            return this;
        }

        public void Execute(TSource source, TTarget target) {
            foreach (var mapping in PropertyMappings) {
                var translate = (Func<TSource, object>)mapping.Value;
                mapping.Key.SetValue(target, translate(source));
            }
        }

        private static PropertyInfo ExtractPropertyInfoFromPropertyExpression<TResult>(Expression<Func<TTarget, TResult>> propertyExpression) {
            
            if (propertyExpression.Body.NodeType == ExpressionType.MemberAccess) {
                return (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
            }
            
            throw new InvalidOperationException("Property expression must be a valid property reference");
        }

        private readonly IDictionary<PropertyInfo, object> PropertyMappings = new Dictionary<PropertyInfo, object>();
    }


    public class MappingConfigurationKey : Tuple<Type, Type> {
        public MappingConfigurationKey(Type sourceType, Type targetType)
            : base(sourceType, targetType) { }

    }


}
