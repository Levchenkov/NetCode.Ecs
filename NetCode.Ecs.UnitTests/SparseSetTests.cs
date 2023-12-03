namespace NetCode.Ecs.UnitTests;

public class SparseSetTests
{
    [Fact]
    public void HasComponent_EmptySet_ShouldBeFalse()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        var result = sparseSet.HasComponent(1);

        result.Should().BeFalse();
    }

    [Fact]
    public void HasComponent_ComponentDoesntExist_ShouldBeFalse()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(1);

        var result = sparseSet.HasComponent(2);

        result.Should().BeFalse();
    }

    [Fact]
    public void HasComponent_ComponentExists_ShouldBeTrue()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(1);

        var result = sparseSet.HasComponent(1);

        result.Should().BeTrue();
    }

    [Fact]
    public void HasComponent_EntityIdIsBiggerThanMaxEntitiesCount_ShouldThrow()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        var action = () => sparseSet.HasComponent(10);

        action.Should().ThrowExactly<IndexOutOfRangeException>();
    }

    [Fact]
    public void AddComponent_AddMoreThanMaxComponentPerType_ShouldThrow()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(0);
        sparseSet.AddComponent(1);
        var action = () => sparseSet.AddComponent(2);

#if DEBUG
        action.Should().ThrowExactly<MaxCapacityException>();
#else
        action.Should().ThrowExactly<IndexOutOfRangeException>();
#endif
    }

    [Fact]
    public void GetComponent_OneComponentExists_ShouldGet()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(3).Max = 42;

        ref var c = ref sparseSet.GetComponent(3);
        c.Max.Should().Be(42);
    }

    [Fact]
    public void GetComponent_TwoComponentsExist_ShouldGet()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(3).Max = 42;
        sparseSet.AddComponent(5).Max = 69;

        ref var c = ref sparseSet.GetComponent(3);
        c.Max.Should().Be(42);

        ref var c2 = ref sparseSet.GetComponent(5);
        c2.Max.Should().Be(69);
    }

    [Fact]
    public void GetComponent_ComponentDoesntExist_ShouldThrow()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(0).Max = 42;

        var action = () => sparseSet.GetComponent(1);

#if DEBUG
        action.Should().ThrowExactly<ComponentNotFoundException>();
#else
        action.Should().ThrowExactly<IndexOutOfRangeException>();
#endif

    }

    [Fact]
    public void Components_TwoComponentsExist_ShouldBeTwo()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(0).Max = 42;
        sparseSet.AddComponent(1).Max = 69;

        var components = sparseSet.Components;
        components.Length.Should().Be(2);

        components[0].Max.Should().Be(42);
        components[1].Max.Should().Be(69);
    }

    [Fact]
    public void Entities_TwoComponentsExist_ShouldBeTwo()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(3);
        sparseSet.AddComponent(5);

        var entities = sparseSet.Entities;
        entities.Length.Should().Be(2);

        entities[0].Id.Should().Be(3);
        entities[1].Id.Should().Be(5);
    }

    [Fact]
    public void Components_EmptySet_ShouldBeEmpty()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.Components.Length.Should().Be(0);
    }

    [Fact]
    public void Entities_EmptySet_ShouldBeEmpty()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void AddComponent_AddTwiceTheSameEntity_ShouldReturnExisting()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(3).Max = 42;
        sparseSet.AddComponent(3).Max.Should().Be(42);
    }

    [Fact]
    public void DeleteComponent_ComponentDoesNotExist_ShouldBeOk()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.DeleteComponent(3);
    }

    [Fact]
    public void DeleteComponent_ComponentExists_ShouldBeOk()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(3).Max = 42;
        sparseSet.DeleteComponent(3);

        sparseSet.HasComponent(3).Should().BeFalse();
    }

    [Fact]
    public void Components_TwoComponentsExistDeleteOne_ShouldBeOne()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(3).Max = 42;
        sparseSet.AddComponent(5).Max = 69;

        sparseSet.DeleteComponent(3);

        sparseSet.Components[0].Max.Should().Be(69);
        sparseSet.Entities[0].Id.Should().Be(5);
    }

    [Fact]
    public void Components_TwoComponentsExistDeleteOneAddOne_ShouldBeTwo()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 2);

        sparseSet.AddComponent(3).Max = 42;
        sparseSet.AddComponent(5).Max = 69;

        sparseSet.DeleteComponent(3);

        sparseSet.AddComponent(9).Max = 13;

        sparseSet.Components[0].Max.Should().Be(69);
        sparseSet.Components[1].Max.Should().Be(13);
        sparseSet.Entities[0].Id.Should().Be(5);
        sparseSet.Entities[1].Id.Should().Be(9);
    }

    [Fact]
    public void Components_ThreeComponentsExistDeleteOne_ShouldBeTwo()
    {
        var sparseSet = new SparseSet<HealthComponent>(10, 3);

        sparseSet.AddComponent(3).Max = 42;
        sparseSet.AddComponent(5).Max = 69;
        sparseSet.AddComponent(9).Max = 13;

        sparseSet.DeleteComponent(5);

        sparseSet.Components[0].Max.Should().Be(42);
        sparseSet.Components[1].Max.Should().Be(13);
        sparseSet.Entities[0].Id.Should().Be(3);
        sparseSet.Entities[1].Id.Should().Be(9);
    }
}