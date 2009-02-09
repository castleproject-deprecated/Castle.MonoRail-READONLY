﻿// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration.Tests
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using Castle.Core.Configuration;
	using Castle.Core.Interceptor;
	using Castle.Core.Resource;
	using Castle.DynamicProxy;
	using Castle.Facilities.Logging;
	using Castle.Facilities.WcfIntegration.Behaviors;
	using Castle.Facilities.WcfIntegration.Demo;
	using Castle.Facilities.WcfIntegration.Tests.Behaviors;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using log4net.Appender;
	using log4net.Config;
	using log4net.Core;
	using NUnit.Framework;

	[TestFixture]
	public class WcfClientFixture
	{
		private MemoryAppender memoryAppender;

		#region Setup/Teardown

		[SetUp]
		public void TestInitialize()
		{
			windsorContainer = new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(WcfClient.ForChannels(
					new DefaultClientModel()
					{
						Endpoint = WcfEndpoint.ForContract<IOperations>()
							.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
					}),
					Component.For<IServiceBehavior>()
						.Instance(new ServiceDebugBehavior()
						{
							IncludeExceptionDetailInFaults = true
						}),
					Component.For<NetDataContractFormatBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Explicit),
					Component.For<Operations>()
						.DependsOn(new { number = 42 })
						.ActAs(new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.ForContract<IOperations>()
								.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"),
							WcfEndpoint.ForContract<IOperationsEx>()
								.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations/Ex")
								)
						),
					Component.For<IAmUsingWindsor>().ImplementedBy<UsingWindsor>()
						.DependsOn(new { number = 42 })
						.ActAs(new DefaultServiceModel()
						)
				);

			RegisterLoggingFacility(windsorContainer);
		}

		[TearDown]
		public void TestCleanup()
		{
			windsorContainer.Dispose();
		}

		#endregion

		private IWindsorContainer windsorContainer;

		[Test]
		public void CanResolveClientInterfaceAssociatedWithChannel()
		{
			windsorContainer.Register(
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(42, client.GetValueFromConstructor());
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelUsingDefaultBinding()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 28 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(WcfEndpoint.ForContract<IOperations>()
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations2")
							)
				)))
			{
				using (IWindsorContainer clientContainer = new WindsorContainer()
					.AddFacility<WcfFacility>(f => f.DefaultBinding = 
						new NetTcpBinding { PortSharingEnabled = true }
					)
					.Register(Component.For<IOperations>()
						.Named("operations")
						.ActAs(new DefaultClientModel()
						{
							Endpoint = WcfEndpoint.At("net.tcp://localhost/Operations2")
						})
					))
				{
					IOperations client = clientContainer.Resolve<IOperations>("operations");
					Assert.AreEqual(28, client.GetValueFromConstructor());
				}
			}
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelUsingSuppliedModel()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 28 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(WcfEndpoint.ForContract<IOperations>()
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations2")
							)
				)))
			{
				using (IWindsorContainer clientContainer = new WindsorContainer()
					.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
					.Register(Component.For<IOperations>()
						.Named("operations")
						.LifeStyle.Transient
						.ActAs(new DefaultClientModel())
					))
				{
					IOperations client1 = clientContainer.Resolve<IOperations>("operations",
						new { Model = new DefaultClientModel
						{
							Endpoint = WcfEndpoint.BoundTo(new NetTcpBinding())
								.At("net.tcp://localhost/Operations2")
						}});
					IOperations client2 = clientContainer.Resolve<IOperations>("operations",
						new { Model = new DefaultClientModel() 
						{
							Endpoint = WcfEndpoint.BoundTo(new NetTcpBinding())
								.At("net.tcp://localhost/Operations2")
						}});
					Assert.AreEqual(28, client1.GetValueFromConstructor());
					Assert.AreEqual(28, client2.GetValueFromConstructor());
					clientContainer.Release(client1);
					clientContainer.Release(client2);
				}
			}
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelUsingSuppliedEndpoint()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 28 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(WcfEndpoint.ForContract<IOperations>()
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations2")
							)
				)))
			{
				using (IWindsorContainer clientContainer = new WindsorContainer()
					.AddFacility<WcfFacility>(f => f.DefaultBinding =
						new NetTcpBinding { PortSharingEnabled = true }
					)
					.Register(Component.For<IOperations>()
						.Named("operations")
						.LifeStyle.Transient
						.ActAs(new DefaultClientModel())
					))
				{
					IOperations client1 = clientContainer.Resolve<IOperations>("operations",
						new { Endpoint = WcfEndpoint.At("net.tcp://localhost/Operations2") });
					IOperations client2 = clientContainer.Resolve<IOperations>("operations",
						new { Endpoint = WcfEndpoint.At("net.tcp://localhost/Operations2") });
					Assert.AreEqual(28, client1.GetValueFromConstructor());
					Assert.AreEqual(28, client2.GetValueFromConstructor());
					clientContainer.Release(client1);
					clientContainer.Release(client2);
				}
			}
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelFromConfiguration()
		{
			windsorContainer.Register(Component.For<IAmUsingWindsor>()
				.Named("usingWindsor")
				.ActAs(new DefaultClientModel()
				{
					Endpoint = WcfEndpoint
						.FromConfiguration("WSHttpBinding_IAmUsingWindsor")
				}));

			IAmUsingWindsor client = windsorContainer.Resolve<IAmUsingWindsor>("usingWindsor");
			Assert.AreEqual(42, client.GetValueFromWindsorConfig());
		}

		[Test]
		public void CanResolveClientWhenListedInTheFacility()
		{
			windsorContainer.Register(Component.For<ClassNeedingService>());
			ClassNeedingService component = windsorContainer.Resolve<ClassNeedingService>();
			Assert.IsNotNull(component.Operations);
			Assert.AreEqual(42, component.Operations.GetValueFromConstructor());
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelUsingRelativeAddress()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 28 })
					.ActAs(new DefaultServiceModel()
						.AddBaseAddresses("net.tcp://localhost/Operations")
						.AddEndpoints(WcfEndpoint.ForContract<IOperations>()
							.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("Extended")
							)	
				)))
			{
				using (IWindsorContainer clientContainer = new WindsorContainer()
					.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
					.Register(Component.For<IOperations>()
						.Named("operations")
						.ActAs(new DefaultClientModel()
						{
							Endpoint = WcfEndpoint
								.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations/Extended")
						
						})
					))
				{
					IOperations client = clientContainer.Resolve<IOperations>("operations");
					Assert.AreEqual(28, client.GetValueFromConstructor());
				}
			}
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelUsingViaAddress()
		{
			using (IWindsorContainer localContainer = new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<IOperations>()
						.ImplementedBy<Operations>()
						.DependsOn(new { number = 22 })
						.ActAs(new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("urn:castle:operations")
								.Via("net.tcp://localhost/OperationsVia")
								)
						),
					Component.For<IOperations>()
						.Named("operations")
						.ActAs(new DefaultClientModel()
						{
							Endpoint = WcfEndpoint
								.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("urn:castle:operations")
								.Via("net.tcp://localhost/OperationsVia")
						})
					))
			{
				IOperations client = localContainer.Resolve<IOperations>("operations");
				Assert.AreEqual(22, client.GetValueFromConstructor());
			}
		}

		[Test]
		public void WillApplyOperationBehaviors()
		{
			windsorContainer.Register(
				Component.For<IOperationsEx>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations/Ex")
							.AddExtensions(typeof(NetDataContractFormatBehavior))
					})
				);

			IOperationsEx client = windsorContainer.Resolve<IOperationsEx>("operations");
			client.Backup(new Dictionary<string, object>());
		}

		[Test]
		public void WillApplyExlpicitScopedKeyEndpointBehaviors()
		{
			CallCountEndpointBehavior.CallCount = 0;
			windsorContainer.Register(
				Component.For<CallCountEndpointBehavior>()
					.Attribute("scope").Eq(WcfExtensionScope.Explicit)
					.Named("specialBehavior")
					.LifeStyle.Transient,
				Component.For<IOperationsEx>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					       {
					       	Endpoint = WcfEndpoint
					       		.BoundTo(new NetTcpBinding {PortSharingEnabled = true})
					       		.At("net.tcp://localhost/Operations/Ex")
					       		.AddExtensions("specialBehavior")
					       })
				);
			IOperationsEx client = windsorContainer.Resolve<IOperationsEx>("operations");
			client.Backup(new Dictionary<string, object>());
			Assert.AreEqual(1, CallCountEndpointBehavior.CallCount);
		}

		[Test]
		public void WillApplyExlpicitScopedServiceEndpointExtensions()
		{
			CallCountEndpointBehavior.CallCount = 0;
			windsorContainer.Register(
				Component.For<CallCountEndpointBehavior>()
					.Attribute("scope").Eq(WcfExtensionScope.Explicit)
					.Named("specialBehavior")
					.LifeStyle.Transient,
				Component.For<IOperationsEx>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations/Ex")
							.AddExtensions(typeof(CallCountEndpointBehavior))
					})
				);
			IOperationsEx client = windsorContainer.Resolve<IOperationsEx>("operations");
			client.Backup(new Dictionary<string, object>());
			Assert.AreEqual(1, CallCountEndpointBehavior.CallCount);
		}

		[Test]
		public void WillRecoverFromAnUnhandledException()
		{
			windsorContainer.Register(
				Component.For<IOperationsEx>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations/Ex")
					
					})
				);

			IOperationsEx client = windsorContainer.Resolve<IOperationsEx>("operations");
			try
			{
				client.ThrowException();
			}
			catch (Exception)
			{
				client.Backup(new Dictionary<string, object>());
			}
		}
			
		[Test]
		public void CanResolveClientAssociatedWithChannelFromXmlConfiguration()
		{
			using (IWindsorContainer container = new WindsorContainer()
					.Install(Configuration.FromXml(new StaticContentResource(xmlConfiguration))))
			{
				IAmUsingWindsor client = container.Resolve<IAmUsingWindsor>("usingWindsor");
				Assert.AreEqual(42, client.GetValueFromWindsorConfig());
			}
		}

		[Test]
		public void CanAccessIContextChannelInterface()
		{
			windsorContainer.Register(
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			IContextChannel channel = client as IContextChannel;
			Assert.IsNotNull(channel);
			Assert.AreEqual(CommunicationState.Created, channel.State);
		}

		[Test]
		public void CanUseOperationContextWithClient()
		{
			windsorContainer.Register(
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			using (new OperationContextScope(WcfContextChannel.For(client)))
            {
				MessageHeader header = MessageHeader.CreateHeader("MyHeader", "", "MyValue", false);
				OperationContext.Current.OutgoingMessageHeaders.Add(header);
				Assert.AreEqual(42, client.GetValueFromConstructor());
            }
		}

		[Test]
		public void CanCaptureRequestsAndResponses()
		{
			windsorContainer.Register(
				Component.For<LogMessageEndpointBehavior>()
					.Attribute("scope").Eq(WcfExtensionScope.Explicit)
					.Named("logMessageBehavior"),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.LogMessages()
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(42, client.GetValueFromConstructor());
			Assert.AreEqual(4, memoryAppender.GetEvents().Length);

			foreach (LoggingEvent log in memoryAppender.GetEvents())
			{
				Assert.AreEqual(typeof(IOperations).FullName, log.LoggerName);
				Assert.IsTrue(log.Properties.Contains("NDC"));
			}
		}

		[Test]
		public void CanCaptureRequestsAndResponsesUsingCustomFormat()
		{
			windsorContainer.Register(
				Component.For<LogMessageEndpointBehavior>()
					.Attribute("scope").Eq(WcfExtensionScope.Explicit)
					.Named("logMessageBehavior"),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.LogMessages("h")
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(42, client.GetValueFromConstructor());
			Assert.AreEqual(4, memoryAppender.GetEvents().Length);

			foreach (LoggingEvent log in memoryAppender.GetEvents())
			{
				Assert.AreEqual(typeof(IOperations).FullName, log.LoggerName);
				Assert.IsTrue(log.Properties.Contains("NDC"));
			}
		}

		[Test]
		public void CanCaptureRequestsAndResponsesUsingGlobalFormatter()
		{
			windsorContainer.Register(
				Component.For<IFormatProvider>().ImplementedBy<HelloFormatter>(),
				Component.For<LogMessageEndpointBehavior>()
					.Attribute("scope").Eq(WcfExtensionScope.Explicit)
					.Named("logMessageBehavior"),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.LogMessages()
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(42, client.GetValueFromConstructor());
			Assert.AreEqual(4, memoryAppender.GetEvents().Length);

			int i = 0;
			foreach (LoggingEvent log in memoryAppender.GetEvents())
			{
				Assert.AreEqual(typeof(IOperations).FullName, log.LoggerName);
				Assert.IsTrue(log.Properties.Contains("NDC"));

				if ((++i % 2) == 0)
				{
					Assert.AreEqual("Hello", log.RenderedMessage);
				}
			}
		}

		[Test]
		public void CanCaptureRequestsAndResponsesUsingFormatExtension()
		{
			windsorContainer.Register(
				Component.For<HelloMessageFormat>(),
				Component.For<LogMessageEndpointBehavior>()
					.Named("logMessageBehavior"),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(42, client.GetValueFromConstructor());
			Assert.AreEqual(4, memoryAppender.GetEvents().Length);

			int i = 0;
			foreach (LoggingEvent log in memoryAppender.GetEvents())
			{
				Assert.AreEqual(typeof(IOperations).FullName, log.LoggerName);
				Assert.IsTrue(log.Properties.Contains("NDC"));

				if ((++i % 2) == 0)
				{
					Assert.AreEqual("Hello", log.RenderedMessage);
				}
			}
		}

		[Test]
		public void CanCaptureRequestsAndResponsesUsingExplicitFormatter()
		{
			windsorContainer.Register(
				Component.For<LogMessageEndpointBehavior>()
					.Attribute("scope").Eq(WcfExtensionScope.Explicit)
					.Named("logMessageBehavior"),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.LogMessages<HelloFormatter>()
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(42, client.GetValueFromConstructor());
			Assert.AreEqual(4, memoryAppender.GetEvents().Length);

			int i = 0;
			foreach (LoggingEvent log in memoryAppender.GetEvents())
			{
				Assert.AreEqual(typeof(IOperations).FullName, log.LoggerName);
				Assert.IsTrue(log.Properties.Contains("NDC"));

				if ((++i % 2) == 0)
				{
					Assert.AreEqual("Hello", log.RenderedMessage);
				}
			}
		}

		[Test]
		public void CanAddMessageHeader()
		{
			windsorContainer.Register(
				Component.For<MessageLifecycleBehavior>(),
				Component.For<LogMessageEndpointBehavior>()
					.Attribute("scope").Eq(WcfExtensionScope.Explicit),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.AddExtensions(new AddOperationsHeader("MyHeader", "Hello"))
							.LogMessages()
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(42, client.GetValueFromConstructor());

			int i = 0;
			foreach (LoggingEvent log in memoryAppender.GetEvents())
			{
				if ((++i % 2) == 0)
				{
					Assert.IsTrue(log.RenderedMessage.Contains("<MyHeader>Hello</MyHeader>"));
				}
			}
		}

		[Test]
		public void CanModifyRequestAndResponseBody()
		{
			windsorContainer.Register(
				Component.For<MessageLifecycleBehavior>(),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.AddExtensions(new ReplaceOperationsResult("100"))
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(100, client.GetValueFromConstructor());
		}

		[Test]
		public void WillUseSameXmlDocumentForConsecutiveModifications()
		{
			StoreMessageBody start = new StoreMessageBody(MessageLifecycle.Requests);
			StoreMessageBody end = new StoreMessageBody(MessageLifecycle.Requests);

			windsorContainer.Register(
				Component.For<MessageLifecycleBehavior>(),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.AddExtensions(start, new ReplaceOperationsResult("100").ExecuteAt(1),
										   new ReplaceOperationsResult("200").ExecuteAt(2),
										   end)
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(200, client.GetValueFromConstructor());
			Assert.IsNotNull(start.Body);
			Assert.AreSame(start.Body, end.Body);
		}

		[Test]
		public void CanModifyRequestAndResponseBodyAndAddHeaders()
		{
			windsorContainer.Register(
				Component.For<MessageLifecycleBehavior>(),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.AddExtensions(new ReplaceOperationsResult("100"),
										   new AddOperationsHeader("MyHeader", "Hello"))
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(100, client.GetValueFromConstructor());

			int i = 0;
			foreach (LoggingEvent log in memoryAppender.GetEvents())
			{
				if ((++i % 2) == 0)
				{
					Assert.IsTrue(log.RenderedMessage.Contains("<MyHeader>Hello</MyHeader>"));
				}
			}
		}

		[Test]
		public void WillCreateNewXmlDocumentForNormalActions()
		{
			StoreMessageBody start = new StoreMessageBody(MessageLifecycle.Requests);
			StoreMessageBody end = new StoreMessageBody(MessageLifecycle.Requests);

			windsorContainer.Register(
				Component.For<MessageLifecycleBehavior>(),
				Component.For<IOperations>()
					.Named("operations")
					.ActAs(new DefaultClientModel()
					{
						Endpoint = WcfEndpoint
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations")
							.AddExtensions(start, new ReplaceOperationsResult("100").ExecuteAt(1),
										   new ReplaceOperationsResult("200").ExecuteAt(2), 
										   new AddOperationsHeader("MyHeader", "Hello"),
										   end)
					})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(200, client.GetValueFromConstructor());
			Assert.IsNotNull(start.Body);
			Assert.IsNotNull(end.Body);
			Assert.AreNotSame(start.Body, end.Body);
		}

		[Test]
		public void WillReleaseAllExtensionsWhenUnregistered()
		{
			windsorContainer.Register(
				Component.For<MessageLifecycleBehavior>(),
				Component.For<IOperations>()
				.Named("operations")
				.ActAs(new DefaultClientModel()
				{
					Endpoint = WcfEndpoint
						.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
						.At("net.tcp://localhost/Operations")
				})
			);
			windsorContainer.Resolve<IOperations>("operations");
			windsorContainer.Kernel.RemoveComponent("operations");
		}

        protected void RegisterLoggingFacility(IWindsorContainer container)
        {
            MutableConfiguration facNode = new MutableConfiguration("facility" );
            facNode.Attributes["id"] = "logging";
            facNode.Attributes["loggingApi"] = "ExtendedLog4net";
			facNode.Attributes["configFile"] = "";
            container.Kernel.ConfigurationStore.AddFacilityConfiguration("logging", facNode);
            container.AddFacility("logging", new LoggingFacility());

			memoryAppender = new MemoryAppender();
			BasicConfigurator.Configure(memoryAppender);
        }

        private static string xmlConfiguration = @"<?xml version='1.0' encoding='utf-8' ?>
<configuration>
	<facilities>
		<facility id='wcf' 
				  type='Castle.Facilities.WcfIntegration.WcfFacility,
				        Castle.Facilities.WcfIntegration' />
	</facilities>

	<components>
		<component id='usingWindsor'
			       type='Castle.Facilities.WcfIntegration.Demo.IAmUsingWindsor, 
				         Castle.Facilities.WcfIntegration.Demo'
			       wcfEndpointConfiguration='WSHttpBinding_IAmUsingWindsor'>
		</component>
	</components>
</configuration>";
	}

	public class EmptyInterceptor : MarshalByRefObject, IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
		}
	}

}
