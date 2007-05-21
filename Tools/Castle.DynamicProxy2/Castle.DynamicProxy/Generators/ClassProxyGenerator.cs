// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

using System.Collections.Generic;

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections;
#if DOTNET2
	using System.Collections.Generic;
#endif
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;
	using System.Xml.Serialization;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using System.Collections.Specialized;
	using System.Runtime.Serialization;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Serialization;

	/// <summary>
	/// 
	/// </summary>
	[CLSCompliant(false)]
	public class ClassProxyGenerator : BaseProxyGenerator
	{
		bool delegateToBaseGetObjectData = false;
		
		public ClassProxyGenerator(ModuleScope scope, Type targetType)
			: base(scope, targetType)
		{
			CheckNotGenericTypeDefinition (targetType, "targetType");
		}

		public Type GenerateCode(Type[] interfaces, ProxyGenerationOptions options)
		{
			CheckNotGenericTypeDefinitions (interfaces, "interfaces");
			Type type;

			ReaderWriterLock rwlock = Scope.RWLock;

			rwlock.AcquireReaderLock(-1);

			CacheKey cacheKey = new CacheKey(targetType, interfaces, options);

			Type cacheType = GetFromCache(cacheKey);

			if (cacheType != null)
			{
				rwlock.ReleaseReaderLock();

				return cacheType;
			}

			rwlock.UpgradeToWriterLock(-1);

			try
			{
				cacheType = GetFromCache(cacheKey);

				if (cacheType != null)
				{
					return cacheType;
				}

				generationHook = options.Hook;

				String newName = targetType.Name + "Proxy" + Guid.NewGuid().ToString("N");

				// Add Interfaces that the proxy implements 

				ArrayList interfaceList = new ArrayList();

				if (interfaces != null)
				{
					interfaceList.AddRange(interfaces);
				}

				AddMixinInterfaces(options, interfaceList);
				
				AddDefaultInterfaces(interfaceList);
				if (targetType.IsSerializable)
				{
					delegateToBaseGetObjectData = VerifyIfBaseImplementsGetObjectData(targetType);
					if (!interfaceList.Contains(typeof(ISerializable)))
						interfaceList.Add(typeof(ISerializable));
				}
				
				ClassEmitter emitter = BuildClassEmitter(newName, targetType, interfaceList);
				emitter.DefineCustomAttribute(new XmlIncludeAttribute(targetType));

			  AddStaticGenerationOptionsField (emitter);

				// Custom attributes

				ReplicateNonInheritableAttributes(targetType, emitter);

					// Fields generations

				FieldReference interceptorsField =
					emitter.CreateField("__interceptors", typeof(IInterceptor[]));

				// Implement builtin Interfaces
				ImplementProxyTargetAccessor(targetType, emitter,interceptorsField);

			
				emitter.DefineCustomAttributeFor(interceptorsField, new XmlIgnoreAttribute());

				// Collect methods

				PropertyToGenerate[] propsToGenerate;
				EventToGenerate[] eventToGenerates;
				MethodInfo[] methods = CollectMethodsAndProperties(emitter, targetType, out propsToGenerate, out eventToGenerates);

				methods = RegisterMixinMethodsAndProperties(emitter, options, methods, ref propsToGenerate, ref eventToGenerates);

				options.Hook.MethodsInspected();

				// Constructor

				ConstructorEmitter typeInitializer = GenerateStaticConstructor(emitter);
				
				FieldReference[] mixinFields = AddMixinFields(options, emitter);

				List<FieldReference> fields = new List<FieldReference>(mixinFields);
				fields.Add(interceptorsField);
				FieldReference[] ctorArgs = fields.ToArray();

				CreateInitializeCacheMethodBody(targetType, methods, emitter, typeInitializer);
				GenerateConstructors(emitter, targetType, ctorArgs);
				GenerateParameterlessConstructor(emitter, targetType, interceptorsField);

				if (delegateToBaseGetObjectData)
				{
					GenerateSerializationConstructor(emitter, interceptorsField, delegateToBaseGetObjectData);
				}

				// Implement interfaces
				if (interfaces != null && interfaces.Length != 0)
				{
					foreach (Type inter in interfaces)
					{
						ImplementBlankInterface(targetType, inter, emitter, interceptorsField, typeInitializer);
					}
				}

				// Create callback methods

				Dictionary<MethodInfo, MethodBuilder> method2Callback = new Dictionary<MethodInfo, MethodBuilder>();

				foreach (MethodInfo method in methods)
				{
					method2Callback[method] = CreateCallbackMethod(emitter, method, method);
				}

				// Create invocation types

				Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

				foreach (MethodInfo method in methods)
				{
					MethodBuilder callbackMethod = method2Callback[method];

					method2Invocation[method] = BuildInvocationNestedType(emitter, targetType,
																		  GetMethodTargetType(method),
																		  method, callbackMethod,
																		  ConstructorVersion.WithoutTargetMethod);
				}

				// Create methods overrides

				Dictionary<MethodInfo, MethodEmitter> method2Emitter = new Dictionary<MethodInfo, MethodEmitter>();

				foreach (MethodInfo method in methods)
				{
					if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) || methodsToSkip.Contains(method))
					{
						continue;
					}

					NestedClassEmitter nestedClass = method2Invocation[method];

					Reference targetRef = GetTargetRef(method, mixinFields, SelfReference.Self);
					MethodEmitter newProxiedMethod = CreateProxiedMethod(
						targetType, method, emitter, nestedClass, interceptorsField, targetRef,
						ConstructorVersion.WithoutTargetMethod, null);

					ReplicateNonInheritableAttributes(method, newProxiedMethod);

					method2Emitter[method] = newProxiedMethod;
				}

				foreach (PropertyToGenerate propToGen in propsToGenerate)
				{
					if (propToGen.CanRead)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.GetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.GetMethod);

						MethodEmitter getEmitter = propToGen.Emitter.CreateGetMethod(atts);

						Reference targetRef = GetTargetRef(propToGen.GetMethod, mixinFields, SelfReference.Self);

						ImplementProxiedMethod(targetType, getEmitter,
											   propToGen.GetMethod, emitter,
											   nestedClass, interceptorsField, targetRef,
											   ConstructorVersion.WithoutTargetMethod, null);

						ReplicateNonInheritableAttributes(propToGen.GetMethod, getEmitter);
					}

					if (propToGen.CanWrite)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.SetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod);

						MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(atts);

						Reference targetRef = GetTargetRef(propToGen.SetMethod, mixinFields, SelfReference.Self);

						ImplementProxiedMethod(targetType, setEmitter,
											   propToGen.SetMethod, emitter,
											   nestedClass, interceptorsField, targetRef,
											   ConstructorVersion.WithoutTargetMethod, null);

						ReplicateNonInheritableAttributes(propToGen.SetMethod, setEmitter);
					}
				}

				foreach (EventToGenerate eventToGenerate in eventToGenerates)
				{

					NestedClassEmitter add_nestedClass = method2Invocation[eventToGenerate.AddMethod];

					MethodAttributes add_atts = ObtainMethodAttributes(eventToGenerate.AddMethod);

					MethodEmitter addEmitter = eventToGenerate.Emitter.CreateAddMethod(add_atts);

					Reference targetRef = GetTargetRef(eventToGenerate.AddMethod, mixinFields, SelfReference.Self);

					ImplementProxiedMethod(targetType, addEmitter,
										   eventToGenerate.AddMethod, emitter,
											add_nestedClass, interceptorsField, targetRef,
										   ConstructorVersion.WithoutTargetMethod, null);

					ReplicateNonInheritableAttributes(eventToGenerate.AddMethod, addEmitter);

					NestedClassEmitter remove_nestedClass = method2Invocation[eventToGenerate.RemoveMethod];

					MethodAttributes remove_atts = ObtainMethodAttributes(eventToGenerate.RemoveMethod);

					MethodEmitter removeEmitter = eventToGenerate.Emitter.CreateRemoveMethod(remove_atts);

					ImplementProxiedMethod(targetType, removeEmitter,
										   eventToGenerate.RemoveMethod, emitter,
											remove_nestedClass, interceptorsField, targetRef,
										   ConstructorVersion.WithoutTargetMethod, null);

					ReplicateNonInheritableAttributes(eventToGenerate.RemoveMethod, removeEmitter);


				}

				ImplementGetObjectData(emitter, interceptorsField, interfaces);

				// Complete type initializer code body

				CompleteInitCacheMethod(typeInitializer.CodeBuilder);

				// Build type

				type = emitter.BuildType();
			  InitializeStaticGenerationOptionsField (type, options);

				AddToCache(cacheKey, type);
			}
			finally
			{
				rwlock.ReleaseWriterLock();
			}

			Scope.SaveAssembly();

			return type;
		}


		protected override Reference GetProxyTargetReference()
		{
			return SelfReference.Self;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return true;
		}

		protected void GenerateSerializationConstructor(ClassEmitter emitter, FieldReference interceptorField, bool delegateToBaseGetObjectData)
		{
			ArgumentReference arg1 = new ArgumentReference(typeof(SerializationInfo));
			ArgumentReference arg2 = new ArgumentReference(typeof(StreamingContext));

			ConstructorEmitter constr = emitter.CreateConstructor(arg1, arg2);

			constr.CodeBuilder.AddStatement(
				new ConstructorInvocationStatement(serializationConstructor,
					arg1.ToExpression(), arg2.ToExpression()));

			Type[] object_arg = new Type[] { typeof(String), typeof(Type) };
			MethodInfo getValueMethod = typeof(SerializationInfo).GetMethod("GetValue", object_arg);

			MethodInvocationExpression getInterceptorInvocation =
				new MethodInvocationExpression(arg1, getValueMethod,
				new ConstReference("__interceptors").ToExpression(),
				new TypeTokenExpression(typeof(IInterceptor[])));

			constr.CodeBuilder.AddStatement(new AssignStatement(
				interceptorField, getInterceptorInvocation));

			constr.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, 
		                                               ArgumentReference arg1, ArgumentReference arg2)
		{
			codebuilder.AddStatement (new ExpressionStatement (new MethodInvocationExpression (null,
			  typeof (ProxySerializer).GetMethod ("SerializeClassProxyData"), arg1.ToExpression (),
			  new ConstReference (delegateToBaseGetObjectData).ToExpression (), new TypeTokenExpression (targetType),
			  SelfReference.Self.ToExpression())));

			if (delegateToBaseGetObjectData)
			{
				MethodInfo baseGetObjectData = targetType.GetMethod("GetObjectData", 
					new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });

				codebuilder.AddStatement( new ExpressionStatement(
					new MethodInvocationExpression( baseGetObjectData, 
						arg1.ToExpression(), arg2.ToExpression() )) );
			}
		}
	}
}
