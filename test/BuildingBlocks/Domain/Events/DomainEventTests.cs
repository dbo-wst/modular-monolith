namespace Template.Core.BuildingBlocks.Domain.Test.Events;

public class DomainEventTests
{
    private sealed record SomethingHappened(string What) : DomainEvent;

    private sealed class Thing : AggregateRoot<Guid>
    {
        public void Do(string what) => Raise(new SomethingHappened(what));
    }

    [Fact]
    public void NewEvent_GetsAUniqueIdAndTheCurrentUtcTime()
    {
        var before = DateTimeOffset.UtcNow;
        var first = new SomethingHappened("a");
        var second = new SomethingHappened("a");
        var after = DateTimeOffset.UtcNow;

        Assert.NotEqual(Guid.Empty, first.EventId);
        Assert.NotEqual(first.EventId, second.EventId);
        Assert.InRange(first.OccurredOn, before, after);
        Assert.Equal(TimeSpan.Zero, first.OccurredOn.Offset);
    }

    [Fact]
    public void EventsWithTheSamePayload_AreNotEqual_BecauseTheirMetadataDiffers()
    {
        Assert.NotEqual(new SomethingHappened("a"), new SomethingHappened("a"));
    }

    [Fact]
    public void Metadata_CanBeSetExplicitly_AndThenTakesPartInEquality()
    {
        var id = Guid.NewGuid();
        var at = new DateTimeOffset(2026, 7, 10, 12, 0, 0, TimeSpan.Zero);

        var first = new SomethingHappened("a") { EventId = id, OccurredOn = at };
        var second = new SomethingHappened("a") { EventId = id, OccurredOn = at };

        Assert.Equal(id, first.EventId);
        Assert.Equal(at, first.OccurredOn);
        Assert.Equal(first, second);
    }

    [Fact]
    public void AggregateRoot_ExposesRaisedEventsInOrder_AndClearsThem()
    {
        var thing = new Thing();
        thing.Do("one");
        thing.Do("two");

        Assert.Equal(["one", "two"], thing.DomainEvents.Cast<SomethingHappened>().Select(e => e.What));
        Assert.False(thing.DomainEvents is List<DomainEvent>, "events must not be exposed as a mutable list");

        thing.ClearDomainEvents();

        Assert.Empty(thing.DomainEvents);
    }
}
