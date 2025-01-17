using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Module = Autofac.Module;

namespace FluentAssertions.Autofac
{
    /// <inheritdoc />
    /// <summary>
    ///     Contains a number of methods to assert that an <see cref="T:FluentAssertions.Autofac.MockContainerBuilder" /> is in
    ///     the expected state.
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerNonUserCode]
#endif
    public class BuilderWrapperAssertions : ReferenceTypeAssertions<BuilderWrapper, BuilderWrapperAssertions>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Returns the type of the subject the assertion applies on.
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected override string Identifier => nameof(BuilderWrapper);

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuilderWrapperAssertions" /> class.
        /// </summary>
        /// <param name="subject"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BuilderWrapperAssertions(BuilderWrapper subject) : base(subject)
        {
            _modules = Subject.GetModules().ToList();
        }

        /// <summary>
        ///     Asserts that the specified module type has been registered on the current <see cref="BuilderWrapper" />.
        /// </summary>
        /// <typeparam name="TModule">The module type</typeparam>
        public void RegisterModule<TModule>() where TModule : Module, new()
        {
            RegisterModule(typeof(TModule));
        }

        /// <summary>
        ///     Asserts that the specified module type has been registered on the current <see cref="BuilderWrapper" />.
        /// </summary>
        /// <param name="moduleType">The module type</param>
        public void RegisterModule(Type moduleType)
        {
            Traverse();
            var module = _modules.FirstOrDefault(m => m.GetType() == moduleType);
            Execute.Assertion
                .ForCondition(module != null)
                .FailWith($"Module '{moduleType}' should be registered but it was not.");
        }

        /// <summary>
        ///     Asserts that the modules contained in the specified assembly have been registered on the current
        ///     <see cref="BuilderWrapper" />.
        /// </summary>
        /// <param name="assembly">The module assembly</param>
        [Obsolete("Use 'RegisterAssemblyModules'")]
        public void RegisterModulesIn(Assembly assembly)
        {
            RegisterModulesOf(assembly);
        }

        /// <summary>
        ///     Asserts that the modules contained in the specified assembly have been registered on the current
        ///     <see cref="BuilderWrapper" />.
        /// </summary>
        /// <param name="assembly">The module assembly</param>
        /// <param name="assemblies">More assemblies to assert</param>
        public void RegisterAssemblyModules(Assembly assembly, params Assembly[] assemblies)
        {
            Enumerable.Repeat(assembly, 1)
                .Concat(assemblies)
                .ToList()
                .ForEach(RegisterModulesOf);
        }

        private void RegisterModulesOf(Assembly assembly)
        {
            var moduleTypes = assembly.GetTypes().Where(t => typeof(Module).IsAssignableFrom(t)).ToList();
            moduleTypes.ForEach(RegisterModule);
        }


        private bool _traversed;
        private readonly List<Module> _modules;

        private void Traverse()
        {
            if (_traversed)
            {
                return;
            }

            var wrapper = new BuilderWrapper();
            _modules.ForEach(module => wrapper.Load(module));
            var traversedModules = wrapper.GetModules();
            _modules.AddRange(traversedModules);

            _traversed = true;
        }
    }
}
