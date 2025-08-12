using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;

namespace AccountService.Tests.HandlerTests.CommandTests;

public static class EntryMocking
{
    public static Mock<EntityEntry<TEntity>> MockEntityEntry<TContext, TEntity>(Mock<TContext> dbContextMock,
        TEntity entity)
        where TContext : DbContext
        where TEntity : class
    {
#pragma warning disable EF1001
        var stateManagerMock = new Mock<IStateManager>();

        stateManagerMock
            .Setup(x => x.CreateEntityFinder(It.IsAny<IEntityType>()))
            .Returns(new Mock<IEntityFinder>().Object);
        stateManagerMock
            .Setup(x => x.ValueGenerationManager)
            .Returns(new Mock<IValueGenerationManager>().Object);
        stateManagerMock
            .Setup(x => x.InternalEntityEntryNotifier)
            .Returns(new Mock<IInternalEntityEntryNotifier>().Object);

        var entityTypeMock = new Mock<IRuntimeEntityType>();
        var keyMock = new Mock<IKey>();
        keyMock
            .Setup(x => x.Properties)
            .Returns([]);
        entityTypeMock
            .Setup(x => x.FindPrimaryKey())
            .Returns(keyMock.Object);
        entityTypeMock
            .Setup(e => e.EmptyShadowValuesFactory)
            .Returns(() => new Mock<ISnapshot>().Object);

        var internalEntityEntry = new InternalEntityEntry(stateManagerMock.Object, entityTypeMock.Object, entity);

        var entityEntryMock = new Mock<EntityEntry<TEntity>>(internalEntityEntry);
        dbContextMock
            .Setup(c => c.Entry(It.IsAny<TEntity>()))
            .Returns(() => entityEntryMock.Object);

        return entityEntryMock;
    }
}