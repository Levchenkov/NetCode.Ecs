namespace NetCode.Ecs.UnitTests;

public class DenseSetTests
{
    [Fact]
    public void Count_ZeroCapacity_ShouldBe0()
    {
        var denseSet = new DenseSet<EmptyComponent>(0);

        denseSet.Count.Should().Be(0);
    }

    [Fact]
    public void Components_ZeroCapacity_ShouldBeEmpty()
    {
        var denseSet = new DenseSet<EmptyComponent>(0);

        denseSet.Components.Length.Should().Be(0);
    }

    [Fact]
    public void Entities_ZeroCapacity_ShouldBeEmpty()
    {
        var denseSet = new DenseSet<EmptyComponent>(0);

        denseSet.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void Add_ZeroCapacity_ExceptionExpected()
    {
        var denseSet = new DenseSet<EmptyComponent>(0);

        var action = () => denseSet.Add(new EntityId(42));
#if DEBUG
        action.Should().ThrowExactly<MaxCapacityException>();
#else
        action.Should().ThrowExactly<IndexOutOfRangeException>();
#endif

    }

    [Fact]
    public void Delete_ZeroCapacity_ExceptionExpected()
    {
        var denseSet = new DenseSet<EmptyComponent>(0);

        var action = () => denseSet.Delete(1);
#if DEBUG
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
#else
        action.Should().ThrowExactly<IndexOutOfRangeException>();
#endif

    }

    [Fact]
    public void GetByIndex_ZeroCapacity_ExceptionExpected()
    {
        var denseSet = new DenseSet<EmptyComponent>(0);

        var action = () => denseSet.GetByIndex(1);
#if DEBUG
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
#else
        action.Should().ThrowExactly<IndexOutOfRangeException>();
#endif

    }

    [Fact]
    public void Ctor_NegativeCapacity_ExceptionExpected()
    {
        var action = () => new DenseSet<EmptyComponent>(-1);

#if DEBUG
        action.Should().ThrowExactly<ArgumentOutOfRangeException>();
#else
        action.Should().ThrowExactly<OverflowException>();
#endif
    }

    [Fact]
    public void Count_CapacityIs1_ShouldBe0()
    {
        var denseSet = new DenseSet<EmptyComponent>(1);

        denseSet.Count.Should().Be(0);
    }

    [Fact]
    public void Components_CapacityIs1_ShouldBeEmpty()
    {
        var denseSet = new DenseSet<EmptyComponent>(1);

        denseSet.Components.Length.Should().Be(0);
    }

    [Fact]
    public void Entities_CapacityIs1_ShouldBeEmpty()
    {
        var denseSet = new DenseSet<EmptyComponent>(1);

        denseSet.Entities.Length.Should().Be(0);
    }

    [Fact]
    public void Add_CapacityIs1_ShouldBeAdded()
    {
        var denseSet = new DenseSet<EmptyComponent>(1);

        denseSet.Add(new EntityId(42));

        denseSet.Count.Should().Be(1);
        denseSet.Components.Length.Should().Be(1);
        denseSet.Entities.Length.Should().Be(1);

        denseSet.Entities[0].Id.Should().Be(42);
    }

    [Fact]
    public void GetByIndex_CapacityIs1_ShouldReturnAddedComponent()
    {
        var denseSet = new DenseSet<HealthComponent>(1);

        denseSet.Add(new EntityId(42)).Max = 69;

        denseSet.Components[0].Max.Should().Be(69);

        denseSet.GetByIndex(0).Max.Should().Be(69);
    }

    [Fact]
    public void Delete_CapacityIs1_ShouldDelete()
    {
        var denseSet = new DenseSet<HealthComponent>(1);

        ref var addedComponent = ref denseSet.Add(new EntityId(42));
        addedComponent.Max = 69;

        denseSet.Delete(0);
    }

    [Fact]
    public void Delete_Add2ComponentsDelete1_ShouldBeOk()
    {
        var denseSet = new DenseSet<HealthComponent>(2);

        denseSet.Add(new EntityId(42)).Max = 42;
        denseSet.Add(new EntityId(69)).Max = 69;

        denseSet.Components[0].Max.Should().Be(42);
        denseSet.Components[1].Max.Should().Be(69);

        denseSet.Entities[0].Id.Should().Be(42);
        denseSet.Entities[1].Id.Should().Be(69);

        var replacedEntityId = denseSet.Delete(0);

        replacedEntityId.Id.Should().Be(69);

        denseSet.Components[0].Max.Should().Be(69);

        denseSet.Entities[0].Id.Should().Be(69);
    }

    [Fact]
    public void Delete_Add2ComponentsDelete1Add1_ShouldBeOk()
    {
        var denseSet = new DenseSet<HealthComponent>(2);

        denseSet.Add(new EntityId(42)).Max = 42;
        denseSet.Add(new EntityId(69)).Max = 69;

        denseSet.Components[0].Max.Should().Be(42);
        denseSet.Components[1].Max.Should().Be(69);

        denseSet.Entities[0].Id.Should().Be(42);
        denseSet.Entities[1].Id.Should().Be(69);

        denseSet.Delete(0);

        denseSet.Add(new EntityId(1024)).Max = 1024;

        denseSet.Components[0].Max.Should().Be(69);
        denseSet.Components[1].Max.Should().Be(1024);

        denseSet.Entities[0].Id.Should().Be(69);
        denseSet.Entities[1].Id.Should().Be(1024);
    }

    [Fact]
    public void Delete_Add3ComponentsDelete1AtTheMiddle_ShouldBeOk()
    {
        var denseSet = new DenseSet<HealthComponent>(3);

        denseSet.Add(new EntityId(42)).Max = 42;
        denseSet.Add(new EntityId(69)).Max = 69;
        denseSet.Add(new EntityId(1024)).Max = 1024;

        denseSet.Delete(1);

        denseSet.Components[0].Max.Should().Be(42);
        denseSet.Components[1].Max.Should().Be(1024);

        denseSet.Entities[0].Id.Should().Be(42);
        denseSet.Entities[1].Id.Should().Be(1024);
    }
}