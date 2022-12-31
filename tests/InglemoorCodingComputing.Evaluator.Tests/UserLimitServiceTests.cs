namespace InglemoorCodingComputing.Evaluator.UnitTests;

public class UserLimitServiceTests
{
    [Fact]
    public void UsersShouldNotBeAbleToExecuteCodeSimultaneously()
    {
        UserLimitService limitService = new();
        var id = Guid.NewGuid();
        Assert.True(limitService.TryLock(id));
        Assert.False(limitService.TryLock(id));
    }

    [Fact]
    public void DifferentUsersShouldBeAbleToExecuteCodeSimultaneously()
    {
        UserLimitService limitService = new();
        var id = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        Assert.True(limitService.TryLock(id));
        Assert.True(limitService.TryLock(id2));
    }

    [Fact]
    public void UsersShouldBeAbleToExecuteCodeAfterPreviousCompletion()
    {
        UserLimitService limitService = new();
        var id = Guid.NewGuid();
        Assert.True(limitService.TryLock(id));
        limitService.Release(id);
        Assert.True(limitService.TryLock(id));
    }

    [Fact]
    public void UsersShouldBeAbleToExecuteCodeAfter10SecondDelayEvenIfNotReleased()
    {
        UserLimitService limitService = new();
        var id = Guid.NewGuid();
        Assert.True(limitService.TryLock(id));
        Thread.Sleep(11000);
        Assert.True(limitService.TryLock(id));
    }

    [Fact]
    public void UsersShouldNotBeAbleToExecuteCodeBefore10SecondDelayIfNotReleased()
    {
        UserLimitService limitService = new();
        var id = Guid.NewGuid();
        Assert.True(limitService.TryLock(id));
        Thread.Sleep(9000);
        Assert.False(limitService.TryLock(id));
    }

    [Fact]
    public void ReleaseingANonExistantIdShouldntProduceAnException()
    {
        UserLimitService limitService = new();
        var id = Guid.NewGuid();
        Assert.Null(Record.Exception(() => limitService.Release(id)));
    }
}