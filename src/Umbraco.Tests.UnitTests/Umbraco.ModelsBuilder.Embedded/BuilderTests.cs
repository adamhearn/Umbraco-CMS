// Copyright (c) Umbraco.
// See LICENSE for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Semver;
using Umbraco.Cms.ModelsBuilder.Embedded;
using Umbraco.Cms.ModelsBuilder.Embedded.Building;

namespace Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded
{
    [TestFixture]
    public class BuilderTests
    {
        [Test]
        public void GenerateSimpleType()
        {
            // Umbraco returns nice, pascal-cased names.
            var type1 = new TypeModel
            {
                Id = 1,
                Alias = "type1",
                ClrName = "Type1",
                ParentId = 0,
                BaseType = null,
                ItemType = TypeModel.ItemTypes.Content,
            };
            type1.Properties.Add(new PropertyModel
            {
                Alias = "prop1",
                ClrName = "Prop1",
                ModelClrType = typeof(string),
            });

            TypeModel[] types = new[] { type1 };

            var modelsBuilderConfig = new ModelsBuilderSettings();
            var builder = new TextBuilder(modelsBuilderConfig, types);

            var sb = new StringBuilder();
            builder.Generate(sb, builder.GetModelsToGenerate().First());
            var gen = sb.ToString();

            SemVersion version = ApiVersion.Current.Version;
            var expected = @"//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v" + version + @"
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.ModelsBuilder.Embedded;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.PublishedModels
{
	[PublishedModel(""type1"")]
	public partial class Type1 : PublishedContentModel
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		public new const string ModelTypeAlias = ""type1"";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<Type1, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public Type1(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		[ImplementPropertyType(""prop1"")]
		public string Prop1 => this.Value<string>(_publishedValueFallback, ""prop1"");
	}
}
";
            Console.WriteLine(gen);
            Assert.AreEqual(expected.ClearLf(), gen);
        }

        [Test]
        public void GenerateSimpleType_Ambiguous_Issue()
        {
            // Umbraco returns nice, pascal-cased names.
            var type1 = new TypeModel
            {
                Id = 1,
                Alias = "type1",
                ClrName = "Type1",
                ParentId = 0,
                BaseType = null,
                ItemType = TypeModel.ItemTypes.Content,
            };
            type1.Properties.Add(new PropertyModel
            {
                Alias = "foo",
                ClrName = "Foo",
                ModelClrType = typeof(IEnumerable<>).MakeGenericType(ModelType.For("foo")),
            });

            var type2 = new TypeModel
            {
                Id = 2,
                Alias = "foo",
                ClrName = "Foo",
                ParentId = 0,
                BaseType = null,
                ItemType = TypeModel.ItemTypes.Element,
            };

            TypeModel[] types = new[] { type1, type2 };

            var modelsBuilderConfig = new ModelsBuilderSettings();
            var builder = new TextBuilder(modelsBuilderConfig, types)
            {
                ModelsNamespace = "Umbraco.Cms.Web.Common.PublishedModels"
            };

            var sb1 = new StringBuilder();
            builder.Generate(sb1, builder.GetModelsToGenerate().Skip(1).First());
            var gen1 = sb1.ToString();
            Console.WriteLine(gen1);

            var sb = new StringBuilder();
            builder.Generate(sb, builder.GetModelsToGenerate().First());
            var gen = sb.ToString();

            SemVersion version = ApiVersion.Current.Version;
            var expected = @"//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v" + version + @"
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.ModelsBuilder.Embedded;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.PublishedModels
{
	[PublishedModel(""type1"")]
	public partial class Type1 : PublishedContentModel
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		public new const string ModelTypeAlias = ""type1"";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<Type1, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public Type1(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Umbraco.ModelsBuilder.Embedded"", """ + version + @""")]
		[ImplementPropertyType(""foo"")]
		public global::System.Collections.Generic.IEnumerable<global::Foo> Foo => this.Value<global::System.Collections.Generic.IEnumerable<global::Foo>>(_publishedValueFallback, ""foo"");
	}
}
";
            Console.WriteLine(gen);
            Assert.AreEqual(expected.ClearLf(), gen);
        }

        [Test]
        public void GenerateAmbiguous()
        {
            var type1 = new TypeModel
            {
                Id = 1,
                Alias = "type1",
                ClrName = "Type1",
                ParentId = 0,
                BaseType = null,
                ItemType = TypeModel.ItemTypes.Content,
                IsMixin = true,
            };
            type1.Properties.Add(new PropertyModel
            {
                Alias = "prop1",
                ClrName = "Prop1",
                ModelClrType = typeof(IPublishedContent),
            });
            type1.Properties.Add(new PropertyModel
            {
                Alias = "prop2",
                ClrName = "Prop2",
                ModelClrType = typeof(StringBuilder),
            });
            type1.Properties.Add(new PropertyModel
            {
                Alias = "prop3",
                ClrName = "Prop3",
                ModelClrType = typeof(global::Umbraco.Cms.Core.Exceptions.BootFailedException),
            });
            TypeModel[] types = new[] { type1 };

            var modelsBuilderConfig = new ModelsBuilderSettings();
            var builder = new TextBuilder(modelsBuilderConfig, types)
            {
                ModelsNamespace = "Umbraco.ModelsBuilder.Models" // forces conflict with Umbraco.ModelsBuilder.Umbraco
            };

            var sb = new StringBuilder();
            foreach (TypeModel model in builder.GetModelsToGenerate())
            {
                builder.Generate(sb, model);
            }

            var gen = sb.ToString();

            Console.WriteLine(gen);

            Assert.IsTrue(gen.Contains(" global::Umbraco.Cms.Core.Models.PublishedContent.IPublishedContent Prop1"));
            Assert.IsTrue(gen.Contains(" global::System.Text.StringBuilder Prop2"));
            Assert.IsTrue(gen.Contains(" global::Umbraco.Cms.Core.Exceptions.BootFailedException Prop3"));
        }

        [TestCase("int", typeof(int))]
        [TestCase("global::System.Collections.Generic.IEnumerable<int>", typeof(IEnumerable<int>))]
        [TestCase("global::Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded.BuilderTestsClass1", typeof(BuilderTestsClass1))]
        [TestCase("global::Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded.BuilderTests.Class1", typeof(Class1))]
        public void WriteClrType(string expected, Type input)
        {
            // note - these assertions differ from the original tests in MB because in the embedded version, the result of Builder.IsAmbiguousSymbol is always true
            // which means global:: syntax will be applied to most things
            var builder = new TextBuilder
            {
                ModelsNamespaceForTests = "ModelsNamespace"
            };
            var sb = new StringBuilder();
            builder.WriteClrType(sb, input);
            Assert.AreEqual(expected, sb.ToString());
        }

        [TestCase("int", typeof(int))]
        [TestCase("global::System.Collections.Generic.IEnumerable<int>", typeof(IEnumerable<int>))]
        [TestCase("global::Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded.BuilderTestsClass1", typeof(BuilderTestsClass1))]
        [TestCase("global::Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded.BuilderTests.Class1", typeof(Class1))]
        public void WriteClrTypeUsing(string expected, Type input)
        {
            // note - these assertions differ from the original tests in MB because in the embedded version, the result of Builder.IsAmbiguousSymbol is always true
            // which means global:: syntax will be applied to most things
            var builder = new TextBuilder();
            builder.Using.Add("Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder");
            builder.ModelsNamespaceForTests = "ModelsNamespace";
            var sb = new StringBuilder();
            builder.WriteClrType(sb, input);
            Assert.AreEqual(expected, sb.ToString());
        }

        [Test]
        public void WriteClrType_WithUsing()
        {
            var builder = new TextBuilder();
            builder.Using.Add("System.Text");
            builder.ModelsNamespaceForTests = "Umbraco.Tests.UnitTests.Umbraco.ModelsBuilder.Models";
            var sb = new StringBuilder();
            builder.WriteClrType(sb, typeof(StringBuilder));

            // note - these assertions differ from the original tests in MB because in the embedded version, the result of Builder.IsAmbiguousSymbol is always true
            // which means global:: syntax will be applied to most things
            Assert.AreEqual("global::System.Text.StringBuilder", sb.ToString());
        }

        [Test]
        public void WriteClrTypeAnother_WithoutUsing()
        {
            var builder = new TextBuilder
            {
                ModelsNamespaceForTests = "Umbraco.Tests.UnitTests.Umbraco.ModelsBuilder.Models"
            };
            var sb = new StringBuilder();
            builder.WriteClrType(sb, typeof(StringBuilder));
            Assert.AreEqual("global::System.Text.StringBuilder", sb.ToString());
        }

        [Test]
        public void WriteClrType_Ambiguous1()
        {
            var builder = new TextBuilder();
            builder.Using.Add("System.Text");
            builder.Using.Add("Umbraco.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded");
            builder.ModelsNamespaceForTests = "SomeRandomNamespace";
            var sb = new StringBuilder();
            builder.WriteClrType(sb, typeof(global::System.Text.ASCIIEncoding));

            // note - these assertions differ from the original tests in MB because in the embedded version, the result of Builder.IsAmbiguousSymbol is always true
            // which means global:: syntax will be applied to most things
            Assert.AreEqual("global::System.Text.ASCIIEncoding", sb.ToString());
        }

        [Test]
        public void WriteClrType_Ambiguous()
        {
            var builder = new TextBuilder();
            builder.Using.Add("System.Text");
            builder.Using.Add("Umbraco.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded");
            builder.ModelsNamespaceForTests = "SomeBorkedNamespace";
            var sb = new StringBuilder();
            builder.WriteClrType(sb, typeof(global::System.Text.ASCIIEncoding));

            // note - these assertions differ from the original tests in MB because in the embedded version, the result of Builder.IsAmbiguousSymbol is always true
            // which means global:: syntax will be applied to most things
            Assert.AreEqual("global::System.Text.ASCIIEncoding", sb.ToString());
        }

        [Test]
        public void WriteClrType_Ambiguous2()
        {
            var builder = new TextBuilder();
            builder.Using.Add("System.Text");
            builder.Using.Add("Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded");
            builder.ModelsNamespaceForTests = "SomeRandomNamespace";
            var sb = new StringBuilder();
            builder.WriteClrType(sb, typeof(ASCIIEncoding));

            // note - these assertions differ from the original tests in MB because in the embedded version, the result of Builder.IsAmbiguousSymbol is always true
            // which means global:: syntax will be applied to most things
            Assert.AreEqual("global::Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded.ASCIIEncoding", sb.ToString());
        }

        [Test]
        public void WriteClrType_AmbiguousNot()
        {
            var builder = new TextBuilder();
            builder.Using.Add("System.Text");
            builder.Using.Add("Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded");
            builder.ModelsNamespaceForTests = "Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Models";
            var sb = new StringBuilder();
            builder.WriteClrType(sb, typeof(ASCIIEncoding));

            // note - these assertions differ from the original tests in MB because in the embedded version, the result of Builder.IsAmbiguousSymbol is always true
            // which means global:: syntax will be applied to most things
            Assert.AreEqual("global::Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded.ASCIIEncoding", sb.ToString());
        }

        [Test]
        public void WriteClrType_AmbiguousWithNested()
        {
            var builder = new TextBuilder();
            builder.Using.Add("System.Text");
            builder.Using.Add("Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded");
            builder.ModelsNamespaceForTests = "SomeRandomNamespace";
            var sb = new StringBuilder();
            builder.WriteClrType(sb, typeof(ASCIIEncoding.Nested));

            // note - these assertions differ from the original tests in MB because in the embedded version, the result of Builder.IsAmbiguousSymbol is always true
            // which means global:: syntax will be applied to most things
            Assert.AreEqual("global::Umbraco.Cms.Tests.UnitTests.Umbraco.ModelsBuilder.Embedded.ASCIIEncoding.Nested", sb.ToString());
        }

        public class Class1
        {
        }
    }

    // make it public to be ambiguous (see above)
    public class ASCIIEncoding
    {
        // can we handle nested types?
        public class Nested
        {
        }
    }

    public class BuilderTestsClass1
    {
    }

    public class System
    {
    }
}
