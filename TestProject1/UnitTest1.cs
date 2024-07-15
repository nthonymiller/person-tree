using FluentAssertions;

namespace TestProject1;

public class UnitTest1
{
	[Theory]
	[InlineData(new[] { "Harry:Ron", "Ron:Ginny", "Ginny:Fran" }, "Harry", "Fran", 2)]
	[InlineData(new[] { "Harry:Ron", "Ron:Ginny", "Ginny:Donald", "Ginny:Fran" }, "Harry", "Fran", 2)]
	[InlineData(new[] { "Harry:Ron", "Ron:Sam", "Ron:Sally", "Ron:Ginny", "Ginny:Donald", "Ginny:Fran" }, "Harry", "Fran", 2)]
	[InlineData(new[] { "Harry:Ron", "Ron:Sam", "Ron:Sally", "Ron:Ginny", "Sally:Helen", "Ginny:Donald", "Ginny:Fran" }, "Harry", "Helen", 2)]
	[InlineData(new[] { "Harry:Ron", "Ron:Ginny", "Ginny:Fran" }, "Ron", "Fran", 1)]
	public void Test1(string[] names, string name1, string name2, int expected)
	{
		var tree = PersonTreeBuilder.BuildTree(names);
		PersonTreeBuilder.GetLinkCount(tree, name1, name2).Should().Be(expected);
	}
}

public static class PersonTreeBuilder
{
	public static Dictionary<string, Person> BuildTree(string[] names)
	{
		Dictionary<string, Person> index = new();

		foreach (var entry in names)
		{
			var items = entry.Split(":");

			var first = items[0];
			var second = items[1];

			if (!index.TryGetValue(first, out var firstPerson))
			{
				firstPerson = new Person(first);
				index[first] = firstPerson;
			}

			if (!index.TryGetValue(second, out var secondPerson))
			{
				secondPerson = new Person(second);
				index[second] = secondPerson;
			}

			if (firstPerson.RelatedTo.Any(x => x.Name == secondPerson.Name))
			{
				continue;
			}

			firstPerson.RelatedTo.Add(secondPerson);
		}

		return index;
	}

	public static int GetLinkCount(Dictionary<string, Person> tree, string name1, string name2)
	{
		if (!tree.TryGetValue(name1, out var person))
		{
			return 0;
		}

		var (found, count) = Traverse(person, name2);
		
		return found ? count - 1 : 0;
	}

	private static (bool, int) Traverse(Person person, string name)
	{
		if (person.Name == name)
		{
			return (true, 0);
		}

		var found = false;
		var count = 0;
		
		foreach (var relatedPerson in person.RelatedTo)
		{
			(found, count) = Traverse(relatedPerson, name);

			if (found)
			{
				return (true, count + 1);
			}
		}
		
		return (found, count);
	}
}

public class Person(string name)
{
	public string Name { get; } = name;
	public List<Person> RelatedTo { get; } = [];
}