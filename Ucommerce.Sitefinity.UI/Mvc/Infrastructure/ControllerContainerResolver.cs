using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;

namespace Ucommerce.Sitefinity.UI.Mvc.Infrastructure
{
    /// <summary>
    /// This class contains logic for locating assemblies containing MVC widget controllers.
    /// </summary>
    internal class ControllerContainerResolver
    {
        #region Public members

        /// <summary>
        /// Gets the assemblies that are marked as controller containers with the <see cref="ControllerContainerAttribute"/> attribute.
        /// </summary>
        public static IEnumerable<Assembly> RetrieveAssemblies()
        {
            IEnumerable<string> assemblyFileNames;

            assemblyFileNames = RetrieveAssembliesFileNames().Distinct().ToArray();

            IDictionary<string, Task<Assembly>> retrieveAssemblyTasks = new Dictionary<string, Task<Assembly>>();

            foreach (string assemblyFileName in assemblyFileNames)
            {
                retrieveAssemblyTasks.Add(assemblyFileName, RetrieveAssemblyAsync(assemblyFileName));
            }

            Task.WaitAll(retrieveAssemblyTasks.Values.ToArray());

            IEnumerable<Assembly> result = retrieveAssemblyTasks.Values
                .Select(v => v.Result)
                .Where(a => a != null);

            return result;
        }

        #endregion

        #region Private members

        /// <summary>
        /// Loads the assembly file into the current application domain.
        /// </summary>
        /// <param name="assemblyFileName">Name of the assembly file.</param>
        /// <returns>The loaded assembly</returns>
        private static Assembly LoadAssembly(string assemblyFileName)
        {
            return Assembly.LoadFrom(assemblyFileName);
        }

        /// <summary>
        /// Gets the assemblies file names that will be inspected for controllers.
        /// </summary>
        private static IEnumerable<string> RetrieveAssembliesFileNames()
        {
            var controllerAssemblyPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "bin");
            return Directory.EnumerateFiles(controllerAssemblyPath, "*.dll", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Determines whether the given <paramref name="assemblyFileName"/> is an assembly file that is marked as <see cref="ContainerControllerAttribute"/>.
        /// </summary>
        /// <param name="assemblyFileName">Filename of the assembly.</param>
        /// <returns>True if the given file name is of an assembly file that is marked as <see cref="ContainerControllerAttribute"/>, false otherwise.</returns>
        private static bool IsControllerContainer(string assemblyFileName)
        {
            return IsMarkedAssembly<ControllerContainerAttribute>(assemblyFileName);
        }

        private static Task<Assembly> RetrieveAssemblyAsync(string assemblyFileName)
        {
            return Task.Run(() => RetrieveAssembly(assemblyFileName));
        }

        private static Assembly RetrieveAssembly(string assemblyFileName)
        {
            if (IsControllerContainer(assemblyFileName))
            {
                Assembly assembly = LoadAssembly(assemblyFileName);

                return assembly;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified assembly file name is marked with a given attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="assemblyFileName">File name of the assembly.</param>
        /// <returns>True if the assembly has the given attribute.</returns>
        private static bool IsMarkedAssembly<TAttribute>(string assemblyFileName)
            where TAttribute : Attribute
        {
            if (assemblyFileName == null)
                return false;

            bool result;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
            try
            {
                try
                {
                    var reflOnlyAssembly = Assembly.ReflectionOnlyLoadFrom(assemblyFileName);

                    result = reflOnlyAssembly != null &&
                            reflOnlyAssembly.GetCustomAttributesData()
                                .Any(d => d.Constructor.DeclaringType.AssemblyQualifiedName == typeof(TAttribute).AssemblyQualifiedName);
                }
                catch (IOException)
                {
                    // We might not be able to load some .DLL files as .NET assemblies. Those files cannot contain controllers.
                    result = false;
                }
                catch (BadImageFormatException)
                {
                    result = false;
                }
            }
            finally
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
            }

            return result;
        }

        private static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assignedWithPolicy = AppDomain.CurrentDomain.ApplyPolicy(args.Name);

            return Assembly.ReflectionOnlyLoad(assignedWithPolicy);
        }

        #endregion
    }
}
