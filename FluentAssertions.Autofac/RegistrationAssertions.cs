using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Lifetime;
using FluentAssertions.Primitives;

namespace FluentAssertions.Autofac
{
    /// <inheritdoc />
    /// <summary>
    ///     Contains a number of methods to assert that an <see cref="T:Autofac.IComponentContext" /> registers value services.
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerNonUserCode]
#endif
    public class RegistrationAssertions : ReferenceTypeAssertions<IComponentContext, RegistrationAssertions>
    {
        /// <summary>
        ///     The type that should be registered on the container
        /// </summary>
        internal readonly Type Type;

        private readonly IComponentRegistration _registration;
        private readonly IList<Parameter> _parameters;

        /// <inheritdoc />
        /// <summary>
        ///     Returns the type of the subject the assertion applies on.
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected override string Identifier => nameof(IComponentContext);

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegistrationAssertions" /> class.
        /// </summary>
        /// <param name="subject">The container</param>
        /// <param name="type">The type that should be registered on the container</param>
        public RegistrationAssertions(IComponentContext subject, Type type) : base(subject)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            _registration = Subject.ComponentRegistry.GetRegistration(Type);
            _parameters = GetParameters(_registration);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegistrationAssertions" /> class.
        /// </summary>
        /// <param name="subject">The container</param>
        /// <param name="registration"></param>
        public RegistrationAssertions(IComponentContext subject, IComponentRegistration registration) : base(subject)
        {
            _registration = registration ?? throw new ArgumentNullException(nameof(registration));
            Type = registration.Activator.LimitType;
            _parameters = GetParameters(_registration);
        }

        /// <summary>
        ///     Asserts that the specified <see typeparamref="TService" /> has been registered on the current
        ///     <see cref="IComponentContext" /> with the specified name.
        /// </summary>
        /// <param name="name">The service name</param>
        /// <typeparam name="TService">The service type</typeparam>
        public RegistrationAssertions Named<TService>(string name)
        {
            return Named(name, typeof(TService));
        }

        /// <summary>
        ///     Asserts that the specified <see paramref="type" /> has been registered on the current <see cref="IComponentContext" />
        ///     with the specified name.
        /// </summary>
        /// <param name="name">The service name</param>
        /// <param name="type">The service type</param>
        public RegistrationAssertions Named(string name, Type type)
        {
            Subject.IsRegisteredWithName(name, type)
                .Should().BeTrue($"Type '{type}' should be registered with name '{name}'");
            return this;
        }

        /// <summary>
        ///     Asserts that the specified <see typeparamref="TService" /> has been registered on the current
        ///     <see cref="IComponentContext" /> with the specified key.
        /// </summary>
        /// <param name="key">The service key</param>
        /// <typeparam name="TService">The service type</typeparam>
        public RegistrationAssertions Keyed<TService>(object key)
        {
            return Keyed(key, typeof(TService));
        }

        /// <summary>
        ///     Asserts that the specified <see typeparamref="TService" /> has been registered on the current
        ///     <see cref="IComponentContext" /> with the specified key.
        /// </summary>
        /// <param name="key">The service key</param>
        /// <param name="type">The service type</param>
        public RegistrationAssertions Keyed(object key, Type type)
        {
            Subject.IsRegisteredWithKey(key, type)
                .Should().BeTrue($"Type '{type}' should be registered with key '{key}'");
            return this;
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as singleton on the current <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions SingleInstance()
        {
            return Lifetime<RootScopeLifetime>()
                .Shared(InstanceSharing.Shared);
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as 'instance per dependency' on the current
        ///     <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions InstancePerDependency()
        {
            return Lifetime<CurrentScopeLifetime>()
                .Shared(InstanceSharing.None);
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as 'instance per lifetime scope' on the current
        ///     <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions InstancePerLifetimeScope()
        {
            return Lifetime<CurrentScopeLifetime>()
                .Shared(InstanceSharing.Shared);
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as 'instance per matching lifetime scope' on the current
        ///     <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions InstancePerMatchingLifetimeScope()
        {
            return Lifetime<MatchingScopeLifetime>()
                .Shared(InstanceSharing.Shared);
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as 'instance per request' on the current
        ///     <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions InstancePerRequest()
        {
            return Lifetime<MatchingScopeLifetime>()
                .Shared(InstanceSharing.Shared);
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as 'instance per owned' of the specified
        ///     <see typeparamref="TService" /> on the current <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions InstancePerOwned<TService>()
        {
            return InstancePerOwned(typeof(TService));
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as 'instance per owned' of the specified
        ///     <see paramref="type" /> on the current <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions InstancePerOwned(Type serviceType)
        {
            // TODO
            return InstancePerMatchingLifetimeScope();
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as 'externally owned' on the current
        ///     <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions ExternallyOwned()
        {
            return Owned(InstanceOwnership.ExternallyOwned);
        }

        /// <summary>
        ///     Asserts that the current service type has been registered as 'owned by lifetime scope' on the current
        ///     <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions OwnedByLifetimeScope()
        {
            return Owned(InstanceOwnership.OwnedByLifetimeScope);
        }

        /// <summary>
        ///     Asserts the current service type has been registered using the specified <see typeparamref="TLifetime" /> on the
        ///     current <see cref="IComponentContext" />.
        /// </summary>
        /// <param name="assert">An optional custom assertion action to execute on the <typeparamref name="TLifetime" /></param>
        /// <typeparam name="TLifetime"></typeparam>
        public RegistrationAssertions Lifetime<TLifetime>(Action<TLifetime> assert = null)
            where TLifetime : IComponentLifetime
        {
            _registration.Lifetime.Should()
                .BeOfType<TLifetime>($"Type '{Type}' should be registered with lifetime '{typeof(TLifetime)}'");
            assert?.Invoke((TLifetime)_registration.Lifetime);
            return this;
        }

        /// <summary>
        ///     Asserts the current service type has been registered using the specified <see cref="InstanceSharing" /> on the
        ///     current <see cref="IComponentContext" />.
        /// </summary>
        /// <param name="sharing">The instance sharing mode</param>
        public RegistrationAssertions Shared(InstanceSharing sharing)
        {
            _registration.Sharing.Should().Be(sharing, $"Type '{Type}' should be shared as '{sharing}'");
            return this;
        }

        /// <summary>
        ///     Asserts the current service type has been registered using the specified <see cref="InstanceOwnership" /> on the
        ///     current <see cref="IComponentContext" />.
        /// </summary>
        /// <param name="ownership">The instance ownership mode</param>
        public RegistrationAssertions Owned(InstanceOwnership ownership)
        {
            _registration.Ownership.Should().Be(ownership, $"Type '{Type}' should be owned '{ownership}'");
            return this;
        }

        /// <summary>
        ///     Asserts the current service type has been registered with auto activation on the current <see cref="IComponentContext" />.
        /// </summary>
        public RegistrationAssertions AutoActivate()
        {
            _registration.AssertAutoActivates(Type);
            return this;
        }

        /// <summary>
        ///     Asserts the current service type has been registered with the specified constructor parameter.
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        public RegistrationAssertions WithParameter(string name, object value)
        {
            return WithParameter(new NamedParameter(name, value));
        }

        /// <summary>
        ///     Asserts the current service type has been registered with the specified constructor parameter.
        /// </summary>
        /// <param name="predicate">
        ///     Must evaluate to <c>true</c> for a parameter for the assertion to pass.
        /// </param>
        /// <param name="matchCount">
        ///     When <c>null</c>, assertion passes when one or more of the parameters matches the
        ///     <paramref name="predicate" />. When set to a value, exactly this number of parameters must match the
        ///     <paramref name="predicate" />.
        /// </param>
        /// <returns></returns>
        public RegistrationAssertions WithParameter(
            Func<Parameter, bool> predicate,
            int? matchCount = null)
        {
            var matchingParams = _parameters.Where(predicate);

            if (matchCount.HasValue)
            {
                matchingParams
                    .Count()
                    .Should()
                    .Be(
                        matchCount.Value,
                        $"exactly {matchCount.Value} parameter(s) matching a predicate should have been registered");
            }
            else
            {
                matchingParams
                    .Any()
                    .Should()
                    .BeTrue("at least one parameter matching a predicate should have been registered");
            }

            return this;
        }

        /// <summary>
        ///     Asserts the current service type has been registered with the specified constructor parameter.
        /// </summary>
        /// <param name="param">The parameter</param>
        public RegistrationAssertions WithParameter(NamedParameter param)
        {
            var p = _parameters
                .OfType<NamedParameter>()
                .FirstOrDefault(np => np.Name == param.Name);

            p.Should().NotBeNull($"Parameter '{param.Name}' should have been registered.");
            p?.Value.Should().BeEquivalentTo(param.Value,
                $"Parameter '{param.Name}' should have been registered with value '{param.Value}'.");

            return this;
        }

        /// <summary>
        ///     Asserts the current service type has been registered with the specified constructor parameter.
        /// </summary>
        /// <param name="param">The parameter</param>
        public RegistrationAssertions WithParameter(PositionalParameter param)
        {
            var p = _parameters
                .OfType<PositionalParameter>()
                .FirstOrDefault(pp => pp.Position == param.Position);

            p.Should().NotBeNull($"Parameter should have been registered at position '{param.Position}'.");
            p?.Value.Should().BeEquivalentTo(param.Value,
                $"Parameter at position '{param.Position}' should have been registered with value '{param.Value}'.");

            return this;
        }

        private static IList<Parameter> GetParameters(IComponentRegistration registration)
        {
            var parameters = new List<Parameter>();

            if (!(registration.Activator is ReflectionActivator activator))
            {
                return parameters;
            }

            const string fieldName = "_defaultParameters";
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                           BindingFlags.Static;
            var field = activator.GetType().GetField(fieldName, bindFlags);
            if (field == null)
            {
                return parameters;
            }

            if (field.GetValue(activator) is Parameter[] p)
            {
                parameters.AddRange(p);
            }

            return parameters;
        }
    }
}
