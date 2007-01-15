namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Tests.Mocks;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultUrlBuilderTestCase
	{
		private DefaultUrlBuilder urlBuilder;
		private UrlInfo noAreaUrl, areaUrl, withSubDomain, diffPort;

		public DefaultUrlBuilderTestCase()
		{
			DefaultUrlTokenizer tokenizer = new DefaultUrlTokenizer();

			noAreaUrl = tokenizer.TokenizeUrl("/home/index.rails", new Uri("http://localhost/home/index.rails"), true, "/");
			areaUrl = tokenizer.TokenizeUrl("/area/home/index.rails", new Uri("http://localhost/area/home/index.rails"), true, "/");
			withSubDomain = tokenizer.TokenizeUrl("/app/home/index.rails", new Uri("http://sub.domain.com/app/home/index.rails"), false, "/app");
			diffPort = tokenizer.TokenizeUrl("/app/home/index.rails", new Uri("http://localhost:81/app/home/index.rails"), false, "/app");
		}

		[SetUp]
		public void Init()
		{
			urlBuilder = new DefaultUrlBuilder();
			urlBuilder.ServerUtil = new MockServerUtility();
		}

		[Test]
		public void SimpleOperations()
		{
			Assert.AreEqual("/product/list.rails", urlBuilder.BuildUrl(noAreaUrl, "product", "list"));
			Assert.AreEqual("/home/list.rails", urlBuilder.BuildUrl(noAreaUrl, DictHelper.Create("action=list")));
			Assert.AreEqual("/product/list.rails?id=1&name=hammett&", urlBuilder.BuildUrl(noAreaUrl, "product", "list", DictHelper.Create("id=1", "name=hammett")));
		}

		[Test]
		public void NoExtensions()
		{
			urlBuilder.UseExtensions = false;

			Assert.AreEqual("/product/list", urlBuilder.BuildUrl(noAreaUrl, "product", "list"));
			Assert.AreEqual("/home/list", urlBuilder.BuildUrl(noAreaUrl, DictHelper.Create("action=list")));
			Assert.AreEqual("/product/list?id=1&name=hammett&", urlBuilder.BuildUrl(noAreaUrl, "product", "list", DictHelper.Create("id=1", "name=hammett")));
			Assert.AreEqual("/area/home/list", urlBuilder.BuildUrl(areaUrl, DictHelper.Create("action=list")));
			Assert.AreEqual("/app/home/list", urlBuilder.BuildUrl(withSubDomain, DictHelper.Create("action=list")));
		}

		[Test]
		public void AbsoluteUrls()
		{
			Assert.AreEqual("http://localhost:81/app/home/list.rails", urlBuilder.BuildUrl(diffPort, DictHelper.Create("absolute=true", "action=list")));
		}

		[Test]
		public void AbsoluteUrlWithDifferentPort()
		{
			Assert.AreEqual("http://localhost:81/app/home/list.rails", urlBuilder.BuildUrl(diffPort, DictHelper.Create("absolute=true", "action=list")));
		}

		[Test]
		public void WithDomains()
		{
			Assert.AreEqual("http://sub.domain.com/app/home/list.rails", urlBuilder.BuildUrl(withSubDomain, DictHelper.Create("absolute=true", "action=list")));
		}

		[Test]
		public void SwitchingDomains()
		{
			Assert.AreEqual("http://testsub.domain.com/app/home/list.rails", urlBuilder.BuildUrl(withSubDomain, DictHelper.Create("absolute=true", "action=list", "subdomain=test")));
			Assert.AreEqual("http://something.else/app/home/list.rails", urlBuilder.BuildUrl(withSubDomain, DictHelper.Create("absolute=true", "action=list", "domain=something.else")));
		}
	}
}
