using System;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Automapping.TestFixtures;
using FluentNHibernate.Conventions;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel.ClassBased;
using NUnit.Framework;

namespace FluentNHibernate.Testing.ConventionsTests
{
    [TestFixture]
    public class ForeignKeyConventionTests
    {
        ConventionsCollection conventions;

        [SetUp]
        public void CreatePersistenceModel()
        {
            conventions = new ConventionsCollection {new TestForeignKeyConvention()};
        }

        [Test]
        public void ShouldSetForeignKeyOnManyToOne()
        {
            var classMap = new ClassMap<ExampleClass>();

            classMap.Id(x => x.Id);
            classMap.References(x => x.Parent);

            var instructions = new PersistenceInstructions();
            instructions.AddSource(new StubProviderSource(classMap));
            instructions.UseConventions(conventions);

            instructions.BuildMappings()
                .First()
                .Classes.First()
                .References.First()
                .Columns.First().Name.ShouldEqual("Parent!");
        }

        [Test]
        public void ShouldSetForeignKeyOnOneToMany()
        {
            var classMap = new ClassMap<ExampleInheritedClass>();

            classMap.Id(x => x.Id);
            classMap.HasMany(x => x.Children);

            var instructions = new PersistenceInstructions();
            instructions.AddSource(new StubProviderSource(classMap));
            instructions.UseConventions(conventions);

            instructions.BuildMappings()
                .First()
                .Classes.First()
                .Collections.First()
                .Key.Columns.First().Name.ShouldEqual("ExampleInheritedClass!");
        }

        [Test]
        public void ShouldSetForeignKeyOnManyToMany()
        {
            var classMap = new ClassMap<ExampleInheritedClass>();

            classMap.Id(x => x.Id);
            classMap.HasManyToMany(x => x.Children);

            var instructions = new PersistenceInstructions();
            instructions.AddSource(new StubProviderSource(classMap));
            instructions.UseConventions(conventions);

            instructions.BuildMappings()
                .First()
                .Classes.First()
                .Collections.First()
                .Key.Columns.First().Name.ShouldEqual("ExampleInheritedClass!");
        }

        [Test]
        public void ShouldSetForeignKeyOnJoin()
        {
            var classMap = new ClassMap<ExampleInheritedClass>();

            classMap.Id(x => x.Id);
            classMap.Join("two", m => { });

            var instructions = new PersistenceInstructions();
            instructions.AddSource(new StubProviderSource(classMap));
            instructions.UseConventions(conventions);

            instructions.BuildMappings()
                .First()
                .Classes.First()
                .Joins.First()
                .Key.Columns.First().Name.ShouldEqual("ExampleInheritedClass!");
        }

        [Test]
        public void ShouldSetForeignKeyOnJoinedSubclasses()
        {
            var classMap = new ClassMap<ExampleClass>();
            classMap.Id(x => x.Id);

            var subclassMap = new SubclassMap<ExampleInheritedClass>();

            var instructions = new PersistenceInstructions();
            instructions.AddSource(new StubProviderSource(classMap, subclassMap));
            instructions.UseConventions(conventions);

            instructions.BuildMappings()
                .First()
                .Classes.First()
                .Subclasses.First()
                .Key.Columns.First().Name.ShouldEqual("ExampleClass!");
        }

        private class TestForeignKeyConvention : ForeignKeyConvention
        {
            protected override string GetKeyName(Member property, Type type)
            {
                return property == null ? type.Name + "!" : property.Name + "!";
            }
        }
    }
}