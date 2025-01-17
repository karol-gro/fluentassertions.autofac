using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Autofac.Core;
using Xunit;

namespace FluentAssertions.Autofac
{
    // ReSharper disable InconsistentNaming
    public class RegistrationAssertions_Should
    {
        [Fact]
        public void Register_Named()
        {
            var containerShouldHave = GetSut(builder =>
                builder.RegisterType<NamedDummy>().Named<IDisposable>("Dummy")
            );

            containerShouldHave.Registered<NamedDummy>()
                .Named<IDisposable>("Dummy")
                .Keyed<IDisposable>("Dummy");
        }

        [Fact]
        public void Register_Keyed()
        {
            var containerShouldHave = GetSut(builder =>
                builder.RegisterType<KeyedDummy>().Keyed<IComparable>(42)
            );

            containerShouldHave.Registered<KeyedDummy>()
                .Keyed<IComparable>(42);
        }

        [Fact]
        public void Register_Lifetime()
        {
            var containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().SingleInstance());
            containerShouldHave.Registered<Dummy>().SingleInstance();

            containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().InstancePerDependency());
            containerShouldHave.Registered<Dummy>().InstancePerDependency();

            containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().InstancePerLifetimeScope());
            containerShouldHave.Registered<Dummy>().InstancePerLifetimeScope();

            containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().InstancePerMatchingLifetimeScope());
            containerShouldHave.Registered<Dummy>().InstancePerMatchingLifetimeScope();

            containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().InstancePerRequest());
            containerShouldHave.Registered<Dummy>().InstancePerRequest();

            containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().InstancePerOwned<IDisposable>());
            containerShouldHave.Registered<Dummy>().InstancePerOwned<IDisposable>();
        }

        [Fact]
        public void Register_Ownership()
        {
            var containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().ExternallyOwned());
            containerShouldHave.Registered<Dummy>()
                .ExternallyOwned()
                .Owned(InstanceOwnership.ExternallyOwned);

            containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().OwnedByLifetimeScope());
            containerShouldHave.Registered<Dummy>()
                .OwnedByLifetimeScope()
                .Owned(InstanceOwnership.OwnedByLifetimeScope);
        }

        [Fact]
        public void Register_AutoActivate()
        {
            var containerShouldHave = GetSut(builder => builder.RegisterType<Dummy>().As<IDisposable>().AutoActivate());
            containerShouldHave.Registered<Dummy>().As<IDisposable>().AutoActivate();
        }

        [Fact]
        public void Register_parameters()
        {
            var builder = new ContainerBuilder();

            const string paramName = "name";
            const string paramValue = "Name";

            builder.RegisterType<Dummy>()
                .As<IDisposable>()
                .WithParameter(paramName, paramValue)
                .WithParameter(new NamedParameter(paramName, paramValue))
                .WithParameter(new PositionalParameter(0, paramValue));

            var container = builder.Build();
            container.Should().Have()
                .Registered<Dummy>()
                .As<IDisposable>()
                .WithParameter(paramName, paramValue)
                .WithParameter(new NamedParameter(paramName, paramValue))
                .WithParameter(new PositionalParameter(0, paramValue))
                ;
        }

        [Fact]
        public void Register_parametersMatchingPredicate()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Dummy>()
                .As<IDisposable>()
                .WithParameter(new TypedParameter(typeof(string), "stringValue"))
                .WithParameter(new TypedParameter(typeof(int), "intValue"));

            static bool IsString(Parameter p) => p is TypedParameter tp
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                                                 && tp.Type == typeof(string) && tp.Value == "stringValue";
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast

            static bool IsTyped(Parameter p)
            {
                return p is TypedParameter;
            }

            var container = builder.Build();
            container.Should().Have()
                .Registered<Dummy>()
                .As<IDisposable>()
                .WithParameter(IsString)
                .WithParameter(IsTyped, 2);
        }

        private static ContainerRegistrationAssertions GetSut(Action<ContainerBuilder> arrange = null)
        {
            var builder = new ContainerBuilder();
            arrange?.Invoke(builder);
            return builder.Build().Should().Have();
        }

        [ExcludeFromCodeCoverage]
        // ReSharper disable ClassNeverInstantiated.Local
        private class Dummy : IDisposable
        {
            public void Dispose() { }
        }

        [ExcludeFromCodeCoverage]
        private class NamedDummy : IDisposable
        {
            public void Dispose() { }
        }

        [ExcludeFromCodeCoverage]
        private class KeyedDummy : IComparable
        {
            public int CompareTo(object obj) { return 42; }
        }

        [ExcludeFromCodeCoverage]
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedType.Local
        private class ParameterizedDummy : Dummy
        {
            public ParameterizedDummy(string name) { Name = name; }

            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Name { get; }
        }
    }
}
