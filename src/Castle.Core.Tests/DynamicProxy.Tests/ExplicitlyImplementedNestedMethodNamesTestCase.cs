// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Reflection;

	using NUnit.Framework;

	using INestedSharedNameFromA = Interfaces.OuterWrapper.InnerWrapperA.ISharedName;
	using INestedSharedNameFromB = Interfaces.OuterWrapper.InnerWrapperB.ISharedName;
	using INestedSharedNameFromC = Interfaces.OuterWrapper.InnerWrapperC.ISharedName;

	[TestFixture]
	public class ExplicitlyImplementedNestedMethodNamesTestCase
	{
		[Test]
		public void DynamicProxy_includes_namespace_and_declaring_type_and_type_name_in_names_of_explicitly_implemented_methods()
		{
			var a = typeof(INestedSharedNameFromA);
			var b = typeof(INestedSharedNameFromB);
			var c = typeof(INestedSharedNameFromC);

			var proxy = new ProxyGenerator().CreateInterfaceProxyWithoutTarget(
				interfaceToProxy: a,
				additionalInterfacesToProxy: new[] { b, c },
				interceptors: new StandardInterceptor());

			var implementingType = proxy.GetType();

			AssertNamingSchemeOfExplicitlyImplementedMethods(b, c, implementingType);
		}

		private void AssertNamingSchemeOfExplicitlyImplementedMethods(Type b, Type c, Type implementingType)
		{
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			// The assertions at the end of this method only make sense if certain preconditions
			// are met. We verify those using NUnit assumptions:

			// We require two interface types that have the same name and a method named `M` each:
			Assume.That(b.IsInterface);
			Assume.That(c.IsInterface);
			Assume.That(b.Name == c.Name);
			Assume.That(b.GetMethod("M") != null);
			Assume.That(c.GetMethod("M") != null);

			// We also need a type that implements the above interfaces:
			Assume.That(b.IsAssignableFrom(implementingType));
			Assume.That(c.IsAssignableFrom(implementingType));

			// If all of the above conditions are met, we expect the methods from the interfaces
			// to be implemented explicitly. For our purposes, this means that they follow the
			// naming scheme `<namespace>.<parent types>.<type>.M`:
			Assert.NotNull(implementingType.GetMethod($"{b.Namespace}.{b.DeclaringType.DeclaringType.Name}.{b.DeclaringType.Name}.{b.Name}.M", bindingFlags));
			Assert.NotNull(implementingType.GetMethod($"{c.Namespace}.{b.DeclaringType.DeclaringType.Name}.{b.DeclaringType.Name}.{c.Name}.M", bindingFlags));
		}
	}
}
