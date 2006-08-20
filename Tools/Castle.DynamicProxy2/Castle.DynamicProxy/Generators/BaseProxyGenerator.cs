// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;
	using System.Runtime.CompilerServices;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	/// <summary>
	/// Base class that exposes the common functionalities
	/// to proxy generation.
	/// </summary>
	public abstract class BaseProxyGenerator
	{
		private readonly ModuleScope scope;

		protected readonly Type classBaseType;
		
		protected ClassEmitter emitter;
		protected FieldReference interceptorsField;
//		protected FieldReference interceptorSelectorField;
//		protected FieldReference useSelectorField;
		protected int nestedCounter;
		protected int callbackCounter;
		protected bool canOnlyProxyVirtuals = true;

#if DOTNET2
		private ReaderWriterLock internalsToDynProxyLock = new ReaderWriterLock();
		private System.Collections.Generic.IDictionary<Assembly, bool> internalsToDynProxy = new System.Collections.Generic.Dictionary<Assembly, bool>();
#endif

		protected BaseProxyGenerator(ModuleScope scope, Type classBaseType)
		{
			this.scope = scope;
			this.classBaseType = classBaseType;
		}

		protected ModuleScope Scope
		{
			get { return scope; }
		}

		protected virtual ClassEmitter BuildClassEmitter(String typeName, Type parentType, Type[] interfaces)
		{
			if (interfaces == null)
			{
				interfaces = new Type[0];
			}

			return new ClassEmitter(Scope, typeName, parentType, interfaces, true);
		}

		protected void GenerateIProxyTargetAccessor()
		{
			emitter.TypeBuilder.AddInterfaceImplementation(typeof(IProxyTargetAccessor));

			MethodEmitter methodEmitter = emitter.CreateMethod("GetTarget",
				MethodAttributes.Public | MethodAttributes.Virtual,
				new ReturnReferenceExpression(typeof(object)));

			methodEmitter.CodeBuilder.AddStatement(
				new ReturnStatement(GetProxyTargetReference()));
		}

		protected bool IsInternalToDynamicProxy(Assembly asm)
		{
#if DOTNET2
			internalsToDynProxyLock.AcquireReaderLock(-1);

			try
			{
				if (!internalsToDynProxy.ContainsKey(asm))
				{
					internalsToDynProxyLock.UpgradeToWriterLock(-1);
					InternalsVisibleToAttribute[] atts = (InternalsVisibleToAttribute[])
						asm.GetCustomAttributes(typeof(InternalsVisibleToAttribute), false);

					bool found = false;

					foreach (InternalsVisibleToAttribute internals in atts)
					{
						if (internals.AssemblyName.Contains(ModuleScope.ASSEMBLY_NAME))
						{
							found = true;
							break;
						}
					}
					internalsToDynProxy.Add(asm, found);
				}
				return internalsToDynProxy[asm];

			}
			finally
			{
				internalsToDynProxyLock.ReleaseLock();
			}
#else
			return false;
#endif
		}

		protected virtual Type CreateType()
		{
			Type newType = emitter.BuildType();

//			RegisterInCache(newType);

			return newType;
		}

		protected virtual void GenerateFields()
		{
			interceptorsField = emitter.CreateField("__interceptors", typeof(IInterceptor[]));
//			useSelectorField = emitter.CreateField("__useSelector", typeof(bool));
//			interceptorSelectorField = emitter.CreateField("__interceptorSelector", typeof(IInterceptorSelector));
		}

		/// <summary>
		/// Used by dinamically implement <see cref="IProxyTargetAccessor"/>
		/// </summary>
		/// <returns></returns>
		protected abstract Reference GetProxyTargetReference();

		// TODO: Implement this
		protected Type GetFromCache(CacheKey key)
		{
			return null;
		}

		// TODO: Implement this
		protected void AddToCache(CacheKey key, Type type)
		{
		}

		protected MethodInfo[] GetMethodsToProxy(Type type, IProxyGenerationHook hook)
		{
			ArrayList methodList = new ArrayList();
			
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			MethodInfo[] methods = type.GetMethods(flags);

			foreach (MethodInfo method in methods)
			{
				bool shouldIntercept = hook.ShouldInterceptMethod(type, method);

				if (!shouldIntercept)
				{
					continue;
				}

				if (shouldIntercept && canOnlyProxyVirtuals && !method.IsVirtual)
				{
					hook.NonVirtualMemberNotification(type, method);
					continue;
				}

				methodList.Add(method);
			}

			return (MethodInfo[]) methodList.ToArray(typeof(MethodInfo));
		}
		
		protected void GenerateMethods(Type targetType, Reference targetRef, IProxyGenerationHook hook, bool useSelector)
		{
			MethodInfo[] methods = GetMethodsToProxy(targetType, hook);

			foreach (MethodInfo method in methods)
			{
				GenerateMethod(method, targetRef, targetType, useSelector);
			}
		}

		private void GenerateMethod(MethodInfo method, Reference targetRef, Type targetType, bool useSelector)
		{
			MethodAttributes atts = ObtainMethodAttributes(method);
				
			MethodEmitter methodEmitter = emitter.CreateMethod(method.Name, atts);
				
			methodEmitter.CopyParametersAndReturnTypeFrom(method);

			MethodInfo methodOnTarget = GetMethodOnTarget(method);

			MethodBuilder callbackMethod = CreateCallbackMethod(method, methodOnTarget);
				
			// Interface proxy
			// NestedClassEmitter iinvocationImpl =
			//	CreateIInvocationImplementation(interfaceType, targetField.Reference.FieldType,
			//					method, methodOnTarget);
								
			NestedClassEmitter iinvocationImpl =
				CreateIInvocationImplementation(emitter.TypeBuilder, methodEmitter, targetType, targetType, method, methodOnTarget, callbackMethod);

			TypeReference[] dereferencedArguments = IndirectReference.WrapIfByRef(methodEmitter.Arguments);

			Type iinvocation = iinvocationImpl.TypeBuilder;

			Type[] set1 = null; Type[] set2 = null;
			
			if (iinvocation.IsGenericType)
			{
				// get type generics
				set1 = targetType.GetGenericArguments();
			}
			
			if (method.IsGenericMethod)
			{
				// get method generics
				set2 = method.GetGenericArguments();
			}

			iinvocation = iinvocation.MakeGenericType(TypeUtil.Union(set1, set2));
	
			LocalReference invocationImplLocal = methodEmitter.CodeBuilder.DeclareLocal(iinvocation);

			// TODO: Initialize iinvocation instance 
			// with ordinary arguments and in and out arguments

			Expression interceptors = null;

			if (useSelector)
			{
				// TODO: Generate code that checks the return of selector
				// if no interceptors is returned, should we invoke the base.Method directly?
			}
			else
			{
				interceptors = interceptorsField.ToExpression();
			}

			TypeTokenExpression typeExp = new TypeTokenExpression(targetType);
			// TypeTokenExpression typeExp = new TypeTokenExpression(classBaseType);
				
			MethodTokenExpression methodTokenExp = 
				new MethodTokenExpression(methodOnTarget == null ? method : methodOnTarget, targetType);

			ConstructorInfo constructor = iinvocationImpl.Constructors[0].Builder;
			
			constructor = TypeBuilder.GetConstructor(
				iinvocation, iinvocationImpl.Constructors[0].Builder);
			
			NewInstanceExpression newInvocImpl = new NewInstanceExpression(
				constructor,
				targetRef.ToExpression(),
				interceptors,
				typeExp,
				methodTokenExp,
				NullExpression.Instance, 
				new ReferencesToObjectArrayExpression(dereferencedArguments));

			methodEmitter.CodeBuilder.AddStatement(new AssignStatement(invocationImplLocal, newInvocImpl));

			methodEmitter.CodeBuilder.AddStatement(new ExpressionStatement(
			                                       	new MethodInvocationExpression(invocationImplLocal, Constants.AbstractInvocationProceed)));

			if (method.ReturnType != typeof(void))
			{
				// Emit code to return with cast from ReturnValue
				MethodInvocationExpression getRetVal =
					new MethodInvocationExpression(invocationImplLocal,
					                               typeof(AbstractInvocation).GetMethod("get_ReturnValue"));

				methodEmitter.CodeBuilder.AddStatement(new ReturnStatement(
				                                       	new ConvertExpression(method.ReturnType, getRetVal)));
			}

			methodEmitter.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private MethodBuilder CreateCallbackMethod(MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			MethodAttributes atts = MethodAttributes.Family;
			
			MethodEmitter callBackMethod = emitter.CreateMethod(methodInfo.Name + "_" + ++callbackCounter, atts);

			MethodInfo targetMethod = methodOnTarget != null ? methodOnTarget : methodInfo;
			
			callBackMethod.CopyParametersAndReturnTypeFrom(targetMethod);
			
			// TODO: Params
			
			if (targetMethod.IsGenericMethod)
			{
				targetMethod = targetMethod.MakeGenericMethod(callBackMethod.ActualGenericParameters);
			}

			callBackMethod.CodeBuilder.AddStatement(new ExpressionStatement(
                                        	new MethodInvocationExpression(SelfReference.Self, targetMethod)));
			
			// TODO: return value

			callBackMethod.CodeBuilder.AddStatement(new ReturnStatement());

			return callBackMethod.MethodBuilder;
		}

		protected virtual MethodInfo GetMethodOnTarget(MethodInfo method)
		{
			return null;
		}

		protected NestedClassEmitter CreateIInvocationImplementation(TypeBuilder proxyType, 
			MethodEmitter methodEmitter, Type primaryType, Type targetType, 
			MethodInfo methodInfo, MethodInfo methodOnTargetInfo, MethodInfo callbackMethod)
		{
			ParameterInfo[] parameters = methodInfo.GetParameters();
			
			nestedCounter++;

			NestedClassEmitter nested = new NestedClassEmitter(emitter,
				"Invocation" + nestedCounter.ToString(), typeof(AbstractInvocation), new Type[0]);

			// Type[] genTypes = TypeUtil.Union(primaryType.GetGenericArguments(), methodEmitter.ActualGenericParameters);
			Type[] genTypes = TypeUtil.Union(primaryType.GetGenericArguments(), methodEmitter.ActualGenericParameters);

			nested.CreateGenericParameters(genTypes);

			if (methodOnTargetInfo == null)
			{
				methodOnTargetInfo = methodInfo;
			}
									
			// Create the fields

			FieldReference fieldRef = CreateIInvocationFields(nested, proxyType);

			// Create constructor

			CreateIInvocationConstructor(emitter.TypeBuilder, nested, fieldRef);

			// Apply generic parameters to the method

//			if (callbackMethod.IsGenericMethod)
//			{
//				Type[] genericArgs = methodOnTargetInfo.GetGenericArguments();
//
//				Type[] newGenericArgs = new Type[genericArgs.Length];
//				
//				int index = 0;
//
//				foreach (Type gArg in genericArgs)
//				{
//					Type genericArg = nested.GetGenericArgument(gArg.Name);
//
//					if (genericArg != null)
//					{
//						newGenericArgs[index++] = genericArg;
//					}
//				}
//
//				// callbackMethod = callbackMethod.MakeGenericMethod(newGenericArgs);
//
//				Type proxyType2 = proxyType.MakeGenericType(nested.GenericTypeParams[0]);
//
//				callbackMethod = TypeBuilder.GetMethod(proxyType2, callbackMethod);
//			}
			
			// InvokeMethodOnTarget implementation

			CreateIInvocationInvokeOnTarget(nested, parameters, fieldRef, callbackMethod);

			return nested;
		}

		private void CreateIInvocationInvokeOnTarget(NestedClassEmitter nested, 
		                                             ParameterInfo[] parameters, 
		                                             FieldReference targetField, MethodInfo methodInfo)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;
			
			MethodEmitter method = nested.CreateMethod("InvokeMethodOnTarget",
													   new ReturnReferenceExpression(typeof(void)), methodAtts);

			Expression[] args = new Expression[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				if (!param.IsOut && !param.IsRetval)
				{
					args[i] = new ConvertExpression(param.ParameterType,
						new MethodInvocationExpression(SelfReference.Self,
							typeof(AbstractInvocation).GetMethod("GetArgumentValue"),
								new LiteralIntExpression(i)));
				}
				else
				{
					throw new NotImplementedException("Int/Ref parameters are not supported yet");
				}
			}

			LocalReference ret_local = null;

			FieldInfo f = targetField.Reference;
			
			method.CodeBuilder.SetNonEmpty();
			method.CodeBuilder.Generator.Emit(OpCodes.Ldarg_0);
			method.CodeBuilder.Generator.Emit(OpCodes.Ldfld, f);

			// methodInfo = TypeBuilder.GetMethod(
			// 	emitter.TypeBuilder.MakeGenericType(emitter.GetGenericArgument("T")), methodInfo);

			methodInfo = methodInfo.MakeGenericMethod(nested.GetGenericArgument("Z"));
			// methodInfo = methodInfo.MakeGenericMethod(typeof(int));
			// methodInfo = methodInfo.MakeGenericMethod(nested.GetGenericArgument("T"));
			// methodInfo = methodInfo.MakeGenericMethod(methodInfo.GetGenericArguments());
			
			method.CodeBuilder.Generator.EmitCall(OpCodes.Call, methodInfo, null);
			
			// method.CodeBuilder.Generator.Emit(OpCodes.Pop);
			method.CodeBuilder.Generator.Emit(OpCodes.Ret);

//			MethodInvocationExpression baseMethodInvExp =
//				new MethodInvocationExpression(targetField, methodInfo, args);

			// TODO: Process out/ref arguments

//			if (methodInfo.ReturnType != typeof(void))
//			{
//				ret_local = method.CodeBuilder.DeclareLocal(typeof(object));
//				method.CodeBuilder.AddStatement(
//					new AssignStatement(ret_local,
//						new ConvertExpression(typeof(object), methodInfo.ReturnType, baseMethodInvExp)));
//			}
//			else
//			{
//				method.CodeBuilder.AddStatement(new ExpressionStatement(baseMethodInvExp));
//			}
//
//			if (methodInfo.ReturnType != typeof(void))
//			{
//				MethodInvocationExpression setRetVal =
//					new MethodInvocationExpression(SelfReference.Self,
//						typeof(AbstractInvocation).GetMethod("set_ReturnValue"), ret_local.ToExpression());
//
//				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
//			}

			// method.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private void CreateIInvocationConstructor(TypeBuilder proxyType, NestedClassEmitter nested, 
		                                          FieldReference targetField)
		{
			ArgumentReference cArg0 = new ArgumentReference(proxyType);
			ArgumentReference cArg1 = new ArgumentReference(typeof(IInterceptor[]));
			ArgumentReference cArg2 = new ArgumentReference(typeof(Type));
			ArgumentReference cArg3 = new ArgumentReference(typeof(MethodInfo));
			ArgumentReference cArg4 = new ArgumentReference(typeof(MethodInfo));
			ArgumentReference cArg5 = new ArgumentReference(typeof(object[]));

			ConstructorEmitter constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4, cArg5);

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));
			constructor.CodeBuilder.InvokeBaseConstructor(Constants.AbstractInvocationConstructor,
				cArg1, cArg2, cArg3, cArg4, cArg5);
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private FieldReference CreateIInvocationFields(NestedClassEmitter nested, Type targetType)
		{
			return nested.CreateField("target", targetType);
		}

		/// <summary>
		/// Based on Nemerle's FrameworkGetMethod
		/// </summary>
//		private MethodInfo FrameworkGetMethod(Type targetType, MethodInfo methodInfo)
//		{
//			Type runtimeType = typeof(object).Assembly.GetType("System.RuntimeType");
//
//			if (targetType.GetType().Equals(runtimeType))
//			{
//				return GetHackishMethod(targetType, methodInfo);
//			}
//			else
//			{
//				Type td = targetType.GetGenericTypeDefinition();
//
//				if (targetType.GetType().Equals(runtimeType))
//				{
//					methodInfo = GetHackishMethod(td, methodInfo);
//				}
//
//				return TypeBuilder.GetMethod(targetType, methodInfo);
//			}
//		}

		/// <summary>
		/// Based on Nemerle's GetHackishMethod
		/// </summary>
//		private MethodInfo GetHackishMethod(Type targetType, MethodInfo methodInfo)
//		{
//			int mToken;
//			
//			if (methodInfo is MethodBuilder)
//			{
//				mToken = (int) typeof(MethodBuilder).
//				               	GetProperty("MetadataTokenInternal", BindingFlags.NonPublic | BindingFlags.Instance).
//								GetValue(methodInfo, null);
//			}
//			else
//			{
//				mToken = methodInfo.MetadataToken;
//			}
//			
//			BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public |
//			                     BindingFlags.Instance | BindingFlags.Static |
//			                     BindingFlags.DeclaredOnly;
//			
//			foreach(MethodInfo m in targetType.GetMethods(flags))
//			{
//				if (m.MetadataToken == mToken)
//					return m;
//			}
//
//			return null;
//		}

		protected MethodAttributes ObtainMethodAttributes(MethodInfo method)
		{
			MethodAttributes atts;

			//			if (context.ShouldCreateNewSlot(method))
			//			{
			//				atts = MethodAttributes.NewSlot;
			//			}
			//			else
			{
				atts = MethodAttributes.Virtual;
			}

			if (method.IsPublic)
			{
				atts |= MethodAttributes.Public;
			}
			if (IsInternalToDynamicProxy(method.DeclaringType.Assembly) && method.IsAssembly)
			{
				atts |= MethodAttributes.Assembly;
			}
			if (method.IsHideBySig)
			{
				atts |= MethodAttributes.HideBySig;
			}

			if (method.IsFamilyAndAssembly)
			{
				atts |= MethodAttributes.FamANDAssem;
			}
			else if (method.IsFamilyOrAssembly)
			{
				atts |= MethodAttributes.FamORAssem;
			}
			else if (method.IsFamily)
			{
				atts |= MethodAttributes.Family;
			}

			if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
			{
				atts |= MethodAttributes.SpecialName;
			}
			return atts;
		}
	}
}
